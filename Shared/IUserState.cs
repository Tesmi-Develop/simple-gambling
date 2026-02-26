namespace Shared
{
    public interface IUserState
    {
        string Id { get; }
        bool IsAdmin { get; set; }
        long SpinCooldownEnd { get; set; }
    }
}