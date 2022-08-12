using System;
using System.ComponentModel.DataAnnotations;
using TaskScheduler.Domain;

namespace TaskScheduler.Model
{
    public class ScheduleTaskCreateModel
    {
        public string Title { get; set; }
        public string Description { get; set; } = "";
        public DateTime TaskStart { get; set; }
        public DateTime TaskEnd { get; set; }
        public TaskPriority TaskPriority { get; set; } = TaskPriority.NormalPriority;
        public bool IsRepetitive { get; set; }
        public TaskRepetitivePeriod RepetitivePeriod { get; set; } = TaskRepetitivePeriod.Daily;

        [DataType(DataType.Date)]
        public DateTime RepetitiveStart => TaskStart.Date;

        [DataType(DataType.Date)]
        public DateTime RepetitiveEnd { get; set; }
        public ScheduleTask ToScheduleTask()
        {
            return new ScheduleTask(title: Title, description: Description, taskStart: TaskStart,
                taskEnd: TaskEnd, taskPriority: TaskPriority, isRepetitive: IsRepetitive,
                repetitivePeriod:RepetitivePeriod, repetitiveEnd: RepetitiveEnd);
        }
    }
}
