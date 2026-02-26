namespace Shared.NetworkEvents
{
    [NetworkEvent(NetworkDirection.ServerToClient)]
    public class InitialDataSync
    {
        public Data CurrentData { get; set; } = null!;
    }
}