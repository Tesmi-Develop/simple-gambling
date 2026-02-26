namespace Shared.NetworkEvents.Gambling
{
    // <summary>
    /// Событие завершения крутки (I CAN'T STOP WINNING!!!).
    /// </summary>
    [NetworkEvent(NetworkDirection.ServerToClient)]
    public class SpinCompleted
    {
        public SpinItem Prize { get; set; } = null!;
    }
}