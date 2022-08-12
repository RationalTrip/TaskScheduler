using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskScheduler.Domain;
using TaskScheduler.Model;

namespace TaskScheduler.Application
{
    public class UserService:IUserService
    {
        readonly IUserRepository _userRepository;
        readonly IScheduleTaskToIndividualTaskConvertor _taskConvertor;
        readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IScheduleTaskToIndividualTaskConvertor taskConvertor, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _taskConvertor = taskConvertor;
            _logger = logger;
        }
        public async Task<Result<IEnumerable<ScheduleTask>>> GetParticipatedScheduleTasksAsync(LoginModel userLogin)
        {
            var loginValidationResult = userLogin.AuthentificatedUserLoginModelValidation<IEnumerable<ScheduleTask>>(_logger);
            if (loginValidationResult?.IsFail ?? false)
                return loginValidationResult;

            return await _userRepository.GetParticipatedTasksAsync(userLogin.Login);
        }
        public async Task<Result<IEnumerable<ScheduleTask>>> GetOwnedScheduleTasksAsync(LoginModel userLogin)
        {
            var loginValidationResult = userLogin.AuthentificatedUserLoginModelValidation <IEnumerable<ScheduleTask>> (_logger);
            if (loginValidationResult?.IsFail ?? false)
                return loginValidationResult;

            return await _userRepository.GetOwnedTasksAsync(userLogin.Login);
        }

        public async Task<Result<IEnumerable<IndividualTask>>> GetParticipatedScheduleTasksInDayAsync(LoginModel userLogin, int year, int month, int day) =>
            await GetParticipatedScheduleTasksInTimeRangeAsync(userLogin, year, month, day, (dateStart) => dateStart.AddDays(1));

        public async Task<Result<IEnumerable<IndividualTask>>> GetParticipatedScheduleTasksInMonthAsync(LoginModel userLogin, int year, int month) =>
            await GetParticipatedScheduleTasksInTimeRangeAsync(userLogin, year, month, DateTime.MinValue.Day, (dateStart) => dateStart.AddMonths(1));

        public async Task<Result<IEnumerable<IndividualTask>>> GetParticipatedScheduleTasksInYearAsync(LoginModel userLogin, int year) =>
            await GetParticipatedScheduleTasksInTimeRangeAsync(userLogin, year, DateTime.MinValue.Month, DateTime.MinValue.Day, (dateStart) => dateStart.AddYears(1));
        
        async Task<Result<IEnumerable<IndividualTask>>> GetParticipatedScheduleTasksInTimeRangeAsync(LoginModel userLogin, int year, int month, int day, Func<DateTime, DateTime> dateStartToDateEnd)
        {
            DateTime dateStart;
            try
            {
                dateStart = new DateTime(year, month, day);
            }
            catch (ArgumentOutOfRangeException)
            {
                return ApplicationCommonResults.GetBadSelectedDateResult<IEnumerable<IndividualTask>>();
            }

            var loginValidationResult = userLogin.AuthentificatedUserLoginModelValidation<IEnumerable<IndividualTask>>(_logger);
            if (loginValidationResult?.IsFail ?? false)
                return loginValidationResult;

            DateTime dateEnd = dateStartToDateEnd(dateStart);

            var scheduleTasksResult = await _userRepository.GetParticipatedScheduleTasksWithinTimeRangeAsync(userLogin.Login, dateStart, dateEnd);

            var individualTasks = _taskConvertor.ToIndividualTasks(scheduleTasksResult.SuccessResult, dateStart, dateEnd);

            return scheduleTasksResult.ToResult(individualTasks);
        }
    }
}
