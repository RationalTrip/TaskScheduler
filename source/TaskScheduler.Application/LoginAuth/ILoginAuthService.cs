using System.Threading.Tasks;
using TaskScheduler.Domain;
using TaskScheduler.Model;

namespace TaskScheduler.Application
{
    public interface ILoginAuthService
    {
        Task<Result<LoginAuth>> RegisterAsync(LoginAuthRegisterModel registryModel);
        Task<Result<LoginAuth>> SingInAsync(LoginAuthSignInModel signInModel);
        Task<Result<bool>> IsLoginExistsAsync(LoginModel loginModel);
    }
}
