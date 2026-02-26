namespace Shared.NetworkEvents.DataEvents
{
    [NetworkEvent(NetworkDirection.ServerToClient)]
    public class AddedSpinItemNetwork
    {
        public SpinItem SpinItem { get; set; }
    }
}