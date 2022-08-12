using System;

namespace TaskScheduler.Domain
{
    public readonly struct IndividualTask
    {
        public readonly DateTime taskStart;
        public readonly DateTime taskEnd;
        public readonly ScheduleTask scheduleTask;

        public IndividualTask(DateTime taskStart, DateTime taskEnd, ScheduleTask scheduleTask)
        {
            this.taskStart = taskStart;
            this.taskEnd = taskEnd;
            this.scheduleTask = scheduleTask;
        }
    }
}
