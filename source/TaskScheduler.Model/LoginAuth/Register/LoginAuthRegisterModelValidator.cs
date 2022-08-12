using FluentValidation;
using TaskScheduler.Model.Credetionals.Login;
using TaskScheduler.Model.Credetionals.Password;

namespace TaskScheduler.Model
{
    public class LoginAuthRegisterModelValidator:AbstractValidator<LoginAuthRegisterModel>
    {
        public LoginAuthRegisterModelValidator()
        {
            this.SetLoginRules();

            this.SetPasswordRules();

            RuleFor(auth => auth.ConfirmPassword).Equal(auth => auth.Password)
                .WithMessage("Confirm Password should be same as Password!");
        }
    }
}
