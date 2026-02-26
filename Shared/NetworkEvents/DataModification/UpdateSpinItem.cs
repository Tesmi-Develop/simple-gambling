namespace Shared.NetworkEvents.DataModification
{
    [NetworkEvent(NetworkDirection.ClientToServer)]
    public class UpdateSpinItem
    {
        public PatchSpinItem PatchSpinItem { get; set; } = null!;
    }
}