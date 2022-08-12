using Microsoft.AspNetCore.Mvc;
using TaskScheduler.Domain;
using TaskScheduler.Model.Credetionals.Login;
using TaskScheduler.Model.Credetionals.Password;

namespace TaskScheduler.Model
{
    public class LoginAuthRegisterModel : ILoginContainer, IPasswordContainer
    {
        [Remote("is-login-exists", "auth", ErrorMessage = "This login already exist", HttpMethod ="POST")]
        public string Login { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public LoginAuth ToLoginAuth() => new (Login, Password);
    }
}
