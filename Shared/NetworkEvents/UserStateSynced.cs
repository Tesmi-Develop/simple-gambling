namespace Shared.NetworkEvents
{
    [NetworkEvent(NetworkDirection.ServerToClient)]
    public class UserStateSynced
    {
        public IUserState UserState { get; set; } = null!;
    }
}