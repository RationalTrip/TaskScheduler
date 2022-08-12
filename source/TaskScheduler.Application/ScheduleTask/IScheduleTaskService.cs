using System.Threading.Tasks;
using TaskScheduler.Domain;
using TaskScheduler.Model;

namespace TaskScheduler.Application
{
    public interface IScheduleTaskService
    {
        Task<Result<ScheduleTask>> GetScheduleTaskAsync(string taskLink);
        Task<Result<ScheduleTask>> DeleteScheduleTaskAsync(LoginModel ownerLogin, string taskLink);
        Task<Result<ScheduleTask>> CreateScheduleTaskAsync(ScheduleTaskCreateModel createTaskModel, LoginModel ownerLogin);
        Task<Result<ScheduleTask>> ParticipateScheduleTaskAsync(LoginModel userLogin, string taskLink);
        Task<Result<ScheduleTask>> LeaveScheduleTaskAsync(LoginModel userLogin, string taskLink);
    }
}
