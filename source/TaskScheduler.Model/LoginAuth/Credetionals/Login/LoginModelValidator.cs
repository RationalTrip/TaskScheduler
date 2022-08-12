using FluentValidation;
using TaskScheduler.Model.Credetionals.Login;

namespace TaskScheduler.Model
{
    public class LoginModelValidator:AbstractValidator<LoginModel>
    {
        public LoginModelValidator()
        {
            this.SetLoginRules();
        }
    }
}
