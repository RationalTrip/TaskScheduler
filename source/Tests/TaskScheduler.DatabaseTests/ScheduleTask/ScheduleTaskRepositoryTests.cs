using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskScheduler.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using TaskScheduler.Domain;
using Microsoft.EntityFrameworkCore;
using TaskScheduler.Model;
using Microsoft.Extensions.Logging;

[assembly: TestDataSourceDiscovery(TestDataSourceDiscoveryOption.DuringExecution)]
namespace TaskScheduler.Database.Tests
{
    [TestClass()]
    public class ScheduleTaskRepositoryTests
    {
        TaskSchedulerContext _context;

        readonly string dbName = "ScheduleTaskRepo";

        [TestInitialize]
        [DeploymentItem(CommonTests.REPOSITORY_DATA_FNAME, CommonTests.TEST_DATA_FOLDER)]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder().UseInMemoryDatabase(dbName);
            _context = new TaskSchedulerContext(options.Options);

            _context.SetDatabaseContext(CommonTests.REPOSITORY_DATA_FNAME);

            _context.SaveChanges();
        }

        [TestCleanup]
        public void TestCleanup() => _context.Database.EnsureDeleted();

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetScheduleTaskLinksExistedArgs), DynamicDataSourceType.Method)]
        public async Task GetScheduleTaskAsync_ExistedLink_TaskReturned(string taskLink)
        {
            //arrange 
            var expectedResult = _context.ScheduleTasks.Where(task => task.Link == taskLink).Single();

            var taskLinkMock = new Mock<IScheduleTaskLinkGenerator>();

            var loggerMock = new Mock<ILogger<ScheduleTaskRepository>>();

            var taskRepository = new ScheduleTaskRepository(_context, taskLinkMock.Object, loggerMock.Object);

            //act
            var actual = await taskRepository.GetScheduleTaskAsync(taskLink);

            //assert 
            Assert.IsTrue(actual.IsSuccess,
                $"Task with Link \"{taskLink}\" should exist in database, so IsSuccess should be true");

            Assert.IsTrue(expectedResult.IsEquivalent(actual.SuccessResult));

            taskLinkMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetScheduleTaskLinksNotExistedArgs), DynamicDataSourceType.Method)]
        public async Task GetScheduleTaskAsync_NotExistedLink_TaskNotFoundFaildCodeAnd404(string taskLink)
        {
            //arrange 
            var expectedStatusCode = StatusCodes.Status404NotFound;
            var expectedFailCode = ResultFailCode.TaskNotFound;

            var taskLinkMock = new Mock<IScheduleTaskLinkGenerator>();

            var loggerMock = new Mock<ILogger<ScheduleTaskRepository>>();

            var taskRepository = new ScheduleTaskRepository(_context, taskLinkMock.Object, loggerMock.Object);

            //act
            var actual = await taskRepository.GetScheduleTaskAsync(taskLink);

            //assert 
            Assert.IsTrue(actual.IsFail,
                $"Task with Link \"{taskLink}\" should not exist in database, so IsSuccess should be true");
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);
            Assert.AreEqual(expectedFailCode, actual.FailCode);

            taskLinkMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetScheduleTaskToCreateAndExistedOwnerArgs), DynamicDataSourceType.Method)]
        public async Task CreateScheduleTaskAsync_ValidData_Success(ScheduleTask taskToCreate, string ownerLogin)
        {
            var kek = GetScheduleTaskToCreateAndExistedOwnerArgs();
            //arrange
            User owner = _context.Users.Where(user => user.LoginAuth.Login == ownerLogin).Single();

            var linkGeneratorMock = new Mock<IScheduleTaskLinkGenerator>();

            var linkGeneratorResultQueue = new Queue<string>(new string[] { GeneratedDbCommon.GetScheduleTaskLink(owner.UserId, 1), $"GeneratedTask{Guid.NewGuid()}" });

            linkGeneratorMock.Setup(mock => mock.GenerateLink(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(linkGeneratorResultQueue.Dequeue);

            var loggerMock = new Mock<ILogger<ScheduleTaskRepository>>();

            IScheduleTaskRepository scheduleTaskRepository = new ScheduleTaskRepository(_context, linkGeneratorMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskRepository.CreateScheduleTaskAsync(taskToCreate, ownerLogin);

            //assert
            Assert.IsTrue(actual.IsSuccess);

            Assert.AreEqual(taskToCreate, actual.SuccessResult);
            Assert.AreEqual(owner, actual.SuccessResult.Owner);

            linkGeneratorMock.Verify(mock => mock.GenerateLink(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                "For creation link should be called for creation schedule task link");

            linkGeneratorMock.Verify(mock => mock.GenerateLink(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                "First link should already be in database, so link generation should be called again");

            linkGeneratorMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetScheduleTaskToCreateAndNotExistedOwnerArgs), DynamicDataSourceType.Method)]
        public async Task CreateScheduleTaskAsync_NotExistedUserLogin_BadAuthCookieAnd401(ScheduleTask taskToCreate, string ownerLogin)
        {
            //arrange
            var expectedFailCode = ResultFailCode.BadAuthCookie;
            var expectedStatusCode = StatusCodes.Status401Unauthorized;

            var linkGeneratorMock = new Mock<IScheduleTaskLinkGenerator>();

            var loggerMock = new Mock<ILogger<ScheduleTaskRepository>>();

            IScheduleTaskRepository scheduleTaskRepository = new ScheduleTaskRepository(_context, linkGeneratorMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskRepository.CreateScheduleTaskAsync(taskToCreate, ownerLogin);

            //assert
            Assert.IsTrue(actual.IsFail);

            Assert.AreEqual(expectedFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);

            linkGeneratorMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetExistedScheduleTaskLinksAndExistedLoginArgs), DynamicDataSourceType.Method)]
        public async Task ParticipateScheduleTaskAsync_ValidData_Success(string login, string scheduleTaskLink)
        {
            //arrange
            var user = _context.Users.Where(user => user.LoginAuth.Login == login).Single();
            var expectedScheduleTask = _context.ScheduleTasks.Where(task => task.Link == scheduleTaskLink).Single();

            var linkGeneratorMock = new Mock<IScheduleTaskLinkGenerator>();

            var loggerMock = new Mock<ILogger<ScheduleTaskRepository>>();

            IScheduleTaskRepository scheduleTaskRepository = new ScheduleTaskRepository(_context, linkGeneratorMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskRepository.ParticipateScheduleTaskAsync(login, scheduleTaskLink);

            //assert
            Assert.IsTrue(actual.IsSuccess);

            Assert.AreEqual(expectedScheduleTask, actual.SuccessResult);

            CollectionAssert.Contains(actual.SuccessResult.Participants, user);
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetNotExistedScheduleTaskLinksAndExistedLoginArgs), DynamicDataSourceType.Method)]
        [DynamicData(nameof(GetNotExistedScheduleTaskLinksAndNotExistedLoginArgs), DynamicDataSourceType.Method)]
        public async Task ParticipateScheduleTaskAsync_NotExistedScheduleTaskLink_TaskNotFoundFailCodeAnd404(string login, string scheduleTaskLink)
        {
            //arrange
            var expectedFailCode = ResultFailCode.TaskNotFound;
            var expectedStatusCode = StatusCodes.Status404NotFound;

            var linkGeneratorMock = new Mock<IScheduleTaskLinkGenerator>();

            var loggerMock = new Mock<ILogger<ScheduleTaskRepository>>();

            IScheduleTaskRepository scheduleTaskRepository = new ScheduleTaskRepository(_context, linkGeneratorMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskRepository.ParticipateScheduleTaskAsync(login, scheduleTaskLink);

            //assert
            Assert.IsTrue(actual.IsFail);

            Assert.AreEqual(expectedFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetExistedScheduleTaskLinksAndNotExistedLoginArgs), DynamicDataSourceType.Method)]
        public async Task ParticipateScheduleTaskAsync_ExistedScheduleTaskLinkNotExistedLogin_BadAuthCookieFailCodeAnd401(string login, string scheduleTaskLink)
        {
            //arrange
            var expectedFailCode = ResultFailCode.BadAuthCookie;
            var expectedStatusCode = StatusCodes.Status401Unauthorized;

            var linkGeneratorMock = new Mock<IScheduleTaskLinkGenerator>();

            var loggerMock = new Mock<ILogger<ScheduleTaskRepository>>();

            IScheduleTaskRepository scheduleTaskRepository = new ScheduleTaskRepository(_context, linkGeneratorMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskRepository.ParticipateScheduleTaskAsync(login, scheduleTaskLink);

            //assert
            Assert.IsTrue(actual.IsFail);

            Assert.AreEqual(expectedFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetExistedScheduleTaskLinksAndExistedLoginArgs), DynamicDataSourceType.Method)]
        public async Task LeaveScheduleTaskAsync_ValidData_Success(string login, string scheduleTaskLink)
        {
            //arrange
            var user = _context.Users.Where(user => user.LoginAuth.Login == login).Single();
            var expectedScheduleTask = _context.ScheduleTasks.Where(task => task.Link == scheduleTaskLink).Single();

            var linkGeneratorMock = new Mock<IScheduleTaskLinkGenerator>();

            var loggerMock = new Mock<ILogger<ScheduleTaskRepository>>();

            IScheduleTaskRepository scheduleTaskRepository = new ScheduleTaskRepository(_context, linkGeneratorMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskRepository.LeaveScheduleTaskAsync(login, scheduleTaskLink);

            //assert
            Assert.IsTrue(actual.IsSuccess);

            Assert.AreEqual(expectedScheduleTask, actual.SuccessResult);

            CollectionAssert.DoesNotContain(actual.SuccessResult.Participants, user);
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetNotExistedScheduleTaskLinksAndExistedLoginArgs), DynamicDataSourceType.Method)]
        [DynamicData(nameof(GetNotExistedScheduleTaskLinksAndNotExistedLoginArgs), DynamicDataSourceType.Method)]
        public async Task LeaveScheduleTaskAsync_NotExistedScheduleTaskLink_TaskNotFoundFailCodeAnd404(string login, string scheduleTaskLink)
        {
            //arrange
            var expectedFailCode = ResultFailCode.TaskNotFound;
            var expectedStatusCode = StatusCodes.Status404NotFound;

            var linkGeneratorMock = new Mock<IScheduleTaskLinkGenerator>();

            var loggerMock = new Mock<ILogger<ScheduleTaskRepository>>();

            IScheduleTaskRepository scheduleTaskRepository = new ScheduleTaskRepository(_context, linkGeneratorMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskRepository.LeaveScheduleTaskAsync(login, scheduleTaskLink);

            //assert
            Assert.IsTrue(actual.IsFail);

            Assert.AreEqual(expectedFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetExistedScheduleTaskLinksAndNotExistedLoginArgs), DynamicDataSourceType.Method)]
        public async Task LeaveScheduleTaskAsync_ExistedScheduleTaskLinkNotExistedLogin_BadAuthCookieFailCodeAnd401(string login, string scheduleTaskLink)
        {
            //arrange
            var expectedFailCode = ResultFailCode.BadAuthCookie;
            var expectedStatusCode = StatusCodes.Status401Unauthorized;

            var linkGeneratorMock = new Mock<IScheduleTaskLinkGenerator>();

            var loggerMock = new Mock<ILogger<ScheduleTaskRepository>>();

            IScheduleTaskRepository scheduleTaskRepository = new ScheduleTaskRepository(_context, linkGeneratorMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskRepository.LeaveScheduleTaskAsync(login, scheduleTaskLink);

            //assert
            Assert.IsTrue(actual.IsFail);

            Assert.AreEqual(expectedFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetDeleteTasksArgsValid), DynamicDataSourceType.Method)]
        public async Task DeleteScheduleTaskAsync_ValidData_Success(string ownerLogin, string scheduleTaskLink)
        {
            //arrange
            var expectedScheduleTask = _context.ScheduleTasks.Where(task => task.Link == scheduleTaskLink).Single();

            var linkGeneratorMock = new Mock<IScheduleTaskLinkGenerator>();

            var loggerMock = new Mock<ILogger<ScheduleTaskRepository>>();

            IScheduleTaskRepository scheduleTaskRepository = new ScheduleTaskRepository(_context, linkGeneratorMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskRepository.DeleteScheduleTaskAsync(ownerLogin, scheduleTaskLink);

            //assert
            Assert.IsTrue(actual.IsSuccess);

            Assert.AreEqual(expectedScheduleTask, actual.SuccessResult);

            Assert.IsFalse(_context.ScheduleTasks.Any(task => task.Link == scheduleTaskLink));
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetExistedScheduleTaskLinksAndNotExistedLoginArgs), DynamicDataSourceType.Method)]
        public async Task DeleteScheduleTaskAsync_NotExistedLogin_BadAuthCookieFailCodeAnd401(string ownerLogin, string scheduleTaskLink)
        {
            //arrange
            var expectedFailCode = ResultFailCode.BadAuthCookie;
            var expectedStatusCode = StatusCodes.Status401Unauthorized;

            var linkGeneratorMock = new Mock<IScheduleTaskLinkGenerator>();

            var loggerMock = new Mock<ILogger<ScheduleTaskRepository>>();

            IScheduleTaskRepository scheduleTaskRepository = new ScheduleTaskRepository(_context, linkGeneratorMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskRepository.DeleteScheduleTaskAsync(ownerLogin, scheduleTaskLink);

            //assert
            Assert.IsTrue(actual.IsFail);

            Assert.AreEqual(expectedFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetDeleteTasksArgsNotOwnerLoginArgs), DynamicDataSourceType.Method)]
        public async Task DeleteScheduleTaskAsync_NotOwnerLogin_AccessDeniedFailCodeAnd403(string ownerLogin, string scheduleTaskLink)
        {
            //arrange
            var expectedFailCode = ResultFailCode.AccessDenied;
            var expectedStatusCode = StatusCodes.Status403Forbidden;

            var linkGeneratorMock = new Mock<IScheduleTaskLinkGenerator>();

            var loggerMock = new Mock<ILogger<ScheduleTaskRepository>>();

            IScheduleTaskRepository scheduleTaskRepository = new ScheduleTaskRepository(_context, linkGeneratorMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskRepository.DeleteScheduleTaskAsync(ownerLogin, scheduleTaskLink);

            //assert
            Assert.IsTrue(actual.IsFail);

            Assert.AreEqual(expectedFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetNotExistedScheduleTaskLinksAndExistedLoginArgs), DynamicDataSourceType.Method)]
        [DynamicData(nameof(GetNotExistedScheduleTaskLinksAndNotExistedLoginArgs), DynamicDataSourceType.Method)]
        public async Task DeleteScheduleTaskAsync_NotExistedScheduleTaskLink_TaskNotFoundFailCodeAnd404(string login, string scheduleTaskLink)
        {
            //arrange
            var expectedFailCode = ResultFailCode.TaskNotFound;
            var expectedStatusCode = StatusCodes.Status404NotFound;

            var linkGeneratorMock = new Mock<IScheduleTaskLinkGenerator>();

            var loggerMock = new Mock<ILogger<ScheduleTaskRepository>>();

            IScheduleTaskRepository scheduleTaskRepository = new ScheduleTaskRepository(_context, linkGeneratorMock.Object, loggerMock.Object);

            //act
            var actual = await scheduleTaskRepository.DeleteScheduleTaskAsync(login, scheduleTaskLink);

            //assert
            Assert.IsTrue(actual.IsFail);

            Assert.AreEqual(expectedFailCode, actual.FailCode);
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);
        }

        static IEnumerable<string> GetScheduleTaskLinksExisted() =>
            GeneratedDbCommon.GetAllScheduleTaskId()
            .Select(taskId => GeneratedDbCommon.GetScheduleTaskLink(taskId));

        [DeploymentItem(CommonTests.SCHEDULE_TASK_LINK_INVALID_FNAME, CommonTests.TEST_DATA_FOLDER)]
        [DeploymentItem(CommonTests.SCHEDULE_TASK_LINK_VALID_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<string> GetScheduleTaskLinksNotExisted() =>
            CommonTests.DeserializeJson<string>(CommonTests.SCHEDULE_TASK_LINK_INVALID_FNAME)
            .Concat(CommonTests.DeserializeJson<string>(CommonTests.SCHEDULE_TASK_LINK_VALID_FNAME))
            .Where(link => !link.StartsWith("Task"));

        static IEnumerable<int> GetExistedUserIds() =>
            Enumerable.Range(GeneratedDbCommon.DbUserFirstId, GeneratedDbCommon.DbUserLastId - GeneratedDbCommon.DbUserFirstId + 1);

        static IEnumerable<string> GetExistedUserLogins() =>
            GetExistedUserIds()
            .Select(userId => $"user{userId}");

        [DeploymentItem(CommonTests.LOGIN_AUTH_REGISTER_FILENAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<string> GetNotExistedUserLogins() =>
            CommonTests.DeserializeJson<LoginAuthRegisterModel>(CommonTests.LOGIN_AUTH_REGISTER_FILENAME)
            .Select(authModel => authModel.Login)
            .Where(login => !login.StartsWith("user"));

        [DeploymentItem(CommonTests.SCHEDULE_TASK_CREATE_MODEL_FNAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<ScheduleTask> GetScheduleTaskToCreate() =>
            CommonTests.DeserializeJson<ScheduleTaskCreateModel>(CommonTests.SCHEDULE_TASK_CREATE_MODEL_FNAME)
            .Select(model => model.ToScheduleTask());

        static IEnumerable<object[]> GetScheduleTaskLinksExistedArgs() =>
            GetScheduleTaskLinksExisted()
            .Select(link => new object[] { link });

        static IEnumerable<object[]> GetScheduleTaskLinksNotExistedArgs() =>
            GetScheduleTaskLinksNotExisted()
            .Select(link => new object[] { link });

        static IEnumerable<object[]> GetScheduleTaskToCreateAndExistedOwnerArgs() =>
            GetScheduleTaskToCreate()
            .FullZip(GetExistedUserLogins())
            .Select(args => new object[] { args.Item1, args.Item2 });

        static IEnumerable<object[]> GetScheduleTaskToCreateAndNotExistedOwnerArgs() =>
            GetScheduleTaskToCreate()
            .RightZip(GetNotExistedUserLogins())
            .Select(args => new object[] { args.Item1, args.Item2 });

        static IEnumerable<object[]> GetExistedScheduleTaskLinksAndExistedLoginArgs() =>
            GetExistedUserLogins()
            .FullZip(GetScheduleTaskLinksExisted())
            .Select(args => new object[] { args.Item1, args.Item2 });

        static IEnumerable<object[]> GetNotExistedScheduleTaskLinksAndExistedLoginArgs() =>
            GetExistedUserLogins()
            .RightZip(GetScheduleTaskLinksNotExisted())
            .Select(args => new object[] { args.Item1, args.Item2 });

        static IEnumerable<object[]> GetExistedScheduleTaskLinksAndNotExistedLoginArgs() =>
            GetNotExistedUserLogins()
            .LeftZip(GetScheduleTaskLinksExisted())
            .Select(args => new object[] { args.Item1, args.Item2 });

        static IEnumerable<object[]> GetNotExistedScheduleTaskLinksAndNotExistedLoginArgs() =>
            GetNotExistedUserLogins()
            .FullZip(GetScheduleTaskLinksNotExisted())
            .Select(args => new object[] { args.Item1, args.Item2 });

        static IEnumerable<object[]> GetDeleteTasksArgsValid()
        {
            return from userId in Enumerable.Range(GeneratedDbCommon.DbUserFirstId, GeneratedDbCommon.DbUserLastId)
                   from month in Enumerable.Range(1, GeneratedDbCommon.MonthsInYear)
                   select new object[] { GeneratedDbCommon.GetUserLogin(userId), GeneratedDbCommon.GetScheduleTaskLink(userId, month) };
        }

        static IEnumerable<object[]> GetDeleteTasksArgsNotOwnerLoginArgs()
        {
            int wrongOwnerId = GeneratedDbCommon.DbUserFirstId + 1;
            int month = 1;

            for(int ownerId = GeneratedDbCommon.DbUserFirstId; ownerId <= GeneratedDbCommon.DbUserLastId; ownerId++)
            {
                month = month % GeneratedDbCommon.MonthsInYear + 1;
                wrongOwnerId = wrongOwnerId % (GeneratedDbCommon.DbUserLastId - GeneratedDbCommon.DbUserFirstId) + GeneratedDbCommon.DbUserFirstId;

                if (wrongOwnerId != ownerId)
                    yield return new object[] { GeneratedDbCommon.GetUserLogin(wrongOwnerId), GeneratedDbCommon.GetScheduleTaskLink(ownerId, month) };

                month += 5;
                wrongOwnerId += 7;
            }
        }
    }
}