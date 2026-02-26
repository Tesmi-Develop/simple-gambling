namespace Shared.NetworkEvents.DataModification
{
    /// <summary>
    /// Сетевой запрос на добавление нового элемента в список прокрутки.
    /// Sprite должен быть представлен в виде Base64 строкой
    /// </summary>
    [NetworkEvent(NetworkDirection.ClientToServer)]
    public class RequestAddSpinItem
    {
        public SpinItem NewItem { get; set; } = null!;
    }
}