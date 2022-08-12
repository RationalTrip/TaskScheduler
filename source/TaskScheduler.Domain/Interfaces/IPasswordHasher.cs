namespace TaskScheduler.Domain
{
    public interface IPasswordHasher
    {
        string HashPassword(string salt, string password);
    }
}