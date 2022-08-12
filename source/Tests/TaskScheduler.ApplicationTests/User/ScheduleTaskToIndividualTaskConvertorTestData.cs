using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TaskScheduler.Domain;

namespace TaskScheduler.Application.Tests
{
    static class ScheduleTaskToIndividualTaskConvertorTestData
    {
        static readonly ScheduleTask repetitiveTaskSchedule = new ScheduleTask("Repetitive Task", "", new DateTime(2022, 6, 25, 10, 0, 0), new DateTime(2022, 6, 25, 18, 0, 0),
            TaskPriority.NormalPriority, true, TaskRepetitivePeriod.Daily, new DateTime(2022, 7, 5));

        static readonly ScheduleTask nonRepetitiveTaskSchedule = new ScheduleTask("Non Repetitive Task", "", new DateTime(2022, 7, 1, 10, 0, 0), new DateTime(2022, 7, 1, 18, 0, 0),
            TaskPriority.NormalPriority, false, TaskRepetitivePeriod.Daily, new DateTime());

        static readonly (DateTime dateStart, DateTime dateEnd)[] testedDates = new[]
        {
            (new DateTime(2022, 1, 1), new DateTime(2023, 1, 1)),
            (new DateTime(2022, 7, 1, 15, 0, 0), new DateTime(2023, 7, 1, 19, 0, 0)),
            (new DateTime(2022, 7, 1, 8, 0, 0), new DateTime(2023, 7, 1, 9, 0, 0)),
            (new DateTime(2022, 7, 1), new DateTime(2023, 7, 10)),
            (new DateTime(2022, 7, 1, 20, 0, 0), new DateTime(2023, 7, 10)),
            (new DateTime(2022, 6, 30), new DateTime(2023, 7, 5)),
            (new DateTime(2022, 6, 25), new DateTime(2023, 7, 1))
        };

        static IEnumerable<IndividualTask> GetAllRepetitiveIndividualTasks()
        {
            var dateStart = repetitiveTaskSchedule.TaskStart;
            var dateEnd = repetitiveTaskSchedule.TaskEnd;

            while(dateStart < repetitiveTaskSchedule.RepetitiveEnd)
            {
                yield return new IndividualTask(dateStart, dateEnd, repetitiveTaskSchedule);

                dateStart = dateStart.AddScheduleTaskPeriod(repetitiveTaskSchedule.RepetitivePeriod);
                dateEnd = dateEnd.AddScheduleTaskPeriod(repetitiveTaskSchedule.RepetitivePeriod);
            }
        }

        static IEnumerable<IndividualTask> GetRepetitiveIndividualTasksInTimeRange(DateTime dateStart, DateTime dateEnd) =>
            GetAllRepetitiveIndividualTasks()
            .Where(task => task.taskStart < dateEnd && task.taskEnd > dateStart);

        static IEnumerable<IndividualTask> GetNonRepetitiveIndividualTasksInTimeRange(DateTime dateStart, DateTime dateEnd) =>
            new IndividualTask[] {new IndividualTask(nonRepetitiveTaskSchedule.TaskStart, nonRepetitiveTaskSchedule.TaskEnd, nonRepetitiveTaskSchedule) }
            .Where(task => task.taskStart < dateEnd && task.taskEnd > dateStart);

        public static IEnumerable<object[]> GetRepetitiveIndividualTasksArgs()
        {
            foreach(var (dateStart, dateEnd) in testedDates)
            {
                var scheduleTasks = new ScheduleTask[] { repetitiveTaskSchedule };
                var expectedResult = GetRepetitiveIndividualTasksInTimeRange(dateStart, dateEnd);

                yield return new object[] { scheduleTasks, dateStart, dateEnd, expectedResult };
            }
        }
        public static IEnumerable<object[]> GetNonRepetitiveIndividualTasksArgs()
        {
            foreach (var (dateStart, dateEnd) in testedDates)
            {
                var scheduleTasks = new ScheduleTask[] { nonRepetitiveTaskSchedule };
                var expectedResult = GetNonRepetitiveIndividualTasksInTimeRange(dateStart, dateEnd);

                yield return new object[] { scheduleTasks, dateStart, dateEnd, expectedResult };
            }
        }
        public static IEnumerable<object[]> GetRepetitiveAndNonRepetitiveIndividualTasksArgs()
        {
            foreach (var (dateStart, dateEnd) in testedDates)
            {
                var scheduleTasks = new ScheduleTask[] { repetitiveTaskSchedule, nonRepetitiveTaskSchedule };
                var expectedResult = GetNonRepetitiveIndividualTasksInTimeRange(dateStart, dateEnd)
                    .Concat(GetRepetitiveIndividualTasksInTimeRange(dateStart, dateEnd));

                yield return new object[] { scheduleTasks, dateStart, dateEnd, expectedResult };
            }
        }
    }
}
