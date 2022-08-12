using FluentValidation;

namespace TaskScheduler.Model.Credetionals.Password
{
    static class PasswordModelExtensions
    {
        public static void SetPasswordRules<T>(this AbstractValidator<T> passwordValidator) where T : IPasswordContainer
        {
            passwordValidator.RuleFor(auth => auth.Password).NotEmpty()
                .WithMessage("Password is required!");

            passwordValidator.RuleFor(auth => auth.Password).MinimumLength(6)
                .WithMessage("Password should contain at least {MinLength} character!");
            passwordValidator.RuleFor(auth => auth.Password).MaximumLength(255)
                .WithMessage("Password maximul length is {MaxLength} character!");

            passwordValidator.RuleFor(auth => auth.Password).Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d_@$!%*?&]+$")
                .WithMessage("Password should contain at least one digit!<br>" +
                "Password should contain at least one upper case letter!<br>" +
                "Password should contain at least one lower case letter!<br>" +
                "Password could contain only digits, upper and lower cases letters and special symbols \"_\", \"@\", \"$\", \"!\", \"%\", \"*\", \"?\" and \"&\"");
        }
    }
}
