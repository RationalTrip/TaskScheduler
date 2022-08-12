namespace TaskScheduler.Domain
{
    public class LoginAuth
    {
        public LoginAuth() { }
        public LoginAuth(string login, string password)
        {
            Login = login;
            Password = password;
        }
        public int AuthId { get; init; }
        public string Login { get; private set; }
        public string Salt { get; private set; }
        public string Password { get; private set; }
        public User User { get; private set; }
        public void SetPassword(string password) => Password = password;
        public void SetSalt(string salt) => Salt = salt;
    }
}
