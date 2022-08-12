using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler.Domain;
using TaskScheduler.Model;

namespace TaskScheduler.Application
{
    public class LoginAuthService:ILoginAuthService
    {
        readonly ILoginAuthRepository _loginAuthRepository;
        public LoginAuthService(ILoginAuthRepository loginAuthRepository) => _loginAuthRepository = loginAuthRepository;

        public async Task<Result<LoginAuth>> RegisterAsync(LoginAuthRegisterModel registryModel)
        {
            var validator = new LoginAuthRegisterModelValidator().Validate(registryModel);

            if (validator.IsValid)
                return await _loginAuthRepository.CreateUserAsync(registryModel.ToLoginAuth());

            (ResultFailCode, string)[] validationFailCheck =
            {
                (ResultFailCode.BadLogin, nameof(LoginModel.Login)),
                (ResultFailCode.BadPassword, nameof(PasswordModel.Password)),
                (ResultFailCode.BadConfirmPassword, nameof(LoginAuthRegisterModel.ConfirmPassword))
            };

            return validator.ToResult<LoginAuth>(validationFailCheck);
        }
        public async Task<Result<LoginAuth>> SingInAsync(LoginAuthSignInModel signInModel)
        {
            var validator = new LoginAuthSignInModelValidator().Validate(signInModel);
            var loginValidator = new LoginModelValidator().Validate(new LoginModel(signInModel.Login));
            var passwordValidator = new PasswordModelValidator().Validate(new PasswordModel(signInModel.Password));

            if (!validator.IsValid || !loginValidator.IsValid || !passwordValidator.IsValid)
                return ApplicationCommonResults.GetBadLoginOrPasswordResult<LoginAuth>();

            return await _loginAuthRepository.SignInUserAsync(signInModel.ToLoginAuth());
        }
        public async Task<Result<bool>> IsLoginExistsAsync(LoginModel loginModel)
        {
            var validator = new LoginModelValidator().Validate(loginModel);

            if (validator.IsValid)
                return new Result<bool>
                {
                    IsSuccess = true,
                    SuccessResult = await _loginAuthRepository.IsLoginExistsAsync(loginModel.Login)
                };

            return new Result<bool> { IsSuccess = true, SuccessResult = false };
        }
    }
}
