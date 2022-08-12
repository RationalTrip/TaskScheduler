using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler.Domain
{
    public interface ILoginAuthRepository
    {
        Task<Result<LoginAuth>> CreateUserAsync(LoginAuth userAuth);
        Task<Result<LoginAuth>> SignInUserAsync(LoginAuth userToAuth);
        Task<bool> IsLoginExistsAsync(string login);
    }
}
