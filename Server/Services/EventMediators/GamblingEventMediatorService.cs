using Hypercube.Utilities.Dependencies;
using Server.ServiceRealisation;
using Server.Utility;
using Shared;
using Shared.NetworkEvents.Gambling;

namespace Server.Services;

[Service]
public sealed class GamblingEventMediatorService : IInitializable
{
    [Dependency] private readonly EventBus _eventBus = null!;
    [Dependency] private readonly CooldownService _cooldownService = null!;
    [Dependency] private readonly NetworkBroadcaster _networkBroadcaster = null!;
    [Dependency] private readonly GamblingService _gamblingService = null!;
    
    public void Init()
    {
        _eventBus.Subscribe<SpinRequested>(requested =>
        {
            var client = requested.GetSender();
            if (!_cooldownService.IsReady(client))
                return;

            var result = _gamblingService.Spin();
            _networkBroadcaster.SendEvent(client, new SpinCompleted { Prize = result });
            _cooldownService.GiveCooldown(client);
        });
    }
}