using FluentValidation;

namespace TaskScheduler.Model
{
    public class LoginAuthSignInModelValidator:AbstractValidator<LoginAuthSignInModel>
    {
        public LoginAuthSignInModelValidator()
        {
            RuleFor(authLogIn => authLogIn.Login).NotEmpty();
            RuleFor(authLogIn => authLogIn.Password).NotEmpty();
        }
    }
}