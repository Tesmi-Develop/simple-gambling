using Shared;

namespace Server.Events.DataEvents;

public class AddedSpinItem
{
    public SpinItem Item { get; set; } = null!;
}