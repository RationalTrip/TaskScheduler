using System.Collections.Generic;
using System;

namespace TaskScheduler.Domain
{
    public class ScheduleTask
    {
        public ScheduleTask() { }
        public ScheduleTask(string title, string description, DateTime taskStart,
            DateTime taskEnd, TaskPriority taskPriority, bool isRepetitive,
            TaskRepetitivePeriod repetitivePeriod, DateTime repetitiveEnd)
        {
            Title = title;
            Description = description;
            TaskStart = taskStart;
            TaskEnd = taskEnd;
            TaskPriority = taskPriority;
            IsRepetitive = isRepetitive;
            RepetitivePeriod = repetitivePeriod;
            RepetitiveEnd = repetitiveEnd;
        }
        public int TaskId { get; init; }
        public string Link { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime TaskStart { get; private set; }
        public DateTime TaskEnd { get; private set; }
        public TaskPriority TaskPriority { get; private set; }
        public User Owner { get; private set; }
        public bool IsRepetitive { get; private set; }
        public TaskRepetitivePeriod RepetitivePeriod { get; private set; }
        public DateTime RepetitiveStart => TaskStart.Date;
        public DateTime RepetitiveEnd { get; private set; }
        public List<User> Participants { get; private set; } = new List<User>();
        public void SetLink(string link) => Link = link;
        public void SetOwner(User owner) => Owner = owner;
    }
}
