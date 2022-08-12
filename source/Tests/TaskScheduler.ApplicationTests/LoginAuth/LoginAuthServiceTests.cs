using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskScheduler.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using TaskScheduler.Model;
using TaskScheduler.Domain;

namespace TaskScheduler.Application.Tests
{
    [TestClass()]
    public class LoginAuthServiceTests
    {
        static Mock<ILoginAuthRepository> GetLoginAuthRepositoryMock()
        {
            var mock = new Mock<ILoginAuthRepository>();
            mock.Setup(repo => repo.CreateUserAsync(It.IsAny<LoginAuth>()))
                .Returns<LoginAuth>(async auth => new Result<LoginAuth> { SuccessResult = auth });

            mock.Setup(repo => repo.SignInUserAsync(It.IsAny<LoginAuth>()))
                .Returns<LoginAuth>(async auth => new Result<LoginAuth> { SuccessResult = auth });

            mock.Setup(repo => repo.IsLoginExistsAsync(It.IsAny<string>()))
                .Returns(async () => true);

            return mock;
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetRegisterValidData), DynamicDataSourceType.Method)]
        public async Task RegisterAsync_Valid_Success(LoginAuthRegisterModel regiserModel)
        {
            //arrange 
            var loginAuthRepoMock = GetLoginAuthRepositoryMock();

            ILoginAuthService authSerivce = new LoginAuthService(loginAuthRepoMock.Object);

            //act
            var actual = await authSerivce.RegisterAsync(regiserModel);

            //assert 
            Assert.IsTrue(actual.IsSuccess, $"Data is valid but was returned fail with Fail Code {actual.FailCode}");

            loginAuthRepoMock.Verify(repo => repo.CreateUserAsync(It.IsAny<LoginAuth>()),
                "Valid register data require creating it with ILoginAuthRepository");

            loginAuthRepoMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetRegisterInValidLoginData), DynamicDataSourceType.Method)]
        public async Task RegisterAsync_BadLogin_BadLoginFailCode(LoginAuthRegisterModel regiserModel)
        {
            //arrange 
            var expectedResultCode = ResultFailCode.BadLogin;

            var loginAuthRepoMock = GetLoginAuthRepositoryMock();

            ILoginAuthService authSerivce = new LoginAuthService(loginAuthRepoMock.Object);

            //act
            var actual = await authSerivce.RegisterAsync(regiserModel);

            //assert 
            Assert.IsTrue(actual.IsFail, $"User login \"{regiserModel.Login}\" should be invalid so result should be fail");
            Assert.AreEqual(expectedResultCode, actual.FailCode, $"Expected result {expectedResultCode}; actual is {actual.FailCode}");

            loginAuthRepoMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetRegisterInValidLoginAndPasswordData), DynamicDataSourceType.Method)]
        public async Task RegisterAsync_BadLoginAndPassword_BadLoginAndPasswordFailCode(LoginAuthRegisterModel regiserModel)
        {
            //arrange 
            var expectedResultCode = ResultFailCode.BadLogin | ResultFailCode.BadPassword;

            var loginAuthRepoMock = GetLoginAuthRepositoryMock();

            ILoginAuthService authSerivce = new LoginAuthService(loginAuthRepoMock.Object);

            //act
            var actual = await authSerivce.RegisterAsync(regiserModel);

            //assert 
            Assert.IsTrue(actual.IsFail, $"User login \"{regiserModel.Login}\" and password \"{regiserModel.Password}\" should be invalid so result should be fail");
            Assert.AreEqual(expectedResultCode, actual.FailCode, $"Expected result {expectedResultCode}; actual is {actual.FailCode}");

            loginAuthRepoMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetRegisterInValidConfirmPasswordData), DynamicDataSourceType.Method)]
        public async Task RegisterAsync_BadConfirmPassword_BadConfirmPasswordFailCode(LoginAuthRegisterModel regiserModel)
        {
            //arrange 
            var expectedResultCode = ResultFailCode.BadConfirmPassword;

            var loginAuthRepoMock = GetLoginAuthRepositoryMock();

            ILoginAuthService authSerivce = new LoginAuthService(loginAuthRepoMock.Object);

            //act
            var actual = await authSerivce.RegisterAsync(regiserModel);

            //assert 
            Assert.IsTrue(actual.IsFail, 
                $"User confimr password \"{regiserModel.ConfirmPassword}\" should be invalid (user password is \"{regiserModel.Password}\") so result should be fail");
            Assert.AreEqual(expectedResultCode, actual.FailCode, $"Expected result {expectedResultCode}; actual is {actual.FailCode}");

            loginAuthRepoMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetSigninValidData), DynamicDataSourceType.Method)]
        public async Task SingInAsync_Valid_Success(LoginAuthSignInModel singInModel)
        {
            //arrange 
            var loginAuthRepoMock = GetLoginAuthRepositoryMock();

            ILoginAuthService authSerivce = new LoginAuthService(loginAuthRepoMock.Object);

            //act
            var actual = await authSerivce.SingInAsync(singInModel);

            //assert 
            Assert.IsTrue(actual.IsSuccess, $"Data is valid but was returned fail with Fail Code {actual.FailCode}");

            loginAuthRepoMock.Verify(repo => repo.SignInUserAsync(It.IsAny<LoginAuth>()),
                "Valid register data require creating it with ILoginAuthRepository");

            loginAuthRepoMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetSigninInValidLoginData), DynamicDataSourceType.Method)]
        public async Task SingInAsync_BadLogin_BadLoginOrPasswordFailCode(LoginAuthSignInModel signInModel)
        {
            //arrange 
            var expectedResultCode = ResultFailCode.BadLoginOrPassword;

            var loginAuthRepoMock = GetLoginAuthRepositoryMock();

            ILoginAuthService authSerivce = new LoginAuthService(loginAuthRepoMock.Object);

            //act
            var actual = await authSerivce.SingInAsync(signInModel);

            //assert 
            Assert.IsTrue(actual.IsFail, $"User login \"{signInModel.Login}\" should be invalid so result should be fail");
            Assert.AreEqual(expectedResultCode, actual.FailCode, $"Expected result {expectedResultCode}; actual is {actual.FailCode}");

            loginAuthRepoMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetSigninInValidPasswordData), DynamicDataSourceType.Method)]
        public async Task SingInAsync_BadPassword_BadLoginOrPasswordFailCode(LoginAuthSignInModel signInModel)
        {
            //arrange 
            var expectedResultCode = ResultFailCode.BadLoginOrPassword;

            var loginAuthRepoMock = GetLoginAuthRepositoryMock();

            ILoginAuthService authSerivce = new LoginAuthService(loginAuthRepoMock.Object);

            //act
            var actual = await authSerivce.SingInAsync(signInModel);

            //assert 
            Assert.IsTrue(actual.IsFail, $"User password \"{signInModel.Password}\" should be invalid so result should be fail");
            Assert.AreEqual(expectedResultCode, actual.FailCode, $"Expected result {expectedResultCode}; actual is {actual.FailCode}");

            loginAuthRepoMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetSigninInValidLoginAndPasswordData), DynamicDataSourceType.Method)]
        public async Task SingInAsync_BadLoginAndPassword_BadLoginOrPasswordFailCode(LoginAuthSignInModel signInModel)
        {
            //arrange 
            var expectedResultCode = ResultFailCode.BadLoginOrPassword;

            var loginAuthRepoMock = GetLoginAuthRepositoryMock();

            ILoginAuthService authSerivce = new LoginAuthService(loginAuthRepoMock.Object);

            //act
            var actual = await authSerivce.SingInAsync(signInModel);

            //assert 
            Assert.IsTrue(actual.IsFail,
                $"User password \"{signInModel.Password}\" and login \"{signInModel.Login}\" should be invalid so result should be fail");
            Assert.AreEqual(expectedResultCode, actual.FailCode, $"Expected result {expectedResultCode}; actual is {actual.FailCode}");

            loginAuthRepoMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetLoginValidData), DynamicDataSourceType.Method)]
        public async Task IsLoginExistsAsync_ValidLogin_RepositoryResultExpected(LoginModel login)
        {
            //arrange 
            var loginAuthRepoMock = GetLoginAuthRepositoryMock();

            ILoginAuthService authSerivce = new LoginAuthService(loginAuthRepoMock.Object);

            //act
            var actual = await authSerivce.IsLoginExistsAsync(login);

            //assert 
            Assert.IsTrue(actual.SuccessResult, $"User login \"{login.Login}\" should be valid so result should be true");
            Assert.IsTrue(actual.IsSuccess, $"User login \"{login.Login}\" should be valid so result should be true");

            loginAuthRepoMock.Verify(repo => repo.IsLoginExistsAsync(login.Login));

            loginAuthRepoMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetLoginInValidData), DynamicDataSourceType.Method)]
        public async Task IsLoginExistsAsync_ValidLogin_TrueReturned(LoginModel login)
        {
            //arrange 
            var expectedResultCode = ResultFailCode.BadLogin;

            var loginAuthRepoMock = GetLoginAuthRepositoryMock();

            ILoginAuthService authSerivce = new LoginAuthService(loginAuthRepoMock.Object);

            //act
            var actual = await authSerivce.IsLoginExistsAsync(login);

            //assert 
            Assert.IsFalse(actual.SuccessResult, $"User login \"{login.Login}\" should be invalid so result should be false");

            loginAuthRepoMock.VerifyNoOtherCalls();
        }

        [DeploymentItem(CommonTests.LOGIN_AUTH_REGISTER_INVALID_CONFIRM_PASSWORD_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<object[]> GetRegisterInValidConfirmPasswordData() =>
            CommonTests.JsonFileToMethodArgument<LoginAuthRegisterModel>(CommonTests.LOGIN_AUTH_REGISTER_INVALID_CONFIRM_PASSWORD_FNAME);

        [DeploymentItem(CommonTests.LOGIN_AUTH_REGISTER_INVALID_LOGIN_AND_PASSWORD_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<object[]> GetRegisterInValidLoginAndPasswordData() =>
            CommonTests.JsonFileToMethodArgument<LoginAuthRegisterModel>(CommonTests.LOGIN_AUTH_REGISTER_INVALID_LOGIN_AND_PASSWORD_FNAME);

        [DeploymentItem(CommonTests.LOGIN_AUTH_REGISTER_INVALID_LOGIN_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<object[]> GetRegisterInValidLoginData() =>
            CommonTests.JsonFileToMethodArgument<LoginAuthRegisterModel>(CommonTests.LOGIN_AUTH_REGISTER_INVALID_LOGIN_FNAME);

        [DeploymentItem(CommonTests.LOGIN_AUTH_REGISTER_INVALID_PASSWORD_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<object[]> GetRegisterInValidPasswordData() =>
            CommonTests.JsonFileToMethodArgument<LoginAuthRegisterModel>(CommonTests.LOGIN_AUTH_REGISTER_INVALID_PASSWORD_FNAME);

        [DeploymentItem(CommonTests.LOGIN_AUTH_REGISTER_VALID__FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<object[]> GetRegisterValidData() =>
            CommonTests.JsonFileToMethodArgument<LoginAuthRegisterModel>(CommonTests.LOGIN_AUTH_REGISTER_VALID__FNAME);

        [DeploymentItem(CommonTests.LOGIN_AUTH_SIGNIN_INVALID_LOGIN_AND_PASSWORD_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<object[]> GetSigninInValidLoginAndPasswordData() =>
            CommonTests.JsonFileToMethodArgument<LoginAuthSignInModel>(CommonTests.LOGIN_AUTH_SIGNIN_INVALID_LOGIN_AND_PASSWORD_FNAME);

        [DeploymentItem(CommonTests.LOGIN_AUTH_SIGNIN_INVALID_LOGIN_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<object[]> GetSigninInValidLoginData() =>
            CommonTests.JsonFileToMethodArgument<LoginAuthSignInModel>(CommonTests.LOGIN_AUTH_SIGNIN_INVALID_LOGIN_FNAME);

        [DeploymentItem(CommonTests.LOGIN_AUTH_SIGNIN_INVALID_PASSWORD_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<object[]> GetSigninInValidPasswordData() =>
            CommonTests.JsonFileToMethodArgument<LoginAuthSignInModel>(CommonTests.LOGIN_AUTH_SIGNIN_INVALID_PASSWORD_FNAME);

        [DeploymentItem(CommonTests.LOGIN_AUTH_SIGNIN_VALID_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<object[]> GetSigninValidData() =>
            CommonTests.JsonFileToMethodArgument<LoginAuthSignInModel>(CommonTests.LOGIN_AUTH_SIGNIN_VALID_FNAME);

        [DeploymentItem(CommonTests.LOGIN_AUTH_SIGNIN_VALID_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<object[]> GetLoginValidData() =>
            CommonTests.DeserializeJson<LoginAuthSignInModel>(CommonTests.LOGIN_AUTH_SIGNIN_VALID_FNAME)
                .Select(signIn => new object[] { new LoginModel(signIn.Login) });

        [DeploymentItem(CommonTests.LOGIN_AUTH_SIGNIN_INVALID_LOGIN_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<object[]> GetLoginInValidData() =>
            CommonTests.DeserializeJson<LoginAuthSignInModel>(CommonTests.LOGIN_AUTH_SIGNIN_INVALID_LOGIN_FNAME)
                .Select(signIn => new object[] { new LoginModel(signIn.Login) });
    }
}