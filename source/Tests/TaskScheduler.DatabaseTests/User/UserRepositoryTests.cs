using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskScheduler.Model;
using TaskScheduler.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace TaskScheduler.Database.Tests
{
    [TestClass()]
    public class UserRepositoryTests
    {
        static TaskSchedulerContext _context;

        readonly static string dbName = "userRepoDb";

        [ClassInitialize]
        [DeploymentItem(CommonTests.REPOSITORY_DATA_FNAME, CommonTests.TEST_DATA_FOLDER)]
        public static void ClassInit(TestContext testContext)
        {
            var options = new DbContextOptionsBuilder().UseInMemoryDatabase(dbName);
            _context = new TaskSchedulerContext(options.Options);

            _context.SetDatabaseContext(CommonTests.REPOSITORY_DATA_FNAME);

            _context.SaveChanges();
        }

        [ClassCleanup]
        public static void ClassCleanup() => _context.Database.EnsureDeleted();

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetExistedUserLoginArgs), DynamicDataSourceType.Method)]
        public async Task GetOwnedTasksAsync_ExistedLogin_Success(string ownerLogin)
        {
            //arrange
            int ownerId = int.Parse(ownerLogin.AsSpan("user".Length));

            var expectedOwnedTasksIds = GeneratedDbCommon.GetOwnedScheduleTaskIds(ownerId).ToArray();

            var loggerMock = new Mock<ILogger<UserRepository>>();

            IUserRepository userRepository = new UserRepository(_context, loggerMock.Object);

            //act
            var actual = await userRepository.GetOwnedTasksAsync(ownerLogin);

            //assert
            Assert.IsTrue(actual.IsSuccess, $"user with login \"{ownerLogin}\" should be in batabase so result should be success");

            var actualOwnedTaskIds = (from taskId in actual.SuccessResult
                                      select taskId.TaskId).ToArray();

            CollectionAssert.AreEquivalent(expectedOwnedTasksIds, actualOwnedTaskIds);
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetNotExistedUserLoginArgs), DynamicDataSourceType.Method)]
        public async Task GetOwnedTasksAsync_NotExistedLogin_BadAuthCookieFailCodeAnd401(string ownerLogin)
        {
            //arrange
            var expectedStatusCode = StatusCodes.Status401Unauthorized;

            var loggerMock = new Mock<ILogger<UserRepository>>();

            IUserRepository userRepository = new UserRepository(_context, loggerMock.Object);

            //act
            var actual = await userRepository.GetOwnedTasksAsync(ownerLogin);

            //assert
            Assert.IsTrue(actual.IsFail, $"user with login \"{ownerLogin}\" should not be in batabase so result should be fail");
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetExistedUserLoginArgs), DynamicDataSourceType.Method)]
        public async Task GetParticipatedTasksAsync_ExistedLogin_Success(string login)
        {
            //arrange
            int ownerId = int.Parse(login.AsSpan("user".Length));

            var expectedOwnedTasksIds = GeneratedDbCommon.GetParticipatedScheduleTaskIds(ownerId).ToArray();

            var loggerMock = new Mock<ILogger<UserRepository>>();

            IUserRepository userRepository = new UserRepository(_context, loggerMock.Object);

            //act
            var actual = await userRepository.GetParticipatedTasksAsync(login);

            //assert
            Assert.IsTrue(actual.IsSuccess, $"user with login \"{login}\" should be in batabase so result should be success");

            var actualOwnedTaskIds = (from taskId in actual.SuccessResult
                                      select taskId.TaskId).ToArray();

            CollectionAssert.AreEquivalent(expectedOwnedTasksIds, actualOwnedTaskIds);
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetNotExistedUserLoginArgs), DynamicDataSourceType.Method)]
        public async Task GetParticipatedTasksAsync_NotExistedLogin_BadAuthCookieFailCodeAnd401(string login)
        {
            //arrange
            var expectedStatusCode = StatusCodes.Status401Unauthorized;

            var loggerMock = new Mock<ILogger<UserRepository>>();

            IUserRepository userRepository = new UserRepository(_context, loggerMock.Object);

            //act
            var actual = await userRepository.GetParticipatedTasksAsync(login);

            //assert
            Assert.IsTrue(actual.IsFail, $"user with login \"{login}\" should not be in batabase so result should be fail");
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetExistedLoginAndExistedTimePeriodArgs), DynamicDataSourceType.Method)]
        public async Task GetParticipatedScheduleTasksWithinTimeRangeAsync_ExistedLogin_Success(string login, DateTime dateStart, DateTime dateEnd)
        {
            //arrange
            int ownerId = int.Parse(login.AsSpan("user".Length));

            var expectedOwnedTasksIds = GeneratedDbCommon.GetParticipatedScheduleTaskIdsInDateRange(ownerId, dateStart, dateEnd).ToArray();

            var loggerMock = new Mock<ILogger<UserRepository>>();

            IUserRepository userRepository = new UserRepository(_context, loggerMock.Object);

            //act
            var actual = await userRepository.GetParticipatedScheduleTasksWithinTimeRangeAsync(login, dateStart, dateEnd);

            //assert
            Assert.IsTrue(actual.IsSuccess, $"user with login \"{login}\" should be in batabase so result should be success");

            var actualOwnedTaskIds = (from taskId in actual.SuccessResult
                                      select taskId.TaskId).ToArray();

            CollectionAssert.AreEquivalent(expectedOwnedTasksIds, actualOwnedTaskIds);
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetNotExistedLoginAndExistedTimePeriodArgs), DynamicDataSourceType.Method)]
        [DynamicData(nameof(GetNotExistedLoginAndNotExistedTimePeriodArgs), DynamicDataSourceType.Method)]
        public async Task GetParticipatedScheduleTasksWithinTimeRangeAsync_NotExistedLogin_BadAuthCookieFailCodeAnd401(string login, DateTime dateStart, DateTime dateEnd)
        {
            //arrange
            var expectedStatusCode = StatusCodes.Status401Unauthorized;

            var loggerMock = new Mock<ILogger<UserRepository>>();

            IUserRepository userRepository = new UserRepository(_context, loggerMock.Object);

            //act
            var actual = await userRepository.GetParticipatedScheduleTasksWithinTimeRangeAsync(login, dateStart, dateEnd);

            //assert
            Assert.IsTrue(actual.IsFail, $"user with login \"{login}\" should not be in batabase so result should be fail");
            Assert.AreEqual(expectedStatusCode, actual.StatusCode);
        }

        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(GetExistedLoginAndNotExistedTimePeriodArgs), DynamicDataSourceType.Method)]
        public async Task GetParticipatedScheduleTasksWithinTimeRangeAsync_NotExistedDateRange_SuccessEmptyResult(string login, DateTime dateStart, DateTime dateEnd)
        {
            //arrange
            var loggerMock = new Mock<ILogger<UserRepository>>();

            IUserRepository userRepository = new UserRepository(_context, loggerMock.Object);

            //act
            var actual = await userRepository.GetParticipatedScheduleTasksWithinTimeRangeAsync(login, dateStart, dateEnd);

            //assert
            Assert.IsTrue(actual.IsSuccess, $"user with login \"{login}\" should be in batabase so result should be success");

            var actualOwnedTaskIds = (from taskId in actual.SuccessResult
                                      select taskId.TaskId).ToArray();

            Assert.AreEqual(0, actualOwnedTaskIds.Length, "For this dateRange should be no tasks found");
        }

        static IEnumerable<string> GetExistedUserLogins() =>
            Enumerable.Range(GeneratedDbCommon.DbUserFirstId, GeneratedDbCommon.DbUserLastId - GeneratedDbCommon.DbUserFirstId + 1)
            .Select(userId => $"user{userId}");

        [DeploymentItem(CommonTests.LOGIN_AUTH_REGISTER_FILENAME, CommonTests.TEST_DATA_FOLDER)]
        static IEnumerable<string> GetNotExistedUserLogins() =>
            CommonTests.DeserializeJson<LoginAuthRegisterModel>(CommonTests.LOGIN_AUTH_REGISTER_FILENAME)
            .Select(authModel => authModel.Login)
            .Where(login => !login.StartsWith("user"));

        static IEnumerable<object[]> GetExistedUserLoginArgs() =>
            GetExistedUserLogins()
            .Select(login => new object[] { login });

        static IEnumerable<object[]> GetNotExistedUserLoginArgs() =>
            GetNotExistedUserLogins()
            .Select(login => new object[] { login });

        static IEnumerable<object[]> GetExistedLoginAndExistedTimePeriodArgs() =>
            GetExistedUserLogins()
            .FullZip(CommonTests.GetExistedDatePeriods())
            .Select(args => new object[] { args.Item1, args.Item2.dateStart, args.Item2.dateEnd });

        static IEnumerable<object[]> GetNotExistedLoginAndExistedTimePeriodArgs() =>
            GetNotExistedUserLogins()
            .LeftZip(CommonTests.GetExistedDatePeriods())
            .Select(args => new object[] { args.Item1, args.Item2.dateStart, args.Item2.dateEnd });

        static IEnumerable<object[]> GetExistedLoginAndNotExistedTimePeriodArgs() =>
            GetExistedUserLogins()
            .RightZip(CommonTests.GetNotExistedDatePeriods())
            .Select(args => new object[] { args.Item1, args.Item2.dateStart, args.Item2.dateEnd });

        static IEnumerable<object[]> GetNotExistedLoginAndNotExistedTimePeriodArgs() =>
            GetNotExistedUserLogins()
            .FullZip(CommonTests.GetNotExistedDatePeriods())
            .Select(args => new object[] { args.Item1, args.Item2.dateStart, args.Item2.dateEnd });
    }
}