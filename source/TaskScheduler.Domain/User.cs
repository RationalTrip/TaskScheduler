using System.Collections.Generic;

namespace TaskScheduler.Domain
{
    public class User
    {
        public User() { }
        public User(LoginAuth loginAuth) => LoginAuth = loginAuth;
        public int UserId { get; init; }
        public int LoginAuthId { get; private set; }
        public LoginAuth LoginAuth { get; private set; }
        public List<ScheduleTask> OwnedTasks { get; private set; } = new List<ScheduleTask>();
        public List<ScheduleTask> ParticipatedTasks { get; private set; } = new List<ScheduleTask>();
    }
}
