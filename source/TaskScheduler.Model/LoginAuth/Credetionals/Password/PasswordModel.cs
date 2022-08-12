using TaskScheduler.Model.Credetionals.Password;

namespace TaskScheduler.Model
{
    public class PasswordModel : IPasswordContainer
    {
        public PasswordModel(string password) => Password = password;
        public string Password { get; set; }
    }
}
