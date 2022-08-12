using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskScheduler.Domain;

namespace TaskScheduler.Database
{
    public class ScheduleTaskRepository:IScheduleTaskRepository
    {
        readonly TaskSchedulerContext _context;
        readonly IScheduleTaskLinkGenerator _linkGenerator;
        readonly ILogger<ScheduleTaskRepository> _logger;

        public ScheduleTaskRepository(TaskSchedulerContext context, IScheduleTaskLinkGenerator linkGenerator, ILogger<ScheduleTaskRepository> logger)
        {
            _context = context;
            _linkGenerator = linkGenerator;
            _logger = logger;
        }

        public async Task<Result<ScheduleTask>> GetScheduleTaskAsync(string link)
        {
            var scheduleTask = await GetScheduleTaskByLinkQuery(link).AsNoTracking().SingleOrDefaultAsync();

            if (scheduleTask is null)
                return DatabaseCommonResults.GetScheduleTaskNotFoundResult<ScheduleTask>();

            return DatabaseCommonResults.GetSuccessResult(scheduleTask);
        }

        public async Task<Result<ScheduleTask>> CreateScheduleTaskAsync(ScheduleTask taskToCreate, string ownerLogin)
        {
            User owner = await GetUserByLoginAsync(ownerLogin);

            if (owner == null)
                return DatabaseCommonResults.GetAuthenticatedUserNotFoundResult<ScheduleTask>(_logger, ownerLogin);

            taskToCreate.SetOwner(owner);

            string link = _linkGenerator.GenerateLink(ownerLogin, taskToCreate.Title, 0);

            for(int i = 1; await IsScheduleTaskLinkExistsAsync(link) ; i++)
                link = _linkGenerator.GenerateLink(ownerLogin, taskToCreate.Title, i);

            taskToCreate.SetLink(link);

            await _context.ScheduleTasks.AddAsync(taskToCreate);

            await _context.SaveChangesAsync();

            return DatabaseCommonResults.GetSuccessResult(taskToCreate);
        }

        public async Task<Result<ScheduleTask>> ParticipateScheduleTaskAsync(string userLogin, string taskLink)
        {
            var scheduleTask = await GetScheduleTaskByLinkQuery(taskLink).SingleOrDefaultAsync();

            if (scheduleTask is null)
                return DatabaseCommonResults.GetScheduleTaskNotFoundResult<ScheduleTask>();

            User user = await GetUserByLoginAsync(userLogin);

            if (user == null)
                return DatabaseCommonResults.GetAuthenticatedUserNotFoundResult<ScheduleTask>(_logger, userLogin);

            if (!scheduleTask.Participants.Contains(user))
                scheduleTask.Participants.Add(user);

            await _context.SaveChangesAsync();

            return DatabaseCommonResults.GetSuccessResult(scheduleTask);
        }

        public async Task<Result<ScheduleTask>> LeaveScheduleTaskAsync(string userLogin, string taskLink)
        {
            var scheduleTask = await GetScheduleTaskByLinkQuery(taskLink).SingleOrDefaultAsync();

            if (scheduleTask is null)
                return DatabaseCommonResults.GetScheduleTaskNotFoundResult<ScheduleTask>();

            User user = await GetUserByLoginAsync(userLogin);

            if (user is null)
                return DatabaseCommonResults.GetAuthenticatedUserNotFoundResult<ScheduleTask>(_logger, userLogin);

            if (scheduleTask.Participants.Contains(user))
                scheduleTask.Participants.Remove(user);

            await _context.SaveChangesAsync();

            return DatabaseCommonResults.GetSuccessResult(scheduleTask);
        }

        public async Task<Result<ScheduleTask>> DeleteScheduleTaskAsync(string ownerLogin, string taskLink)
        {
            var scheduleTaskToRemove = await GetScheduleTaskByLinkQuery(taskLink).SingleOrDefaultAsync();

            if (scheduleTaskToRemove is null)
                return DatabaseCommonResults.GetScheduleTaskNotFoundResult<ScheduleTask>();

            User owner = await GetUserByLoginAsync(ownerLogin);

            if (owner is null)
                return DatabaseCommonResults.GetAuthenticatedUserNotFoundResult<ScheduleTask>(_logger, ownerLogin);

            if (owner != scheduleTaskToRemove.Owner)
                return DatabaseCommonResults.GetAccessDeniedResult<ScheduleTask>();

            _context.ScheduleTasks.Remove(scheduleTaskToRemove);

            await _context.SaveChangesAsync();

            return DatabaseCommonResults.GetSuccessResult(scheduleTaskToRemove);
        }

        async Task<User> GetUserByLoginAsync(string login)
        {
            return await _context.LoginAuths.Where(auth => auth.Login == login)
                .Select(auth => auth.User).SingleOrDefaultAsync();
        }

        IQueryable<ScheduleTask> GetScheduleTaskByLinkQuery(string link) =>
            _context.ScheduleTasks.Where(task => task.Link == link)
                .Include(task => task.Participants)
                    .ThenInclude(user => user.LoginAuth)
                .Include(task => task.Owner)
                    .ThenInclude(user => user.LoginAuth);

        async Task<bool> IsScheduleTaskLinkExistsAsync(string link) => await _context.ScheduleTasks.AnyAsync(task => task.Link == link);
    }
}
