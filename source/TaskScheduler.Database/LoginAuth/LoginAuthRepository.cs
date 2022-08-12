using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskScheduler.Domain;

namespace TaskScheduler.Database
{
    public class LoginAuthRepository:ILoginAuthRepository
    {
        readonly ISaltGenerator _saltGenerator;
        readonly IPasswordHasher _passwordHasher;
        readonly TaskSchedulerContext _context;

        public LoginAuthRepository(TaskSchedulerContext context, ISaltGenerator saltGenerator,
            IPasswordHasher passwordHasher)
        {
            _context = context;
            _saltGenerator = saltGenerator;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<LoginAuth>> CreateUserAsync(LoginAuth userAuth)
        {
            if (await IsLoginExistsAsync(userAuth.Login))
                return DatabaseCommonResults.GetLoginAlreadyExistsResult<LoginAuth>();

            string salt = _saltGenerator.GenerateSalt();
            userAuth.SetSalt(salt);

            string password = _passwordHasher.HashPassword(salt, userAuth.Password);
            userAuth.SetPassword(password);

            var user = new User(userAuth);

            await _context.LoginAuths.AddAsync(userAuth);
            await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();

            return DatabaseCommonResults.GetSuccessResult(userAuth);
        }

        public async Task<Result<LoginAuth>> SignInUserAsync(LoginAuth userToAuth)
        {
            LoginAuth trueUserAuth = await _context.LoginAuths.Where(auth => auth.Login == userToAuth.Login)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (trueUserAuth is null)
                return DatabaseCommonResults.GetBadLoginOrPasswordResult<LoginAuth>();

            string salt = trueUserAuth.Salt;
            string passwordHashed = _passwordHasher.HashPassword(salt, userToAuth.Password);

            if (passwordHashed != trueUserAuth.Password)
                return DatabaseCommonResults.GetBadLoginOrPasswordResult<LoginAuth>();

            return DatabaseCommonResults.GetSuccessResult(trueUserAuth);
        }

        public async Task<bool> IsLoginExistsAsync(string login)
        {
            return await _context.LoginAuths.AnyAsync(loginAuth => loginAuth.Login == login);
        }
    }
}
