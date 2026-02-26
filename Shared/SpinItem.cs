using System;

namespace Shared
{
    [Serializable]
    public class SpinItem
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Sprite { get; set; } = string.Empty;
        public int Weight { get; set; } = 0;
    }
}