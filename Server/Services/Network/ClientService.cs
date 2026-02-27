using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using Hypercube.Utilities.Debugging.Logger;
using Hypercube.Utilities.Dependencies;
using Server.DTO;
using Server.Events.ClientEvents;
using Server.ServiceRealisation;
using Shared;

namespace Server.Services.Network;

[Service]
public sealed class ClientService : IInitializable
{
    [Dependency] private readonly NetworkServerService _networkServerService = null!;
    [Dependency] private readonly EventBus _eventBus = null!;
    [Dependency] private readonly DependenciesContainer _dependencies = null!;
    [Dependency] private readonly ILogger _logger = null!;
    
    public IReadOnlyDictionary<TcpClient, Client> Clients => _clients;
    private readonly Dictionary<TcpClient, Client> _clients = [];
    private readonly Dictionary<string, Client> _clientsById = [];

    private void RegisterClient(TcpClient socket)
    {
        if (_clients.ContainsKey(socket))
            return;
        
        var client = new Client(socket);
        _dependencies.Inject(client);
        
        _clients.Add(socket, client);
        _clientsById.Add(client.UserState.Id, client);
        _eventBus.Publish(new ClientConnected
        {
            Client = client,
        });
        _logger.Debug($"Created new client with id {client.UserState.Id}");
    }

    private void UnregisterClient(TcpClient socket)
    {
        if (!_clients.Remove(socket, out var client))
            return;

        _clientsById.Remove(client.UserState.Id);
        _eventBus.Publish(new ClientDisconnected
        {
            Client = client,
        });
        _logger.Debug($"Removed client with id {client.UserState.Id}");
    }

    public Client GetClient(TcpClient client)
    {
        return _clients[client];
    }

    public bool TryGetClient(TcpClient socket, [MaybeNullWhen(false)] out Client client)
    {
        return _clients.TryGetValue(socket, out client);
    }

    public Client GetClientById(string id)
    {
        return _clientsById[id];
    }
    
    public void Init()
    {
        _networkServerService.OnConnect += RegisterClient;
        _networkServerService.OnDisconnect += UnregisterClient;
    }
}