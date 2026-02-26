namespace Shared.Events
{
    [NetworkEvent(NetworkDirection.ServerToClient)]
    public class UserStateSynced
    {
        public IUserState UserState { get; set; } = null!;
    }
}