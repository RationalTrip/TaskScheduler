using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using TaskScheduler.Domain;
using TaskScheduler.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TaskScheduler.Application.Tests
{
    [TestClass()]
    public class UserServiceTests
    {

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetValidLoginArgs), DynamicDataSourceType.Method)]
        public async Task GetParticipatedScheduleTasksAsync_ValidLogin_Success(string login)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var expectedResult = new Result<IEnumerable<ScheduleTask>>();

            var taskConvertorMock = new Mock<IScheduleTaskToIndividualTaskConvertor>();

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(mock => mock.GetParticipatedTasksAsync(login))
                .Returns(async () => expectedResult);

            var loggerMock = new Mock<ILogger<UserService>>();

            IUserService userSerivce = new UserService(userRepositoryMock.Object, taskConvertorMock.Object, loggerMock.Object);

            //act
            var actual = await userSerivce.GetParticipatedScheduleTasksAsync(loginModel);

            //assert 
            Assert.IsTrue(actual.IsSuccess, $"login \"{login}\" is valid but was returned fail with Fail Code {actual.FailCode}");
            Assert.AreEqual(expectedResult, actual);

            userRepositoryMock.Verify(mock => mock.GetParticipatedTasksAsync(login),
                "Valid getting participated tasks require call to IUserRepository");

            taskConvertorMock.VerifyNoOtherCalls();
            userRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetInValidLoginArgs), DynamicDataSourceType.Method)]
        public async Task GetParticipatedScheduleTasksAsync_InValidLogin_BadAuthCookieFailCode(string login)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var expectedFailCode = ResultFailCode.BadAuthCookie;

            var taskConvertorMock = new Mock<IScheduleTaskToIndividualTaskConvertor>();

            var userRepositoryMock = new Mock<IUserRepository>();

            var loggerMock = new Mock<ILogger<UserService>>();

            IUserService userSerivce = new UserService(userRepositoryMock.Object, taskConvertorMock.Object, loggerMock.Object);

            //act
            var actual = await userSerivce.GetParticipatedScheduleTasksAsync(loginModel);

            //assert 
            Assert.IsTrue(actual.IsFail, $"login \"{login}\" is not valid but was returned success");
            Assert.AreEqual(expectedFailCode, actual.FailCode);

            taskConvertorMock.VerifyNoOtherCalls();
            userRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetValidLoginArgs), DynamicDataSourceType.Method)]
        public async Task GetOwnedScheduleTasksAsync_ValidLogin_Success(string login)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var expectedResult = new Result<IEnumerable<ScheduleTask>>();

            var taskConvertorMock = new Mock<IScheduleTaskToIndividualTaskConvertor>();

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(mock => mock.GetOwnedTasksAsync(login))
                .ReturnsAsync(expectedResult);

            var loggerMock = new Mock<ILogger<UserService>>();

            IUserService userSerivce = new UserService(userRepositoryMock.Object, taskConvertorMock.Object, loggerMock.Object);

            //act
            var actual = await userSerivce.GetOwnedScheduleTasksAsync(loginModel);

            //assert 
            Assert.IsTrue(actual.IsSuccess, $"login \"{login}\" is valid but was returned fail with Fail Code {actual.FailCode}");
            Assert.AreEqual(expectedResult, actual);

            userRepositoryMock.Verify(mock => mock.GetOwnedTasksAsync(login),
                "Valid register data require creating it with ILoginAuthRepository");

            taskConvertorMock.VerifyNoOtherCalls();
            userRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetInValidLoginArgs), DynamicDataSourceType.Method)]
        public async Task GetOwnedScheduleTasksAsync_InValidLogin_BadAuthCookieFailCode(string login)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var expectedFailCode = ResultFailCode.BadAuthCookie;

            var taskConvertorMock = new Mock<IScheduleTaskToIndividualTaskConvertor>();

            var userRepositoryMock = new Mock<IUserRepository>();

            var loggerMock = new Mock<ILogger<UserService>>();

            IUserService userSerivce = new UserService(userRepositoryMock.Object, taskConvertorMock.Object, loggerMock.Object);

            //act
            var actual = await userSerivce.GetOwnedScheduleTasksAsync(loginModel);

            //assert 
            Assert.IsTrue(actual.IsFail, $"login \"{login}\" is not valid but was returned success");
            Assert.AreEqual(expectedFailCode, actual.FailCode);

            taskConvertorMock.VerifyNoOtherCalls();
            userRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetValidLoginAndDaysArgs), DynamicDataSourceType.Method)]
        public async Task GetParticipatedScheduleTasksInDayAsync_Valid_Success(string login, int year, int month, int day)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var dbResult = new Result<IEnumerable<ScheduleTask>>();
            var expectedResult = new IndividualTask[0];

            var expectedDateStart = new DateTime(year, month, day);
            var expectedDateEnd = new DateTime(year, month, day).AddDays(1);

            var taskConvertorMock = new Mock<IScheduleTaskToIndividualTaskConvertor>();
            taskConvertorMock.Setup(mock => mock.ToIndividualTasks(dbResult.SuccessResult, expectedDateStart, expectedDateEnd))
                .Returns(expectedResult);

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(mock => mock.GetParticipatedScheduleTasksWithinTimeRangeAsync(login, expectedDateStart, expectedDateEnd))
                .Returns(async () => dbResult);

            var loggerMock = new Mock<ILogger<UserService>>();

            IUserService userSerivce = new UserService(userRepositoryMock.Object, taskConvertorMock.Object, loggerMock.Object);

            //act
            var actual = await userSerivce.GetParticipatedScheduleTasksInDayAsync(loginModel, year, month, day);

            //assert 
            Assert.IsTrue(actual.IsSuccess, $"login \"{login}\" is valid but was returned fail with Fail Code {actual.FailCode}");
            Assert.AreEqual(expectedResult, actual.SuccessResult);

            taskConvertorMock.Verify(mock => mock.ToIndividualTasks(dbResult.SuccessResult, expectedDateStart, expectedDateEnd),
                "Valid result require converting schedule tasks to individual");

            userRepositoryMock.Verify(mock => mock.GetParticipatedScheduleTasksWithinTimeRangeAsync(login, expectedDateStart, expectedDateEnd),
                "Valid getting participated tasks require call to IUserRepository");

            taskConvertorMock.VerifyNoOtherCalls();
            userRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetInValidLoginAndValidvDaysArgs), DynamicDataSourceType.Method)]
        public async Task GetParticipatedScheduleTasksInDayAsync_InvalidLoginAndValidDate_BadAuthCookieFailCode(string login, int year, int month, int day)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var expectedFailCode = ResultFailCode.BadAuthCookie;
            var expectedStatusCode = StatusCodes.Status401Unauthorized;

            var taskConvertorMock = new Mock<IScheduleTaskToIndividualTaskConvertor>();

            var userRepositoryMock = new Mock<IUserRepository>();

            var loggerMock = new Mock<ILogger<UserService>>();

            IUserService userSerivce = new UserService(userRepositoryMock.Object, taskConvertorMock.Object, loggerMock.Object);

            //act
            var actual = await userSerivce.GetParticipatedScheduleTasksInDayAsync(loginModel, year, month, day);

            //assert 
            Assert.IsTrue(actual.IsFail, $"login \"{login}\" is not valid but was returned success");
            Assert.AreEqual(expectedFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);

            taskConvertorMock.VerifyNoOtherCalls();
            userRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetValidLoginAndInValidDaysArgs), DynamicDataSourceType.Method)]
        [DynamicData(nameof(GetInValidLoginAndInValidDaysArgs), DynamicDataSourceType.Method)]
        public async Task GetParticipatedScheduleTasksInDayAsync_InvalidDate_BadSelectedDateFailCode(string login, int year, int month, int day)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var expectedFailCode = ResultFailCode.BadSelectedDate;
            var expectedStatusCode = StatusCodes.Status404NotFound;

            var taskConvertorMock = new Mock<IScheduleTaskToIndividualTaskConvertor>();

            var userRepositoryMock = new Mock<IUserRepository>();

            var loggerMock = new Mock<ILogger<UserService>>();

            IUserService userSerivce = new UserService(userRepositoryMock.Object, taskConvertorMock.Object, loggerMock.Object);

            //act
            var actual = await userSerivce.GetParticipatedScheduleTasksInDayAsync(loginModel, year, month, day);

            //assert 
            Assert.IsTrue(actual.IsFail, $"date \"{day}.{month}.{year}\" is not valid but was returned success");
            Assert.AreEqual(expectedFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);

            taskConvertorMock.VerifyNoOtherCalls();
            userRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetValidLoginAndMonthsArgs), DynamicDataSourceType.Method)]
        public async Task GetParticipatedScheduleTasksInMonthAsync_Valid_Success(string login, int year, int month)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var dbResult = new Result<IEnumerable<ScheduleTask>>();
            var expectedResult = new IndividualTask[0];

            var expectedDateStart = new DateTime(year, month, 1);
            var expectedDateEnd = new DateTime(year, month, 1).AddMonths(1);

            var taskConvertorMock = new Mock<IScheduleTaskToIndividualTaskConvertor>();
            taskConvertorMock.Setup(mock => mock.ToIndividualTasks(dbResult.SuccessResult, expectedDateStart, expectedDateEnd))
                .Returns(expectedResult);

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(mock => mock.GetParticipatedScheduleTasksWithinTimeRangeAsync(login, expectedDateStart, expectedDateEnd))
                .Returns(async () => dbResult);

            var loggerMock = new Mock<ILogger<UserService>>();

            IUserService userSerivce = new UserService(userRepositoryMock.Object, taskConvertorMock.Object, loggerMock.Object);

            //act
            var actual = await userSerivce.GetParticipatedScheduleTasksInMonthAsync(loginModel, year, month);

            //assert 
            Assert.IsTrue(actual.IsSuccess, $"login \"{login}\" is valid but was returned fail with Fail Code {actual.FailCode}");
            Assert.AreEqual(expectedResult, actual.SuccessResult);

            taskConvertorMock.Verify(mock => mock.ToIndividualTasks(dbResult.SuccessResult, expectedDateStart, expectedDateEnd),
                "Valid result require converting schedule tasks to individual");

            userRepositoryMock.Verify(mock => mock.GetParticipatedScheduleTasksWithinTimeRangeAsync(login, expectedDateStart, expectedDateEnd),
                "Valid getting participated tasks require call to IUserRepository");

            taskConvertorMock.VerifyNoOtherCalls();
            userRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetInValidLoginAndValidMonthsArgs), DynamicDataSourceType.Method)]
        public async Task GetParticipatedScheduleTasksInMonthAsync_InvalidLoginValidDate_BadAuthCookieFailCode(string login, int year, int month)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var expectedFailCode = ResultFailCode.BadAuthCookie;
            var expectedStatusCode = StatusCodes.Status401Unauthorized;

            var taskConvertorMock = new Mock<IScheduleTaskToIndividualTaskConvertor>();

            var userRepositoryMock = new Mock<IUserRepository>();

            var loggerMock = new Mock<ILogger<UserService>>();

            IUserService userSerivce = new UserService(userRepositoryMock.Object, taskConvertorMock.Object, loggerMock.Object);

            //act
            var actual = await userSerivce.GetParticipatedScheduleTasksInMonthAsync(loginModel, year, month);

            //assert 
            Assert.IsTrue(actual.IsFail, $"login \"{login}\" is not valid but was returned success");
            Assert.AreEqual(expectedFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);

            taskConvertorMock.VerifyNoOtherCalls();
            userRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetValidLoginAndInValidMonthsArgs), DynamicDataSourceType.Method)]
        [DynamicData(nameof(GetInValidLoginAndInValidMonthsArgs), DynamicDataSourceType.Method)]
        public async Task GetParticipatedScheduleTasksInMonthAsync_InvalidDate_BadSelectedDateFailCode(string login, int year, int month)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var expectedFailCode = ResultFailCode.BadSelectedDate;
            var expectedStatusCode = StatusCodes.Status404NotFound;

            var taskConvertorMock = new Mock<IScheduleTaskToIndividualTaskConvertor>();

            var userRepositoryMock = new Mock<IUserRepository>();

            var loggerMock = new Mock<ILogger<UserService>>();

            IUserService userSerivce = new UserService(userRepositoryMock.Object, taskConvertorMock.Object, loggerMock.Object);

            //act
            var actual = await userSerivce.GetParticipatedScheduleTasksInMonthAsync(loginModel, year, month);

            //assert 
            Assert.IsTrue(actual.IsFail, $"date \"{month}.{year}\" is not valid but was returned success");
            Assert.AreEqual(expectedFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);

            taskConvertorMock.VerifyNoOtherCalls();
            userRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetValidLoginAndYearsArgs), DynamicDataSourceType.Method)]
        public async Task GetParticipatedScheduleTasksInYearAsync_Valid_Success(string login, int year)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var dbResult = new Result<IEnumerable<ScheduleTask>>();
            var expectedResult = new IndividualTask[0];

            var expectedDateStart = new DateTime(year, 1, 1);
            var expectedDateEnd = new DateTime(year, 1, 1).AddYears(1);

            var taskConvertorMock = new Mock<IScheduleTaskToIndividualTaskConvertor>();
            taskConvertorMock.Setup(mock => mock.ToIndividualTasks(dbResult.SuccessResult, expectedDateStart, expectedDateEnd))
                .Returns(expectedResult);

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(mock => mock.GetParticipatedScheduleTasksWithinTimeRangeAsync(login, expectedDateStart, expectedDateEnd))
                .Returns(async () => dbResult);

            var loggerMock = new Mock<ILogger<UserService>>();

            IUserService userSerivce = new UserService(userRepositoryMock.Object, taskConvertorMock.Object, loggerMock.Object);

            //act
            var actual = await userSerivce.GetParticipatedScheduleTasksInYearAsync(loginModel, year);

            //assert 
            Assert.IsTrue(actual.IsSuccess, $"login \"{login}\" is valid but was returned fail with Fail Code {actual.FailCode}");
            Assert.AreEqual(expectedResult, actual.SuccessResult);

            taskConvertorMock.Verify(mock => mock.ToIndividualTasks(dbResult.SuccessResult, expectedDateStart, expectedDateEnd),
                "Valid result require converting schedule tasks to individual");

            userRepositoryMock.Verify(mock => mock.GetParticipatedScheduleTasksWithinTimeRangeAsync(login, expectedDateStart, expectedDateEnd),
                "Valid getting participated tasks require call to IUserRepository");

            taskConvertorMock.VerifyNoOtherCalls();
            userRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetInValidLoginAndValidYearsArgs), DynamicDataSourceType.Method)]
        public async Task GetParticipatedScheduleTasksInYearAsync_InvalidLoginAndValidDate_BadAuthCookieFailCode(string login, int year)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var expectedFailCode = ResultFailCode.BadAuthCookie;
            var expectedStatusCode = StatusCodes.Status401Unauthorized;

            var taskConvertorMock = new Mock<IScheduleTaskToIndividualTaskConvertor>();

            var userRepositoryMock = new Mock<IUserRepository>();

            var loggerMock = new Mock<ILogger<UserService>>();

            IUserService userSerivce = new UserService(userRepositoryMock.Object, taskConvertorMock.Object, loggerMock.Object);

            //act
            var actual = await userSerivce.GetParticipatedScheduleTasksInYearAsync(loginModel, year);

            //assert 
            Assert.IsTrue(actual.IsFail, $"login \"{login}\" is not valid but was returned success");
            Assert.AreEqual(expectedFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);

            taskConvertorMock.VerifyNoOtherCalls();
            userRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetValidLoginAndInValidYearsArgs), DynamicDataSourceType.Method)]
        [DynamicData(nameof(GetInValidLoginAndInValidYearsArgs), DynamicDataSourceType.Method)]
        public async Task GetParticipatedScheduleTasksInYearAsync_InvalidDate_BadSelectedDateFailCode(string login, int year)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var expectedFailCode = ResultFailCode.BadSelectedDate;
            var expectedStatusCode = StatusCodes.Status404NotFound;

            var taskConvertorMock = new Mock<IScheduleTaskToIndividualTaskConvertor>();

            var userRepositoryMock = new Mock<IUserRepository>();

            var loggerMock = new Mock<ILogger<UserService>>();

            IUserService userSerivce = new UserService(userRepositoryMock.Object, taskConvertorMock.Object, loggerMock.Object);

            //act
            var actual = await userSerivce.GetParticipatedScheduleTasksInYearAsync(loginModel, year);

            //assert 
            Assert.IsTrue(actual.IsFail, $"year \"{year}\" is not valid but was returned success");
            Assert.AreEqual(expectedFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);

            taskConvertorMock.VerifyNoOtherCalls();
            userRepositoryMock.VerifyNoOtherCalls();
        }

        [DeploymentItem(CommonTests.LOGIN_AUTH_SIGNIN_VALID_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<string> GetValidLogin() =>
            CommonTests.DeserializeJson<LoginAuthSignInModel>(CommonTests.LOGIN_AUTH_SIGNIN_VALID_FNAME)
                .Select(auth => auth.Login);

        [DeploymentItem(CommonTests.LOGIN_AUTH_SIGNIN_INVALID_LOGIN_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<string> GetInValidLogin() =>
            CommonTests.DeserializeJson<LoginAuthSignInModel>(CommonTests.LOGIN_AUTH_SIGNIN_INVALID_LOGIN_FNAME)
                .Select(auth => auth.Login);

        static IEnumerable<object[]> GetValidLoginArgs() => GetValidLogin().Select(login => new object[] { login });

        static IEnumerable<object[]> GetInValidLoginArgs() => GetInValidLogin().Select(login => new object[] { login });

        static IEnumerable<object[]> GetValidLoginAndYearsArgs() => CommonTests.GetValidYear()
            .FullZip(GetValidLogin())
            .Select(arg => new object[] { arg.Item2, arg.Item1 });

        static IEnumerable<object[]> GetInValidLoginAndValidYearsArgs() => CommonTests.GetValidYear()
            .FullZip(GetInValidLogin())
            .Select(arg => new object[] { arg.Item2, arg.Item1 });
        static IEnumerable<object[]> GetValidLoginAndInValidYearsArgs() => CommonTests.GetInValidYear()
            .FullZip(GetValidLogin())
            .Select(arg => new object[] { arg.Item2, arg.Item1 });

        static IEnumerable<object[]> GetInValidLoginAndInValidYearsArgs() => CommonTests.GetInValidYear()
            .FullZip(GetInValidLogin())
            .Select(arg => new object[] { arg.Item2, arg.Item1 });

        static IEnumerable<object[]> GetValidLoginAndMonthsArgs() => CommonTests.GetValidMonth()
            .FullZip(GetValidLogin())
            .Select(arg => new object[] { arg.Item2, arg.Item1.year, arg.Item1.month });

        static IEnumerable<object[]> GetInValidLoginAndValidMonthsArgs() => CommonTests.GetValidMonth()
            .FullZip(GetInValidLogin())
            .Select(arg => new object[] { arg.Item2, arg.Item1.year, arg.Item1.month });
        static IEnumerable<object[]> GetValidLoginAndInValidMonthsArgs() => CommonTests.GetInValidMonth()
            .FullZip(GetValidLogin())
            .Select(arg => new object[] { arg.Item2, arg.Item1.year, arg.Item1.month });

        static IEnumerable<object[]> GetInValidLoginAndInValidMonthsArgs() => CommonTests.GetInValidMonth()
            .FullZip(GetInValidLogin())
            .Select(arg => new object[] { arg.Item2, arg.Item1.year, arg.Item1.month });

        static IEnumerable<object[]> GetValidLoginAndDaysArgs() => CommonTests.GetValidDay()
            .FullZip(GetValidLogin())
            .Select(arg => new object[] { arg.Item2, arg.Item1.year, arg.Item1.month, arg.Item1.day });

        static IEnumerable<object[]> GetInValidLoginAndValidvDaysArgs() => CommonTests.GetValidDay()
            .FullZip(GetInValidLogin())
            .Select(arg => new object[] { arg.Item2, arg.Item1.year, arg.Item1.month, arg.Item1.day });
        static IEnumerable<object[]> GetValidLoginAndInValidDaysArgs() => CommonTests.GetInValidDay()
            .FullZip(GetValidLogin())
            .Select(arg => new object[] { arg.Item2, arg.Item1.year, arg.Item1.month, arg.Item1.day });

        static IEnumerable<object[]> GetInValidLoginAndInValidDaysArgs() => CommonTests.GetInValidDay()
            .FullZip(GetInValidLogin())
            .Select(arg => new object[] { arg.Item2, arg.Item1.year, arg.Item1.month, arg.Item1.day });
    }
}