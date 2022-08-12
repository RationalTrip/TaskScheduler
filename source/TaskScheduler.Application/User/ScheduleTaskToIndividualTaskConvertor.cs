using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler.Domain;

namespace TaskScheduler.Application
{
    public class ScheduleTaskToIndividualTaskConvertor : IScheduleTaskToIndividualTaskConvertor
    {
        public IEnumerable<IndividualTask> ToIndividualTasks(IEnumerable<ScheduleTask> scheduleTasks, DateTime dateStart, DateTime dateEnd)
        {
            if (scheduleTasks is null)
                return null;

            return from taskScheme in scheduleTasks
                   from individualTask in ToIndividualTasks(taskScheme, dateStart, dateEnd)
                   select individualTask;
        }

        IEnumerable<IndividualTask> ToIndividualTasks(ScheduleTask scheduleTask, DateTime dateStart, DateTime dateEnd)
        {
            if (scheduleTask.TaskStart < dateEnd && scheduleTask.TaskEnd > dateStart)
                yield return new IndividualTask(scheduleTask.TaskStart, scheduleTask.TaskEnd, scheduleTask);

            if (!scheduleTask.IsRepetitive)
                yield break;

            int firstPeriodIdDateRange = scheduleTask.RepetitivePeriod.PeriodsCountBetweenDates(scheduleTask.TaskEnd, dateStart);
            firstPeriodIdDateRange = firstPeriodIdDateRange < 1 ? 1 : firstPeriodIdDateRange;

            var individualTaskStart = scheduleTask.TaskStart.AddScheduleTaskPeriod(scheduleTask.RepetitivePeriod, firstPeriodIdDateRange);
            var individualTaskEnd = scheduleTask.TaskEnd.AddScheduleTaskPeriod(scheduleTask.RepetitivePeriod, firstPeriodIdDateRange);

            while (individualTaskStart < scheduleTask.RepetitiveEnd && individualTaskStart < dateEnd)
            {
                if(individualTaskStart > dateStart || individualTaskEnd > dateStart)
                    yield return new IndividualTask(individualTaskStart, individualTaskEnd, scheduleTask);

                individualTaskStart = individualTaskStart.AddScheduleTaskPeriod(scheduleTask.RepetitivePeriod);
                individualTaskEnd = individualTaskEnd.AddScheduleTaskPeriod(scheduleTask.RepetitivePeriod);
            }
        }
    }
}
