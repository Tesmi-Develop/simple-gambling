namespace Shared
{
    public interface IUserState
    {
        string Id { get; }
        long SpinCooldownEnd { get; set; }
    }
}