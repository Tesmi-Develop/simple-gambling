namespace Shared.NetworkEvents
{
    /// <summary>
    /// Событие вызываемое при изменении состояния клиента.
    /// </summary>
    [NetworkEvent(NetworkDirection.ServerToClient)]
    public class UserStateSynced
    {
        public IUserState UserState { get; set; } = null!;
    }
}