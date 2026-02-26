using System;
using System.Collections.Generic;

namespace Shared
{

    [Serializable]
    public class Data
    {
        public int SpinCooldown { get; set; }
        public List<SpinItem> SpinItems { get; set; } = new List<SpinItem>();
    }
}