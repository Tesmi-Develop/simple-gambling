namespace Shared.NetworkEvents.DataModification
{
    /// <summary>
    /// Сетевой запрос на частичное изменение (патч) существующего элемента.
    /// Использует PatchSpinItem для обновления только измененных полей.
    /// Sprite должен быть представлен в виде Base64 строкой
    /// </summary>
    [NetworkEvent(NetworkDirection.ClientToServer)]
    public class RequestUpdateSpinItem
    {
        public PatchSpinItem PatchSpinItem { get; set; } = null!;
    }
}