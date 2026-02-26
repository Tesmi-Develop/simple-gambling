namespace Shared.NetworkEvents.DataEvents
{
    [NetworkEvent(NetworkDirection.ServerToClient)]
    public class UpdatedSpinItemNetwork
    {
        public SpinItem SpinItem { get; set; }
    }
}