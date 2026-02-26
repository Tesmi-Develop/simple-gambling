using Shared;

namespace Server.DTO;

public class UserState : IUserState
{
    public string Id { get; }
    public bool IsAdmin { get; set; } = false;
    public long SpinCooldownEnd { get; set; }

    public UserState(string id)
    {
        Id = id;
    }
}