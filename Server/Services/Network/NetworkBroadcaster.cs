using System.Collections.Frozen;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Hypercube.Utilities.Dependencies;
using Server.DTO;
using Server.Events;
using Server.ServiceRealisation;
using Server.Utility;
using Shared;

namespace Server.Services.Network;

[Service]
public sealed class NetworkBroadcaster : IInitializable
{
    [Dependency] private readonly NetworkServerService _networkServerService = null!;
    [Dependency] private readonly ClientService _clientService = null!;
    [Dependency] private readonly EventBus _eventBus = null!;
    private static FrozenDictionary<string, Type>  _types = null!;

    private bool IsNetworkEvent(Type type)
    {
        return type.IsDefined(typeof(NetworkEventAttribute), false);
    }

    private void SendNetworkEvent(SentNetworkEvent networkEvent)
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
        
        SendNetworkEvent(new SentNetworkEvent
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
        
        var json = Encoding.UTF8.GetString(message);
        var packetObject = JsonSerializer.Deserialize<Packet>(json);
        if (packetObject == null)
            throw new Exception("Packet object is null");

        var jsonData = (JsonElement)packetObject.Data;
        
        if (!_types.TryGetValue(packetObject.EventName, out var eventType))
            throw new Exception($"Unknown packet type, Got event name: {packetObject.EventName}, JSON: {json}");
        
        var arg = jsonData.Deserialize(eventType);
        if (arg is null)
            throw new Exception($"Unknown packet type, Got null when deserialize event data. Event name: {packetObject.EventName}, JSON: {json}");
        
        packetObject.Data = arg;

        var networkEventAttribute = eventType.GetCustomAttribute<NetworkEventAttribute>(false);
        if (networkEventAttribute is null || networkEventAttribute.Direction != NetworkDirection.ClientToServer)
            return;
        
        NetworkEventMetadata.SetSender(packetObject.Data, client);
        _eventBus.Publish(packetObject.Data, eventType);
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
        var types = new Dictionary<string, Type>();
        foreach (var (type, data) in ReflectionHelper.GetAllTypes<NetworkEventAttribute>())
        {
            types[type.Name] = type;
        }

        _types = types.ToFrozenDictionary();
        
        _networkServerService.OnReceive += HandleClientPackage;
    }
}