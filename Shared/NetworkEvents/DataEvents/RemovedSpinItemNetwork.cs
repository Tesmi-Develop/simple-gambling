namespace Shared.NetworkEvents.DataEvents
{
    /// <summary>
    /// Сетевое событие, уведомляющее клиентов об удалении SpinItem.
    /// </summary>
    [NetworkEvent(NetworkDirection.ServerToClient)]
    public class RemovedSpinItemNetwork
    {
        public string ItemName { get; set; } = string.Empty;
    }
}