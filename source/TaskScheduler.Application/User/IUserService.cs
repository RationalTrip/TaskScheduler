using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskScheduler.Domain;
using TaskScheduler.Model;

namespace TaskScheduler.Application
{
    public interface IUserService
    {
        Task<Result<IEnumerable<ScheduleTask>>> GetParticipatedScheduleTasksAsync(LoginModel userLogin);
        Task<Result<IEnumerable<ScheduleTask>>> GetOwnedScheduleTasksAsync(LoginModel userLogin);
        Task<Result<IEnumerable<IndividualTask>>> GetParticipatedScheduleTasksInDayAsync(LoginModel userLogin, int year, int month, int day);
        Task<Result<IEnumerable<IndividualTask>>> GetParticipatedScheduleTasksInMonthAsync(LoginModel userLogin, int year, int month);
        Task<Result<IEnumerable<IndividualTask>>> GetParticipatedScheduleTasksInYearAsync(LoginModel userLogin, int year);
    }
}
