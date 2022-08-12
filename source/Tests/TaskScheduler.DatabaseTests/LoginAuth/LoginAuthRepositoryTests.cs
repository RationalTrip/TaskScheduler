using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskScheduler.Domain;
using TaskScheduler.Model;
using System;

namespace TaskScheduler.Database.Tests
{
    [TestClass()]
    public class LoginAuthRepositoryTests
    {
        static int databaseAndTestDataSplitter;

        readonly static string defaultSalt = "Salt";

        readonly string dbName = "LoginAuthRepositoryUnitTest";

        TaskSchedulerContext _context;

        static Mock<IPasswordHasher> GetPasswordHasherMock()
        {
            Mock<IPasswordHasher> passwordHasher = new Mock<IPasswordHasher>();
            passwordHasher.Setup(hasher => hasher.HashPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((salt, password) => salt + password);

            return passwordHasher;
        }

        static Mock<ISaltGenerator> GetSaltGeneratorMock()
        {
            Mock<ISaltGenerator> saltGenerator = new Mock<ISaltGenerator>();
            saltGenerator.Setup(generator => generator.GenerateSalt()).Returns(defaultSalt);

            return saltGenerator;
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            databaseAndTestDataSplitter = GetLoginAuthRegisterModelWrongConfirmPassword().Count() / 2;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder().UseInMemoryDatabase(dbName);
            _context = new TaskSchedulerContext(options.Options);

            foreach (var authModelArg in GetDatabaseLoginAuthModel())
            {
                var authModel = (LoginAuthRegisterModel)authModelArg[0];

                var auth = new LoginAuth(authModel.Login, authModel.Password);

                auth.SetSalt(defaultSalt);
                auth.SetPassword(defaultSalt + auth.Password);

                var user = new User(auth);

                _context.Users.Add(user);
                _context.LoginAuths.Add(auth);
            }

            _context.SaveChanges();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.EnsureDeleted();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetTestLoginAuthModel), DynamicDataSourceType.Method)]
        public async Task CreateUserAsync_FreeLogin_Success(LoginAuthRegisterModel loginAuthModel)
        {
            //arrange 
            var loginAuth = new LoginAuth(loginAuthModel.Login, loginAuthModel.Password);

            var expectedLogin = loginAuth.Login;
            var expectedPassword = defaultSalt + loginAuth.Password;
            var expectedSalt = defaultSalt;

            var saltGeneratorMock = GetSaltGeneratorMock();
            var passwordHasherMock = GetPasswordHasherMock();

            ILoginAuthRepository authRepository = new LoginAuthRepository(_context, saltGeneratorMock.Object, passwordHasherMock.Object);

            //act
            var actual = await authRepository.CreateUserAsync(loginAuth);

            //assert 
            Assert.IsTrue(actual.IsSuccess, $"User \"{loginAuth.Login}\" shouldn't be in batabase before creation so IsSuccess should be true");

            var actualAuth = actual.SuccessResult;

            Assert.AreEqual(expectedLogin, actualAuth.Login, $"Expected LoginAuth login {expectedLogin}; actual is {actualAuth.Login}");
            Assert.AreEqual(expectedPassword, actualAuth.Password, $"Expected LoginAuth login {expectedPassword}; actual is {actualAuth.Password}");
            Assert.AreEqual(expectedSalt, actualAuth.Salt, $"Expected LoginAuth login {expectedSalt}; actual is {actualAuth.Salt}");

            saltGeneratorMock.Verify(generator => generator.GenerateSalt(), "Successful creation require salt generation");
            passwordHasherMock.Verify(hasher => hasher.HashPassword(defaultSalt, loginAuthModel.Password),
                "Successful creation require password hashing");

            saltGeneratorMock.VerifyNoOtherCalls();
            passwordHasherMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetDatabaseLoginAuthModel), DynamicDataSourceType.Method)]
        public async Task CreateUserAsync_ExistedLogin_BadLogin(LoginAuthRegisterModel loginAuthModel)
        {
            //arrange 
            var loginAuth = new LoginAuth(loginAuthModel.Login, loginAuthModel.Password);

            var expectedResultCode = ResultFailCode.BadLogin;

            var saltGeneratorMock = GetSaltGeneratorMock();
            var passwordHasherMock = GetPasswordHasherMock();

            ILoginAuthRepository authRepository = new LoginAuthRepository(_context, saltGeneratorMock.Object, passwordHasherMock.Object);

            //act
            var actual = await authRepository.CreateUserAsync(loginAuth);

            //assert 
            Assert.IsTrue(actual.IsFail, $"User \"{loginAuth.Login}\" should be in batabase before creation so IsFail should be true");
            Assert.AreEqual(expectedResultCode, actual.FailCode, $"Expected result {expectedResultCode}; actual is {actual.FailCode}");
            
            saltGeneratorMock.VerifyNoOtherCalls();
            passwordHasherMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetDatabaseLoginAuthModel), DynamicDataSourceType.Method)]
        public async Task SignInUserAsync_ExistedUser_Success(LoginAuthRegisterModel loginAuthModel)
        {
            //arrange 
            var loginAuth = new LoginAuth(loginAuthModel.Login, loginAuthModel.Password);

            var expectedLogin = loginAuth.Login;
            var expectedPassword = defaultSalt + loginAuth.Password;
            var expectedSalt = defaultSalt;

            var saltGeneratorMock = GetSaltGeneratorMock();
            var passwordHasherMock = GetPasswordHasherMock();

            ILoginAuthRepository authRepository = new LoginAuthRepository(_context, saltGeneratorMock.Object, passwordHasherMock.Object);

            //act
            var actual = await authRepository.SignInUserAsync(loginAuth);

            //assert 
            Assert.IsTrue(actual.IsSuccess, $"User \"{loginAuth.Login}\" should be in batabase before sign in so IsSuccess should be true");

            var actualAuth = actual.SuccessResult;

            Assert.AreEqual(expectedLogin, actualAuth.Login, $"Expected LoginAuth login {expectedLogin}; actual is {actualAuth.Login}");
            Assert.AreEqual(expectedPassword, actualAuth.Password, $"Expected LoginAuth login {expectedPassword}; actual is {actualAuth.Password}");
            Assert.AreEqual(expectedSalt, actualAuth.Salt, $"Expected LoginAuth login {expectedSalt}; actual is {actualAuth.Salt}");

            passwordHasherMock.Verify(hasher => hasher.HashPassword(defaultSalt, loginAuthModel.Password),
                "Password comparison require password hashing");

            saltGeneratorMock.VerifyNoOtherCalls();
            passwordHasherMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetDatabaseLoginAuthModel), DynamicDataSourceType.Method)]
        public async Task SignInUserAsync_WrongPassword_BadLoginOrPasswordCode(LoginAuthRegisterModel loginAuthModel)
        {
            //arrange 
            var loginAuth = new LoginAuth(loginAuthModel.Login, loginAuthModel.ConfirmPassword);

            var expectedResultCode = ResultFailCode.BadLoginOrPassword;

            var saltGeneratorMock = GetSaltGeneratorMock();
            var passwordHasherMock = GetPasswordHasherMock();

            ILoginAuthRepository authRepository = new LoginAuthRepository(_context, saltGeneratorMock.Object, passwordHasherMock.Object);

            //act
            var actual = await authRepository.SignInUserAsync(loginAuth);

            //assert 
            Assert.IsTrue(actual.IsFail, $"User \"{loginAuth.Login}\" has worng password \"{loginAuth.Password}\" so IsFail should be true");
            Assert.AreEqual(expectedResultCode, actual.FailCode, $"Expected result {expectedResultCode}; actual is {actual.FailCode}");

            passwordHasherMock.Verify(hasher => hasher.HashPassword(defaultSalt, loginAuthModel.ConfirmPassword),
                "Password comparison require password hashing");

            saltGeneratorMock.VerifyNoOtherCalls();
            passwordHasherMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetTestLoginAuthModel), DynamicDataSourceType.Method)]
        public async Task SignInUserAsync_WrongLogin_BadLoginOrPasswordCode(LoginAuthRegisterModel loginAuthModel)
        {
            //arrange 
            var loginAuth = new LoginAuth(loginAuthModel.Login, loginAuthModel.Password);

            var expectedResultCode = ResultFailCode.BadLoginOrPassword;

            var saltGeneratorMock = GetSaltGeneratorMock();
            var passwordHasherMock = GetPasswordHasherMock();

            ILoginAuthRepository authRepository = new LoginAuthRepository(_context, saltGeneratorMock.Object, passwordHasherMock.Object);

            //act
            var actual = await authRepository.SignInUserAsync(loginAuth);

            //assert 
            Assert.IsTrue(actual.IsFail, $"User \"{loginAuth.Login}\" shouldn't be in batabase before sign in so IsFail should be true");
            Assert.AreEqual(expectedResultCode, actual.FailCode, $"Expected result {expectedResultCode}; actual is {actual.FailCode}");

            saltGeneratorMock.VerifyNoOtherCalls();
            passwordHasherMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetDatabaseLoginAuthModel), DynamicDataSourceType.Method)]
        public async Task IsLoginExistsAsync_LoginExists_TrueExpected(LoginAuthRegisterModel loginAuthModel)
        {
            //arrange 
            var login = loginAuthModel.Login;

            var saltGeneratorMock = GetSaltGeneratorMock();
            var passwordHasherMock = GetPasswordHasherMock();

            ILoginAuthRepository authRepository = new LoginAuthRepository(_context, saltGeneratorMock.Object, passwordHasherMock.Object);

            //act
            var actual = await authRepository.IsLoginExistsAsync(login);

            //assert 
            Assert.IsTrue(actual, $"User with login \"{login}\" exists in database, so should be returned true");

            saltGeneratorMock.VerifyNoOtherCalls();
            passwordHasherMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetTestLoginAuthModel), DynamicDataSourceType.Method)]
        public async Task IsLoginExistsAsync_LoginNotExists_FalseExpected(LoginAuthRegisterModel loginAuthModel)
        {
            //arrange 
            var login = loginAuthModel.Login;

            var saltGeneratorMock = GetSaltGeneratorMock();
            var passwordHasherMock = GetPasswordHasherMock();

            ILoginAuthRepository authRepository = new LoginAuthRepository(_context, saltGeneratorMock.Object, passwordHasherMock.Object);

            //act
            var actual = await authRepository.IsLoginExistsAsync(login);

            //assert 
            Assert.IsFalse(actual, $"User with login \"{login}\" don't exists in database, so should be returned false");

            saltGeneratorMock.VerifyNoOtherCalls();
            passwordHasherMock.VerifyNoOtherCalls();
        }

        [DeploymentItem(CommonTests.LOGIN_AUTH_REGISTER_FILENAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<object[]> GetLoginAuthRegisterModelWrongConfirmPassword() =>
            CommonTests.JsonFileToMethodArgument<LoginAuthRegisterModel>(CommonTests.LOGIN_AUTH_REGISTER_FILENAME);

        static IEnumerable<object[]> GetDatabaseLoginAuthModel() =>
            GetLoginAuthRegisterModelWrongConfirmPassword().Take(databaseAndTestDataSplitter);

        static IEnumerable<object[]> GetTestLoginAuthModel() =>
            GetLoginAuthRegisterModelWrongConfirmPassword().Skip(databaseAndTestDataSplitter);
    }
}