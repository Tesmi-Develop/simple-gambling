using System.Runtime.CompilerServices;
using Server.DTO;

namespace Server.Utility;

public static class NetworkEventMetadata
{
    private static readonly ConditionalWeakTable<object, Client> Senders = new();

    public static void SetSender(object ev, Client client) => Senders.Add(ev, client);
    
    public static Client GetSender(this object ev) 
    {
        Senders.TryGetValue(ev, out var client);
        if (client is null)
            throw new Exception("Tried to get sender outside of a network event");
        
        return client;
    }
}