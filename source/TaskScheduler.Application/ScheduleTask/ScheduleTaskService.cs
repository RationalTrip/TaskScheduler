using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler.Domain;
using TaskScheduler.Model;

namespace TaskScheduler.Application
{
    public class ScheduleTaskService:IScheduleTaskService
    {
        readonly IScheduleTaskRepository _scheduleTaskRepository;
        readonly ILogger<ScheduleTaskService> _logger;
        public ScheduleTaskService(IScheduleTaskRepository scheduleTaskRepository, ILogger<ScheduleTaskService> logger)
        {
            _scheduleTaskRepository = scheduleTaskRepository;
            _logger = logger;
        }
        static Result<T> ValidateScheduleTaskLink<T>(string link)
        {
            if (string.IsNullOrWhiteSpace(link))
                return ApplicationCommonResults.GetEmptyScheduleTaskLinkResult<T>();
            return null;
        }
        public async Task<Result<ScheduleTask>> GetScheduleTaskAsync(string taskLink)
        {
            var taskLinkValidationResult = ValidateScheduleTaskLink<ScheduleTask>(taskLink);
            if (taskLinkValidationResult?.IsFail ?? false)
                return taskLinkValidationResult;

            return await _scheduleTaskRepository.GetScheduleTaskAsync(taskLink);
        }
        public async Task<Result<ScheduleTask>> DeleteScheduleTaskAsync(LoginModel ownerLogin, string taskLink)
        {
            var taskLinkValidationResult = ValidateScheduleTaskLink<ScheduleTask>(taskLink);
            if (taskLinkValidationResult?.IsFail ?? false)
                return taskLinkValidationResult;

            var ownerLoginValidationResult = ownerLogin.AuthentificatedUserLoginModelValidation<ScheduleTask>(_logger);
            if (ownerLoginValidationResult?.IsFail ?? false)
                return ownerLoginValidationResult;

            return await _scheduleTaskRepository.DeleteScheduleTaskAsync(ownerLogin.Login, taskLink);
        }
        public async Task<Result<ScheduleTask>> CreateScheduleTaskAsync(ScheduleTaskCreateModel createTaskModel, LoginModel ownerLogin)
        {
            var ownerloginValidationResult = ownerLogin.AuthentificatedUserLoginModelValidation<ScheduleTask>(_logger);
            if (ownerloginValidationResult?.IsFail ?? false)
                return ownerloginValidationResult;

            var createTaskValidator = new ScheduleTaskCreateModelValidator().Validate(createTaskModel);

            if (createTaskValidator.IsValid)
                return await _scheduleTaskRepository.CreateScheduleTaskAsync(createTaskModel.ToScheduleTask(), ownerLogin.Login);

            (ResultFailCode, string)[] createTaskValidationFailCheck =
            {
                (ResultFailCode.BadScheduleTaskTitle, nameof(ScheduleTaskCreateModel.Title)),
                (ResultFailCode.BadScheduleTaskTime, nameof(ScheduleTaskCreateModel.TaskStart)),
                (ResultFailCode.BadScheduleTaskPriority, nameof(ScheduleTaskCreateModel.TaskPriority)),
                (ResultFailCode.BadScheduleTaskPeriod, nameof(ScheduleTaskCreateModel.RepetitivePeriod))
            };

            return createTaskValidator.ToResult<ScheduleTask>(createTaskValidationFailCheck);
        }
        public async Task<Result<ScheduleTask>> ParticipateScheduleTaskAsync(LoginModel userLogin, string taskLink)
        {
            var taskLinkValidationResult = ValidateScheduleTaskLink<ScheduleTask>(taskLink);
            if (taskLinkValidationResult?.IsFail ?? false)
                return taskLinkValidationResult;

            var userLoginValidationResult = userLogin.AuthentificatedUserLoginModelValidation<ScheduleTask>(_logger);
            if (userLoginValidationResult?.IsFail ?? false)
                return userLoginValidationResult;

            return await _scheduleTaskRepository.ParticipateScheduleTaskAsync(userLogin.Login, taskLink);
        }
        public async Task<Result<ScheduleTask>> LeaveScheduleTaskAsync(LoginModel userLogin, string taskLink)
        {
            var taskLinkValidationResult = ValidateScheduleTaskLink<ScheduleTask>(taskLink);
            if (taskLinkValidationResult?.IsFail ?? false)
                return taskLinkValidationResult;

            var userLoginValidationResult = userLogin.AuthentificatedUserLoginModelValidation<ScheduleTask>(_logger);
            if (userLoginValidationResult?.IsFail ?? false)
                return userLoginValidationResult;

            return await _scheduleTaskRepository.LeaveScheduleTaskAsync(userLogin.Login, taskLink);
        }
    }
}
