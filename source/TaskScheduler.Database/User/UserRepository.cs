using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskScheduler.Domain;

namespace TaskScheduler.Database
{
    public class UserRepository:IUserRepository
    {
        readonly TaskSchedulerContext _context;
        readonly ILogger<UserRepository> _logger;
        public UserRepository(TaskSchedulerContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<Result<IEnumerable<ScheduleTask>>> GetOwnedTasksAsync(string login)
        {
            if (!_context.LoginAuths.Any(loginAuth => loginAuth.Login == login))
                return DatabaseCommonResults.GetAuthenticatedUserNotFoundResult<IEnumerable<ScheduleTask>>(_logger, login);

            var ownedTask = from auth in _context.LoginAuths
                            where auth.Login == login
                            from task in auth.User.OwnedTasks
                                   select task;

            var ownedTaskResult = await ownedTask
                .Include(task => task.Participants)
                    .ThenInclude(user => user.LoginAuth)
                .AsNoTracking()
                .ToArrayAsync();

            return DatabaseCommonResults.GetSuccessResult(ownedTaskResult.AsEnumerable());
        }
        public async Task<Result<IEnumerable<ScheduleTask>>> GetParticipatedTasksAsync(string login)
        {
            if (!_context.LoginAuths.Any(loginAuth => loginAuth.Login == login))
                return DatabaseCommonResults.GetAuthenticatedUserNotFoundResult<IEnumerable<ScheduleTask>>(_logger, login);

            var participatedTask = from auth in _context.LoginAuths
                                   where auth.Login == login
                                   from task in auth.User.ParticipatedTasks
                                   select task;

            var participatedTaskResult = await participatedTask
                .Include(task => task.Owner)
                    .ThenInclude(user => user.LoginAuth)
                .AsNoTracking()
                .ToArrayAsync();

            return DatabaseCommonResults.GetSuccessResult(participatedTaskResult.AsEnumerable());
        }
        public async Task<Result<IEnumerable<ScheduleTask>>> GetParticipatedScheduleTasksWithinTimeRangeAsync(string login, DateTime dateStart, DateTime dateEnd)
        {
            if (!_context.LoginAuths.Any(loginAuth => loginAuth.Login == login))
                return DatabaseCommonResults.GetAuthenticatedUserNotFoundResult<IEnumerable<ScheduleTask>>(_logger, login);

            var participatedTask = from auth in _context.LoginAuths
                                   where auth.Login == login
                                   from task in auth.User.ParticipatedTasks
                                   where dateEnd > task.TaskStart && (dateStart < task.TaskEnd || task.IsRepetitive) 
                                   select task;

            var participatedTaskInTimeRange = (await participatedTask.AsNoTracking().ToArrayAsync())
                .Where(task => !task.IsRepetitive || dateStart < task.RepetitiveEnd + (task.TaskEnd - task.TaskStart.Date));

            return DatabaseCommonResults.GetSuccessResult(participatedTaskInTimeRange);
        }
    }
}
