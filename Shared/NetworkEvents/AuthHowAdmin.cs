namespace Shared.NetworkEvents
{
    /// <summary>
    /// Событие авторизации клиента как администратора.
    /// </summary>
    public class AuthHowAdmin
    {
        public string Password { get; set; } = null!;
    }
}