using System.Threading.Tasks;

namespace TaskScheduler.Domain
{
    public interface IScheduleTaskRepository
    {
        Task<Result<ScheduleTask>> GetScheduleTaskAsync(string link);
        Task<Result<ScheduleTask>> CreateScheduleTaskAsync(ScheduleTask taskToCreate, string ownerLogin);
        Task<Result<ScheduleTask>> ParticipateScheduleTaskAsync(string userLogin, string taskLink);
        Task<Result<ScheduleTask>> LeaveScheduleTaskAsync(string userLogin, string taskLink);
        Task<Result<ScheduleTask>> DeleteScheduleTaskAsync(string ownerLogin, string taskLink);
    }
}
