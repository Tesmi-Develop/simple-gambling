using Server.DTO;

namespace Server.Events.ClientEvents;

public class ClientDisconnected
{
    public Client Client = null!;
}