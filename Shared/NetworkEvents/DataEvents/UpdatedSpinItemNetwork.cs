namespace Shared.NetworkEvents.DataEvents
{
    /// <summary>
    /// Сетевое событие, уведомляющее клиентов об обновлении SpinItem.
    /// </summary>
    [NetworkEvent(NetworkDirection.ServerToClient)]
    public class UpdatedSpinItemNetwork
    {
        public SpinItem SpinItem { get; set; }
    }
}