using Hypercube.Utilities.Debugging.Logger;
using Hypercube.Utilities.Dependencies;
using Server.ServiceRealisation;
using Server.Services.DTOHandlers;
using Server.Utility;
using Shared;
using Shared.NetworkEvents;

namespace Server.Services.Network;

[Service]
public class AdminService : IInitializable
{
    [Dependency] private readonly EventBus _eventBus = null!;
    [Dependency] private readonly UserStateService  _userStateService = null!;
    [Dependency] private readonly ILogger _logger = null!;
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
            _logger.Debug($"Client {client.UserState.Id} got admin.");
        });
        
        Console.WriteLine($"Admin password {_password}");
    }
}