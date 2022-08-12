using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskScheduler.Domain
{
    public interface IUserRepository
    {
        Task<Result<IEnumerable<ScheduleTask>>> GetOwnedTasksAsync(string login);
        Task<Result<IEnumerable<ScheduleTask>>> GetParticipatedTasksAsync(string login);
        Task<Result<IEnumerable<ScheduleTask>>> GetParticipatedScheduleTasksWithinTimeRangeAsync(string login, DateTime dateStart, DateTime dateEnd);
    }
}
