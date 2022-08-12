using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler.Domain;

namespace TaskScheduler.Database.Tests
{
    class GeneratedDbCommon
    {
        public const int DbUserFirstId = 1;

        public const int DbUserLastId = 10;

        const int DbScheduleTasksYear = 2022;

        const int DbScheduleTaskStartHour = 18;

        const int DbScheduleTaskEndHour = 19;

        public const int MonthsInYear = 12;

        public static IEnumerable<int> GetParticipatedScheduleTaskIdsInDateRange(int userId, DateTime dateStart, DateTime dateEnd) =>
            GetScheduleTaskIdsInDateRange(dateStart, dateEnd)
            .Where(taskId => IsUserParticipateTask(taskId, userId));

        public static IEnumerable<int> GetParticipatedScheduleTaskIds(int userId) =>
            GetAllScheduleTaskId()
            .Where(taskId => IsUserParticipateTask(taskId, userId));

        public static IEnumerable<int> GetAllScheduleTaskId()
        {
            for (int userId = DbUserFirstId; userId <= DbUserLastId; userId++)
                for (int month = 1; month <= MonthsInYear; month++)
                    yield return GetScheduleTaskId(userId, month);
        }
        public static IEnumerable<int> GetOwnedScheduleTaskIds(int ownerId)
        {
            for (int month = 1; month <= MonthsInYear; month++)
                yield return GetScheduleTaskId(ownerId, month);
        }

        static IEnumerable<int> GetScheduleTaskIdsInDateRange(DateTime dateStart, DateTime dateEnd)
        {
            for (int ownerId = DbUserFirstId; ownerId <= DbUserLastId; ownerId++)
            {
                for (int month = 1; month <= MonthsInYear; month++)
                {
                    var taskStart = GetScheduleTaskStartTime(ownerId, month);
                    var taskEnd = GetScheduleTaskEndTime(ownerId, month);

                    var taskIsRepetitive = GetScheduleTaskIsRepetitive(ownerId, month);
                    var taskRepetitiveEnd = GetScheduleTaskRepetitiveEnd(month);

                    if (dateEnd > taskStart)
                        if (dateStart < taskEnd || taskIsRepetitive && dateStart < taskRepetitiveEnd + (taskEnd - taskStart.Date))
                            yield return GetScheduleTaskId(ownerId, month);
                }
            }
        }

        static bool IsUserParticipateTask(int taskId, int userId) => (taskId / 100 + taskId % 100) % userId == 0;

        public static string GetUserLogin(int userId) => $"user{userId}";
        public static string GetUserPassword(int userId) => $"Password{userId}";
        public static string GetUserSalt(int userId) => $"{userId}";
        public static int GetScheduleTaskId(int userId, int month) => userId * 100 + month;
        public static string GetScheduleTaskLink(int userId, int month) => GetScheduleTaskLink(GetScheduleTaskId(userId, month));
        public static string GetScheduleTaskLink(int taskId) => $"Task{taskId}";
        public static string GetScheduleTaskTitle(int userId, int month) => $"Task {GetScheduleTaskId(userId, month)} with owner User {userId}";
        public static string GetScheduleTaskDescription() => "";
        public static DateTime GetScheduleTaskStartTime(int userId, int month) => new DateTime(2022, month, userId, 18, 0, 0);
        public static DateTime GetScheduleTaskEndTime(int userId, int month) => new DateTime(2022, month, userId, 19, 0, 0);
        public static TaskPriority GetScheduleTaskPriority() => TaskPriority.NormalPriority;
        public static bool GetScheduleTaskIsRepetitive(int userId, int month) => (userId + month) % 2 == 0;
        public static TaskRepetitivePeriod GetScheduleTaskRepetitivePeriod() => TaskRepetitivePeriod.Daily;

        public static DateTime GetScheduleTaskRepetitiveEnd(int month) => new DateTime(2022, month, 1, 0, 0, 0).AddMonths(1);

        static ScheduleTask CreateScheduleTask(User owner, int month)
        {
            int ownerId = owner.UserId;

            var taskId = GetScheduleTaskId(ownerId, month);
            var taskTitle = GetScheduleTaskTitle(ownerId, month);
            var taskDescription = GetScheduleTaskDescription();
            var taskStart = GetScheduleTaskStartTime(ownerId, month);
            var taskEnd = GetScheduleTaskEndTime(ownerId, month);
            var taskLink = GetScheduleTaskLink(ownerId, month);
            var taskPriority = GetScheduleTaskPriority();
            var taskIsRepetitive = GetScheduleTaskIsRepetitive(ownerId, month);
            var taskRepetitivePeriod = GetScheduleTaskRepetitivePeriod();
            var taskRepetitiveEnd = GetScheduleTaskRepetitiveEnd(month);

            var task = new ScheduleTask(taskTitle, taskDescription, taskStart, taskEnd, taskPriority,
                taskIsRepetitive, taskRepetitivePeriod, taskRepetitiveEnd) { TaskId = taskId };

            task.SetOwner(owner);
            task.SetLink(taskLink);

            return task;
        }
        public static IEnumerable<ScheduleTask> GenerateDbByTasks()
        {
            User[] userPool = new User[DbUserLastId + 1];

            for (int userId = DbUserFirstId; userId <= DbUserLastId; userId++)
            {
                string login = GetUserLogin(userId);
                string password = GetUserPassword(userId);

                LoginAuth auth = new LoginAuth(login, password) { AuthId = userId };

                auth.SetSalt("");

                User user = new User(auth) { UserId = userId };

                userPool[userId] = user;
            }

            for (int ownerId = DbUserFirstId; ownerId <= DbUserLastId; ownerId++)
            {
                User owner = userPool[ownerId];
                for (int month = 1; month <= MonthsInYear; month++)
                {
                    var scheduleTask = CreateScheduleTask(owner, month);

                    for (var participantId = DbUserFirstId; participantId <= DbUserLastId; participantId++)
                        if (IsUserParticipateTask(scheduleTask.TaskId, participantId))
                            scheduleTask.Participants.Add(userPool[participantId]);
                    yield return scheduleTask;
                }
            }
        }
    }
}
