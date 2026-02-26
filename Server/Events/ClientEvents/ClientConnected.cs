using Server.DTO;

namespace Server.Events.ClientEvents;

public class ClientConnected
{
    public Client Client = null!;
}