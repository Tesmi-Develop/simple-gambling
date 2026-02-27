using Hypercube.Utilities.Debugging.Logger;
using Hypercube.Utilities.Dependencies;
using Server.DTO;
using Server.ServiceRealisation;
using Server.Services.DTOHandlers;

namespace Server.Services.Mechanics;

[Service]
public sealed class CooldownService
{
    [Dependency] private readonly DataTrackerService _dataTrackerService = null!;
    [Dependency] private readonly UserStateService _userStateService = null!;
    [Dependency] private readonly ILogger _logger = null!;
    
    public bool IsReady(Client client)
    {
        var userState = client.UserState;
        var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return currentTime > userState.SpinCooldownEnd;
    }

    public void GiveCooldown(Client client)
    {
        _userStateService.MutateUserState(client, state =>
        {
            state.SpinCooldownEnd = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + _dataTrackerService.Read().SpinCooldown;
            _logger.Debug($"Client {client.UserState.Id} got cooldown {state.SpinCooldownEnd}");
        });
    }
}