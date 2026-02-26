using Server.DTO;

namespace Server.Events;

public enum BroadcastType
{
    All,
    Targeted,
}

public class SentNetworkEvent
{
    public BroadcastType BroadcastType = BroadcastType.All;
    public List<Client> Clients = [];
    public object EventData = null!;
    public Type EventType = null!;
}