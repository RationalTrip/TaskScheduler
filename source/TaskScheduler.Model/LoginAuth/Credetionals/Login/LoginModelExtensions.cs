using FluentValidation;

namespace TaskScheduler.Model.Credetionals.Login
{
    static class LoginModelExtensions
    {
        public static void SetLoginRules<T>(this AbstractValidator<T> loginValidator) where T: ILoginContainer
        {
            loginValidator.RuleFor(login => login.Login).MinimumLength(3)
                .WithMessage("Login required Lenght is {MinLength}.");
            loginValidator.RuleFor(login => login.Login).MaximumLength(255)
                .WithMessage("Login maximum Lenght is {MaxLength}.");
            loginValidator.RuleFor(login => login.Login).NotEmpty()
                .WithMessage("Login must not be empty.");
            loginValidator.RuleFor(login => login.Login).Matches(@"^[0-9a-zA-Z_]+$")
                .WithMessage("Login should contain only digits, English letters or underscore.");
        }
    }
}
