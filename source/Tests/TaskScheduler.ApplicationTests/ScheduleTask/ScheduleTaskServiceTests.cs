using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskScheduler.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler.Model;
using TaskScheduler.Domain;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TaskScheduler.Application.Tests
{
    [TestClass()]
    public class ScheduleTaskServiceTests
    {

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetValidScheduleTaskLinkArg), DynamicDataSourceType.Method)]
        public async Task GetScheduleTaskAsync_ValidLink_Success(string link)
        {
            //arrange 
            var expectedResult = new Result<ScheduleTask>();

            var scheduleTasRepositorykMock = new Mock<IScheduleTaskRepository>();
            scheduleTasRepositorykMock.Setup(mock => mock.GetScheduleTaskAsync(link))
                .Returns(async () => expectedResult);

            var loggerMock = new Mock<ILogger<ScheduleTaskService>>();

            IScheduleTaskService scheduleTaskService = new ScheduleTaskService(scheduleTasRepositorykMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskService.GetScheduleTaskAsync(link);

            //assert 
            Assert.IsTrue(actual.IsSuccess, $"link \"{link}\" is valid but was returned fail with Fail Code {actual.FailCode}");
            Assert.AreEqual(expectedResult, actual);

            scheduleTasRepositorykMock.Verify(mock => mock.GetScheduleTaskAsync(link),
                "Getting task require using IScheduleTaskService");

            scheduleTasRepositorykMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetInValidScheduleTaskLinkArg), DynamicDataSourceType.Method)]
        public async Task GetScheduleTaskAsync_InValidLink_BadScheduleTaskLinkFailCode(string link)
        {
            //arrange 
            var expectedResultFailCode = ResultFailCode.BadScheduleTaskLink;

            var scheduleTasRepositorykMock = new Mock<IScheduleTaskRepository>();

            var loggerMock = new Mock<ILogger<ScheduleTaskService>>();

            IScheduleTaskService scheduleTaskService = new ScheduleTaskService(scheduleTasRepositorykMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskService.GetScheduleTaskAsync(link);

            //assert 
            Assert.IsTrue(actual.IsFail, $"link \"{link}\" is invalid but was returned success");
            Assert.AreEqual(expectedResultFailCode, actual.FailCode);


            scheduleTasRepositorykMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetValidLoginAndValidScheduleTaskLinkArg), DynamicDataSourceType.Method)]
        public async Task DeleteScheduleTaskAsync_ValidData_Success(string login, string link)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var expectedResult = new Result<ScheduleTask>();

            var scheduleTasRepositorykMock = new Mock<IScheduleTaskRepository>();
            scheduleTasRepositorykMock.Setup(mock => mock.DeleteScheduleTaskAsync(login, link))
                .Returns(async () => expectedResult);

            var loggerMock = new Mock<ILogger<ScheduleTaskService>>();

            IScheduleTaskService scheduleTaskService = new ScheduleTaskService(scheduleTasRepositorykMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskService.DeleteScheduleTaskAsync(loginModel, link);

            //assert 
            Assert.IsTrue(actual.IsSuccess, $"link \"{link}\" is valid but was returned fail with Fail Code {actual.FailCode}");
            Assert.AreEqual(expectedResult, actual);

            scheduleTasRepositorykMock.Verify(mock => mock.DeleteScheduleTaskAsync(login, link),
                "Deleting task require using IScheduleTaskService");

            scheduleTasRepositorykMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetValidLoginAndInValidScheduleTaskLinkArg), DynamicDataSourceType.Method)]
        [DynamicData(nameof(GetInValidLoginAndInValidScheduleTaskLinkArg), DynamicDataSourceType.Method)]
        public async Task DeleteScheduleTaskAsync_InValidLink_BadScheduleTaskLinkFailCodeAnd404NotFound(string login, string link)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var expectedResultFailCode = ResultFailCode.BadScheduleTaskLink;
            var expectedStatusCode = StatusCodes.Status404NotFound;

            var scheduleTasRepositorykMock = new Mock<IScheduleTaskRepository>();

            var loggerMock = new Mock<ILogger<ScheduleTaskService>>();

            IScheduleTaskService scheduleTaskService = new ScheduleTaskService(scheduleTasRepositorykMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskService.DeleteScheduleTaskAsync(loginModel, link);

            //assert 
            Assert.IsTrue(actual.IsFail, $"link \"{link}\" is invalid but was returned success");
            Assert.AreEqual(expectedResultFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);

            scheduleTasRepositorykMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetInValidLoginAndValidScheduleTaskLinkArg), DynamicDataSourceType.Method)]
        public async Task DeleteScheduleTaskAsync_InValidLogin_BadAuthCookieFailCodeAnd401(string login, string link)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var expectedResultFailCode = ResultFailCode.BadAuthCookie;
            var expectedStatusCode = StatusCodes.Status401Unauthorized;

            var scheduleTasRepositorykMock = new Mock<IScheduleTaskRepository>();

            var loggerMock = new Mock<ILogger<ScheduleTaskService>>();

            IScheduleTaskService scheduleTaskService = new ScheduleTaskService(scheduleTasRepositorykMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskService.DeleteScheduleTaskAsync(loginModel, link);

            //assert 
            Assert.IsTrue(actual.IsFail, $"login \"{login}\" is invalid but was returned success");
            Assert.AreEqual(expectedResultFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);

            scheduleTasRepositorykMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetValidLoginAndValidScheduleTaskLinkArg), DynamicDataSourceType.Method)]
        public async Task ParticipateScheduleTaskAsync_ValidData_Success(string login, string link)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var expectedResult = new Result<ScheduleTask>();

            var scheduleTasRepositorykMock = new Mock<IScheduleTaskRepository>();
            scheduleTasRepositorykMock.Setup(mock => mock.ParticipateScheduleTaskAsync(login, link))
                .Returns(async () => expectedResult);

            var loggerMock = new Mock<ILogger<ScheduleTaskService>>();

            IScheduleTaskService scheduleTaskService = new ScheduleTaskService(scheduleTasRepositorykMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskService.ParticipateScheduleTaskAsync(loginModel, link);

            //assert 
            Assert.IsTrue(actual.IsSuccess, $"link \"{link}\" is valid but was returned fail with Fail Code {actual.FailCode}");
            Assert.AreEqual(expectedResult, actual);

            scheduleTasRepositorykMock.Verify(mock => mock.ParticipateScheduleTaskAsync(login, link),
                "Participating task require using IScheduleTaskService");

            scheduleTasRepositorykMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetValidLoginAndInValidScheduleTaskLinkArg), DynamicDataSourceType.Method)]
        [DynamicData(nameof(GetInValidLoginAndInValidScheduleTaskLinkArg), DynamicDataSourceType.Method)]
        public async Task ParticipateScheduleTaskAsync_InValidLink_BadScheduleTaskLinkFailCodeAnd404NotFound(string login, string link)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var expectedResultFailCode = ResultFailCode.BadScheduleTaskLink;
            var expectedStatusCode = StatusCodes.Status404NotFound;

            var scheduleTasRepositorykMock = new Mock<IScheduleTaskRepository>();

            var loggerMock = new Mock<ILogger<ScheduleTaskService>>();

            IScheduleTaskService scheduleTaskService = new ScheduleTaskService(scheduleTasRepositorykMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskService.ParticipateScheduleTaskAsync(loginModel, link);

            //assert 
            Assert.IsTrue(actual.IsFail, $"link \"{link}\" is invalid but was returned success");
            Assert.AreEqual(expectedResultFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);

            scheduleTasRepositorykMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetInValidLoginAndValidScheduleTaskLinkArg), DynamicDataSourceType.Method)]
        public async Task ParticipateScheduleTaskAsync_InValidLogin_BadAuthCookieFailCodeAnd401(string login, string link)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var expectedResultFailCode = ResultFailCode.BadAuthCookie;
            var expectedStatusCode = StatusCodes.Status401Unauthorized;

            var scheduleTasRepositorykMock = new Mock<IScheduleTaskRepository>();

            var loggerMock = new Mock<ILogger<ScheduleTaskService>>();

            IScheduleTaskService scheduleTaskService = new ScheduleTaskService(scheduleTasRepositorykMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskService.ParticipateScheduleTaskAsync(loginModel, link);

            //assert 
            Assert.IsTrue(actual.IsFail, $"login \"{login}\" is invalid but was returned success");
            Assert.AreEqual(expectedResultFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);

            scheduleTasRepositorykMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetValidLoginAndValidScheduleTaskLinkArg), DynamicDataSourceType.Method)]
        public async Task LeaveScheduleTaskAsync_ValidData_Success(string login, string link)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var expectedResult = new Result<ScheduleTask>();

            var scheduleTasRepositorykMock = new Mock<IScheduleTaskRepository>();
            scheduleTasRepositorykMock.Setup(mock => mock.LeaveScheduleTaskAsync(login, link))
                .Returns(async () => expectedResult);

            var loggerMock = new Mock<ILogger<ScheduleTaskService>>();

            IScheduleTaskService scheduleTaskService = new ScheduleTaskService(scheduleTasRepositorykMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskService.LeaveScheduleTaskAsync(loginModel, link);

            //assert 
            Assert.IsTrue(actual.IsSuccess, $"link \"{link}\" is valid but was returned fail with Fail Code {actual.FailCode}");
            Assert.AreEqual(expectedResult, actual);

            scheduleTasRepositorykMock.Verify(mock => mock.LeaveScheduleTaskAsync(login, link),
                "Leaving task require using IScheduleTaskService");

            scheduleTasRepositorykMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetValidLoginAndInValidScheduleTaskLinkArg), DynamicDataSourceType.Method)]
        [DynamicData(nameof(GetInValidLoginAndInValidScheduleTaskLinkArg), DynamicDataSourceType.Method)]
        public async Task LeaveScheduleTaskAsync_InValidLink_BadScheduleTaskLinkFailCodeAnd404NotFound(string login, string link)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var expectedResultFailCode = ResultFailCode.BadScheduleTaskLink;
            var expectedStatusCode = StatusCodes.Status404NotFound;

            var scheduleTasRepositorykMock = new Mock<IScheduleTaskRepository>();

            var loggerMock = new Mock<ILogger<ScheduleTaskService>>();

            IScheduleTaskService scheduleTaskService = new ScheduleTaskService(scheduleTasRepositorykMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskService.LeaveScheduleTaskAsync(loginModel, link);

            //assert 
            Assert.IsTrue(actual.IsFail, $"link \"{link}\" is invalid but was returned success");
            Assert.AreEqual(expectedResultFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);

            scheduleTasRepositorykMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetInValidLoginAndValidScheduleTaskLinkArg), DynamicDataSourceType.Method)]
        public async Task LeaveScheduleTaskAsync_InValidLogin_BadAuthCookieFailCodeAnd401(string login, string link)
        {
            //arrange 
            var loginModel = new LoginModel(login);

            var expectedResultFailCode = ResultFailCode.BadAuthCookie;
            var expectedStatusCode = StatusCodes.Status401Unauthorized;

            var scheduleTasRepositorykMock = new Mock<IScheduleTaskRepository>();

            var loggerMock = new Mock<ILogger<ScheduleTaskService>>();

            IScheduleTaskService scheduleTaskService = new ScheduleTaskService(scheduleTasRepositorykMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskService.LeaveScheduleTaskAsync(loginModel, link);

            //assert 
            Assert.IsTrue(actual.IsFail, $"login \"{login}\" is invalid but was returned success");
            Assert.AreEqual(expectedResultFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);

            scheduleTasRepositorykMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetValidScheduleTaskCreateModelAndValidOwnerLogin), DynamicDataSourceType.Method)]
        public async Task CreateScheduleTaskAsync_ValidData_Success(ScheduleTaskCreateModel createModel, string ownerLogin)
        {
            //arrange 
            var ownerLoginModel = new LoginModel(ownerLogin);

            var expectedResultModel = createModel.ToScheduleTask();

            var scheduleTasRepositorykMock = new Mock<IScheduleTaskRepository>();
            scheduleTasRepositorykMock.Setup(mock => mock.CreateScheduleTaskAsync(It.IsAny<ScheduleTask>(), ownerLogin))
                .Returns<ScheduleTask, string>(async (scheduleTask, ownerName) => new Result<ScheduleTask> { SuccessResult = scheduleTask });

            var loggerMock = new Mock<ILogger<ScheduleTaskService>>();

            IScheduleTaskService scheduleTaskService = new ScheduleTaskService(scheduleTasRepositorykMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskService.CreateScheduleTaskAsync(createModel, ownerLoginModel);

            //assert 
            Assert.IsTrue(actual.IsSuccess, $"Model and login is valid but was returned fail with Fail Code {actual.FailCode}");
            
            Assert.AreEqual(expectedResultModel.Title, actual.SuccessResult.Title);
            Assert.AreEqual(expectedResultModel.Description, actual.SuccessResult.Description);
            Assert.AreEqual(expectedResultModel.TaskPriority, actual.SuccessResult.TaskPriority);
            Assert.AreEqual(expectedResultModel.TaskStart, actual.SuccessResult.TaskStart);
            Assert.AreEqual(expectedResultModel.TaskEnd, actual.SuccessResult.TaskEnd);
            Assert.AreEqual(expectedResultModel.IsRepetitive, actual.SuccessResult.IsRepetitive);
            Assert.AreEqual(expectedResultModel.RepetitivePeriod, actual.SuccessResult.RepetitivePeriod);
            Assert.AreEqual(expectedResultModel.RepetitiveStart, actual.SuccessResult.RepetitiveStart);
            Assert.AreEqual(expectedResultModel.RepetitiveEnd, actual.SuccessResult.RepetitiveEnd);

            scheduleTasRepositorykMock.Verify(mock => mock.CreateScheduleTaskAsync(It.IsAny<ScheduleTask>(), ownerLogin),
                "Creating task require using IScheduleTaskService");

            scheduleTasRepositorykMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetValidScheduleTaskCreateModelAndInValidOwnerLogin), DynamicDataSourceType.Method)]
        public async Task CreateScheduleTaskAsync_InvalidOwnerLogin_BadAuthCookieStatusCodeAnd401(ScheduleTaskCreateModel createModel, string ownerLogin)
        {
            //arrange 
            var ownerLoginModel = new LoginModel(ownerLogin);

            var expectedFailCode = ResultFailCode.BadAuthCookie;
            var expectedStatusCode = StatusCodes.Status401Unauthorized;

            var scheduleTasRepositorykMock = new Mock<IScheduleTaskRepository>();

            var loggerMock = new Mock<ILogger<ScheduleTaskService>>();

            IScheduleTaskService scheduleTaskService = new ScheduleTaskService(scheduleTasRepositorykMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskService.CreateScheduleTaskAsync(createModel, ownerLoginModel);

            //assert 
            Assert.IsTrue(actual.IsFail, $"login \"{ownerLogin}\" is invalid but was returned success");

            Assert.AreEqual(expectedFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);

            scheduleTasRepositorykMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetInValidPeriodScheduleTaskCreateModelAndValidOwnerLogin), DynamicDataSourceType.Method)]
        [DynamicData(nameof(GetInValidTitleScheduleTaskCreateModelAndValidOwnerLogin), DynamicDataSourceType.Method)]
        [DynamicData(nameof(GetInValidStartTimeScheduleTaskCreateModelAndValidOwnerLogin), DynamicDataSourceType.Method)]
        [DynamicData(nameof(GetInValidPriorityScheduleTaskCreateModelAndValidOwnerLogin), DynamicDataSourceType.Method)]
        public async Task CreateScheduleTaskAsync_InvalidModel_Fail(ScheduleTaskCreateModel createModel, string ownerLogin, ResultFailCode expectedFailCode)
        {
            //arrange 
            var ownerLoginModel = new LoginModel(ownerLogin);

            var scheduleTasRepositorykMock = new Mock<IScheduleTaskRepository>();

            var loggerMock = new Mock<ILogger<ScheduleTaskService>>();

            IScheduleTaskService scheduleTaskService = new ScheduleTaskService(scheduleTasRepositorykMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskService.CreateScheduleTaskAsync(createModel, ownerLoginModel);

            //assert 
            Assert.IsTrue(actual.IsFail, $"model is invalid but was returned success");

            Assert.AreEqual(expectedFailCode, actual.FailCode);

            scheduleTasRepositorykMock.VerifyNoOtherCalls();
        }

        [DeploymentItem(CommonTests.LOGIN_AUTH_SIGNIN_VALID_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<string> GetValidLogin() => CommonTests.DeserializeJson<LoginAuthSignInModel>(CommonTests.LOGIN_AUTH_SIGNIN_VALID_FNAME)
            .Select(authModel => authModel.Login);

        [DeploymentItem(CommonTests.LOGIN_AUTH_SIGNIN_INVALID_LOGIN_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<string> GetInValidLogin() => CommonTests.DeserializeJson<LoginAuthSignInModel>(CommonTests.LOGIN_AUTH_SIGNIN_INVALID_LOGIN_FNAME)
            .Select(authModel => authModel.Login);

        [DeploymentItem(CommonTests.SCHEDULE_TASK_LINK_VALID_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<string> GetValidScheduleTaskLink() => CommonTests.DeserializeJson<string>(CommonTests.SCHEDULE_TASK_LINK_VALID_FNAME);

        [DeploymentItem(CommonTests.SCHEDULE_TASK_LINK_INVALID_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<string> GetInValidScheduleTaskLink() => CommonTests.DeserializeJson<string>(CommonTests.SCHEDULE_TASK_LINK_INVALID_FNAME);

        static IEnumerable<object[]> GetValidScheduleTaskLinkArg() => GetValidScheduleTaskLink()
            .Select(link => new object[] { link });

        static IEnumerable<object[]> GetInValidScheduleTaskLinkArg() => GetInValidScheduleTaskLink()
            .Select(link => new object[] { link });

        static IEnumerable<object[]> GetValidLoginAndValidScheduleTaskLinkArg() => GetValidLogin()
            .FullZip(GetValidScheduleTaskLink())
            .Select(args => new object[] { args.Item1, args.Item2 });

        static IEnumerable<object[]> GetInValidLoginAndValidScheduleTaskLinkArg() => GetInValidLogin()
            .LeftZip(GetValidScheduleTaskLink())
            .Select(args => new object[] { args.Item1, args.Item2 });

        static IEnumerable<object[]> GetValidLoginAndInValidScheduleTaskLinkArg() => GetInValidLogin()
            .RightZip(GetInValidScheduleTaskLink())
            .Select(args => new object[] { args.Item1, args.Item2 });

        static IEnumerable<object[]> GetInValidLoginAndInValidScheduleTaskLinkArg() => GetInValidLogin()
            .FullZip(GetInValidScheduleTaskLink())
            .Select(args => new object[] { args.Item1, args.Item2 });

        [DeploymentItem(CommonTests.SCHEDULE_TASK_CREATE_VALID_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<object[]> GetValidScheduleTaskCreateModelAndValidOwnerLogin() => GetValidLogin()
            .FullZip(CommonTests.DeserializeJson<ScheduleTaskCreateModel>(CommonTests.SCHEDULE_TASK_CREATE_VALID_FNAME))
            .Select(args => new object[] { args.Item2, args.Item1 });

        [DeploymentItem(CommonTests.SCHEDULE_TASK_CREATE_VALID_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<object[]> GetValidScheduleTaskCreateModelAndInValidOwnerLogin() => GetInValidLogin()
            .LeftZip(CommonTests.DeserializeJson<ScheduleTaskCreateModel>(CommonTests.SCHEDULE_TASK_CREATE_VALID_FNAME))
            .Select(args => new object[] { args.Item2, args.Item1 });

        [DeploymentItem(CommonTests.SCHEDULE_TASK_CREATE_INVALID_PERIOD_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<object[]> GetInValidPeriodScheduleTaskCreateModelAndValidOwnerLogin() => GetValidLogin()
            .RightZip(CommonTests.DeserializeJson<ScheduleTaskCreateModel>(CommonTests.SCHEDULE_TASK_CREATE_INVALID_PERIOD_FNAME))
            .Select(args => new object[] { args.Item2, args.Item1, ResultFailCode.BadScheduleTaskPeriod });

        [DeploymentItem(CommonTests.SCHEDULE_TASK_CREATE_INVALID_TITLE_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<object[]> GetInValidTitleScheduleTaskCreateModelAndValidOwnerLogin() => GetValidLogin()
            .RightZip(CommonTests.DeserializeJson<ScheduleTaskCreateModel>(CommonTests.SCHEDULE_TASK_CREATE_INVALID_TITLE_FNAME))
            .Select(args => new object[] { args.Item2, args.Item1, ResultFailCode.BadScheduleTaskTitle });

        [DeploymentItem(CommonTests.SCHEDULE_TASK_CREATE_INVALID_START_TIME_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<object[]> GetInValidStartTimeScheduleTaskCreateModelAndValidOwnerLogin() => GetValidLogin()
            .RightZip(CommonTests.DeserializeJson<ScheduleTaskCreateModel>(CommonTests.SCHEDULE_TASK_CREATE_INVALID_START_TIME_FNAME))
            .Select(args => new object[] { args.Item2, args.Item1, ResultFailCode.BadScheduleTaskTime });

        [DeploymentItem(CommonTests.SCHEDULE_TASK_CREATE_INVALID_PRIORITY_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<object[]> GetInValidPriorityScheduleTaskCreateModelAndValidOwnerLogin() => GetValidLogin()
            .RightZip(CommonTests.DeserializeJson<ScheduleTaskCreateModel>(CommonTests.SCHEDULE_TASK_CREATE_INVALID_PRIORITY_FNAME))
            .Select(args => new object[] { args.Item2, args.Item1, ResultFailCode.BadScheduleTaskPriority });
    }
}