using Hypercube.Utilities.Dependencies;
using Server.ServiceRealisation;
using Server.Services.DTOHandlers;
using Server.Utility;
using Shared;
using Shared.NetworkEvents;

namespace Server.Services.Network;

public class AdminService : IInitializable
{
    [Dependency] private readonly EventBus _eventBus = null!;
    [Dependency] private readonly UserStateService  _userStateService = null!;
    private string _password = null!;
    
    public void Init()
    {
        _password = Guid.NewGuid().ToString();
        
        _eventBus.Subscribe<AuthHowAdmin>(args =>
        {
            var client = args.GetSender();
            if (client.UserState.IsAdmin)
                return;
            
            if (args.Password != _password)
                return;
            
            _userStateService.MutateUserState(client, state =>
            {
                state.IsAdmin = true;
            });
        });
        
        Console.WriteLine($"Admin password {_password}");
    }
}