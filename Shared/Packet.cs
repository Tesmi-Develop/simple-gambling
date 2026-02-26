using System;

namespace Shared
{
    [Serializable]
    public class Packet
    {
        public string EventName { get; set; } = string.Empty;
        public object Data { get; set; } = null!;
    }
}