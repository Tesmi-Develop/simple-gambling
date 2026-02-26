using Server.DTO;

namespace Server.Events;

public class ClientDisconnected
{
    public Client Client = null!;
}