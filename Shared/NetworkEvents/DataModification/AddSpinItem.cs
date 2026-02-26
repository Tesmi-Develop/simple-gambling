namespace Shared.NetworkEvents.DataModification
{
    [NetworkEvent(NetworkDirection.ClientToServer)]
    public class AddSpinItem
    {
        public SpinItem NewItem { get; set; } = null!;
    }
}