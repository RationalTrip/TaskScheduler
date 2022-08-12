using TaskScheduler.Model.Credetionals.Login;

namespace TaskScheduler.Model
{
    public class LoginModel: ILoginContainer
    {
        public LoginModel(string login) => Login = login;
        public string Login { get; set; }
    }
}
