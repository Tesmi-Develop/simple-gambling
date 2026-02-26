namespace Shared.NetworkEvents
{
    /// <summary>
    /// Событие начальной синхронизации данных при подключении клиента.
    /// </summary>
    [NetworkEvent(NetworkDirection.ServerToClient)]
    public class InitialDataSync
    {
        public Data CurrentData { get; set; } = null!;
    }
}