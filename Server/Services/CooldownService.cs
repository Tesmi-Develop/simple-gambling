using Hypercube.Utilities.Dependencies;
using Server.DTO;
using Server.ServiceRealisation;

namespace Server.Services;

[Service]
public sealed class CooldownService
{
    [Dependency] private readonly DataTrackerService _dataTrackerService = null!;
    [Dependency] private readonly UserStateService _userStateService = null!;
    
    public bool IsReady(Client client)
    {
        var userState = client.UserState;
        var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return currentTime > userState.SpinCooldownEnd;
    }

    public void GiveCooldown(Client client)
    {
        _userStateService.MutateUserState(client, (state) =>
        {
            state.SpinCooldownEnd = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + _dataTrackerService.Read().SpinCooldown;
        });
    }
}