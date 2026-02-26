using System;

namespace Shared
{
    public enum NetworkDirection
    {
        ServerToClient,
        ClientToServer
    }
    public class NetworkEventAttribute : Attribute
    {
        public NetworkDirection Direction { get; }

        public NetworkEventAttribute(NetworkDirection direction)
        {
            Direction = direction;
        }
    }
}