namespace Shared.NetworkEvents.DataModification
{
    [NetworkEvent(NetworkDirection.ClientToServer)]
    public class RemoveSpinItem
    {
        public string ItemName { get; set; } = null!;
    }
}