using Shared;

namespace Server.Events.DataEvents;

public class UpdatedSpinItem
{
    public SpinItem Item { get; set; } = null!;
}