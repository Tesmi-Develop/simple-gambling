namespace Shared.Events.Gambling
{
    [NetworkEvent(NetworkDirection.ServerToClient)]
    public class SpinCompleted
    {
        public SpinItem Prize { get; set; } = null!;
    }
}