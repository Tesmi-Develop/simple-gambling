namespace Shared.NetworkEvents.DataModification
{
    /// <summary>
    /// Сетевой запрос на удаление существующего элемента из списка.
    /// Содержит уникальный идентификатор (Name) удаляемого объекта.
    /// </summary>
    [NetworkEvent(NetworkDirection.ClientToServer)]
    public class RequestRemoveSpinItem
    {
        public string ItemName { get; set; } = null!;
    }
}