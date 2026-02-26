using System.Net.Sockets;
using System.Reflection;
using Hypercube.Utilities.Dependencies;
using Server.DTO;
using Server.Events;
using Server.ServiceRealisation;
using Server.Utility;
using Shared;

namespace Server.Services;

[Service]
public sealed class NetworkBroadcaster : IInitializable
{
    [Dependency] private readonly NetworkServerService _networkServerService = null!;
    [Dependency] private readonly ClientService _clientService = null!;
    [Dependency] private readonly EventBus _eventBus = null!;

    private bool IsNetworkEvent(Type type)
    {
        return type.IsDefined(typeof(NetworkEventAttribute), false);
    }

    private void SendNetworkEvent(SendedNetworkEvent networkEvent)
    {
        var packet = new Packet
        {
            EventName = networkEvent.EventType.Name,
            Data = networkEvent.EventData
        };

        if (networkEvent.BroadcastType == BroadcastType.All)
        {
            foreach (var (_, client) in _clientService.Clients)
            {
                _ = client.Send(packet);
            }
            return;
        }

        foreach (var client in networkEvent.Clients)
        {
            _ = client.Send(packet);
        }
    }
    
    private void PrepareAndPublish<T>(T eventData, BroadcastType broadcastType, List<Client> clients = null) where T : class
    {
        var type = typeof(T);
        
        if (!IsNetworkEvent(type))
            throw new Exception($"Type {type.Name} is not a network event");
        
        var attribute = type.GetCustomAttribute<NetworkEventAttribute>(false);
        if (attribute is null)
            throw new Exception($"NetworkEventAttribute is missing on {type.Name}");

        if (attribute.Direction != NetworkDirection.ServerToClient)
            throw new Exception($"Cannot send {type.Name}: direction is not ServerToClient");
        
        SendNetworkEvent(new SendedNetworkEvent
        {
            BroadcastType = broadcastType,
            Clients = clients,
            EventData = eventData,
            EventType = type,
        });
    }
    
    private void HandleClientPackage(TcpClient socket, byte[] message)
    {
        if (!_clientService.TryGetClient(socket, out var client))
            return;
        
        var (packet, eventType) = client!.Receive(message);

        var networkEventAttribute = eventType.GetCustomAttribute<NetworkEventAttribute>(false);
        if (networkEventAttribute is null || networkEventAttribute.Direction != NetworkDirection.ClientToServer)
            return;
        
        NetworkEventMetadata.SetSender(packet.Data, client);
        _eventBus.Publish(packet.Data, eventType);
    }
    
    public void SendEvent<T>(List<Client> clients, T eventData) where T : class
    {
        PrepareAndPublish(eventData, BroadcastType.Targeted, clients);
    }
    
    public void SendEvent<T>(Client client, T eventData) where T : class
    {
        PrepareAndPublish(eventData, BroadcastType.Targeted, [client]);
    }

    public void SendEvent<T>(T eventData) where T : class
    {
        PrepareAndPublish(eventData, BroadcastType.All);
    }

    public void Init()
    {
        _networkServerService.OnReceive += HandleClientPackage;
    }
}