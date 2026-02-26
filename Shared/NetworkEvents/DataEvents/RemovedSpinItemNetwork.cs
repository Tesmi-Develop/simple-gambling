namespace Shared.NetworkEvents.DataEvents
{
    [NetworkEvent(NetworkDirection.ServerToClient)]
    public class RemovedSpinItemNetwork
    {
        public string ItemName { get; set; } = string.Empty;
    }
}