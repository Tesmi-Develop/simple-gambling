namespace Shared.NetworkEvents.DataEvents
{
    /// <summary>
    /// Сетевое событие, отправляемое клиентам при добавлении нового SpinItem.
    /// Используется для синхронизации состояния после успешного создания предмета.
    /// </summary>
    [NetworkEvent(NetworkDirection.ServerToClient)]
    public class AddedSpinItemNetwork
    {
        public SpinItem SpinItem { get; set; }
    }
}