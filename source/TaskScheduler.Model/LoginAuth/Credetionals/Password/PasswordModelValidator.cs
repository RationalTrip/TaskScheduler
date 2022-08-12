using FluentValidation;
using TaskScheduler.Model.Credetionals.Password;

namespace TaskScheduler.Model
{
    public class PasswordModelValidator:AbstractValidator<PasswordModel>
    {
        public PasswordModelValidator()
        {
            this.SetPasswordRules();
        }
    }
}
