using TaskScheduler.Domain;

namespace TaskScheduler.Model
{
    public class LoginAuthSignInModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public LoginAuth ToLoginAuth() => new (Login, Password);
    }
}
