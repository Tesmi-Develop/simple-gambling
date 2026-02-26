using Hypercube.Utilities.Dependencies;
using Server.DTO;
using Server.Events.ClientEvents;
using Server.ServiceRealisation;
using Server.Services.Network;
using Shared;
using Shared.NetworkEvents;

namespace Server.Services.DTOHandlers;

[Service]
public sealed class UserStateService : IInitializable
{
    [Dependency] private readonly EventBus _eventBus = null!;
    [Dependency] private readonly NetworkBroadcaster _networkBroadcaster = null!;
    
    public void MutateUserState(Client client, Action<IUserState> action)
    {
        action(client.UserState);
        _networkBroadcaster.SendEvent(client, new UserStateSynced { UserState = client.UserState });
    }

    public void Init()
    {
        _eventBus.Subscribe<ClientConnected>(arg =>
        {
            _networkBroadcaster.SendEvent(arg.Client, new UserStateSynced { UserState = arg.Client.UserState });
        });
    }
}