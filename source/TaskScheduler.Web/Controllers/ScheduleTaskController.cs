using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TaskScheduler.Model;
using TaskScheduler.Application;
using TaskScheduler.Domain;
using Microsoft.Extensions.Logging;

namespace TaskScheduler.Web
{
    [Route(CONTROLLER_ROUTE)]
    public class ScheduleTaskController : Controller
    {
        public const string CONTROLLER_ROUTE = "tasks";

        public const string CREATE_SCHEDULE_TASK_ROUTE = "create";
        public const string DELETE_SCHEDULE_TASK_ROUTE = "delete";
        public const string GET_SCHEDULE_TASK_ROUTE = "get-task";
        public const string PARTICIPATE_SCHEDULE_TASK_ROUTE = "participate";
        public const string LEAVE_SCHEDULE_TASK_ROUTE = "leave";

        readonly IScheduleTaskService _scheduleTaskService;
        readonly ILogger<ScheduleTaskController> _logger;
        public ScheduleTaskController(IScheduleTaskService scheduleTaskService, ILogger<ScheduleTaskController> logger)
        {
            _scheduleTaskService = scheduleTaskService;
            _logger = logger;
        }

        [Route(CREATE_SCHEDULE_TASK_ROUTE)]
        public IActionResult CreateScheduleTask()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                TempData[WebCommon.DangerMessageName] = new string[] { "For creating task you should be authenticated. So use register or sign in for this." };
                return RedirectToAction(AuthController.SIGN_IN_ROUTE, AuthController.CONTROLLER_ROUTE);
            }

            return View(new ScheduleTaskCreateModel());
        }

        [HttpPost]
        [Route(CREATE_SCHEDULE_TASK_ROUTE)]
        public async Task<IActionResult> CreateScheduleTaskAsync(ScheduleTaskCreateModel createTaskModel)
        {
            string ownerLogin = HttpContext.User?.Identity?.Name;

            var taskResult = await _scheduleTaskService.CreateScheduleTaskAsync(createTaskModel, new LoginModel(ownerLogin));

            var badStatusCodeHandle = this.HandleBadStatusCode(taskResult);
            if (badStatusCodeHandle is not null)
                return badStatusCodeHandle;

            if (taskResult.IsFail)
                return View(createTaskModel);

            string taskLink = taskResult.SuccessResult.Link;

            return RedirectToAction(GET_SCHEDULE_TASK_ROUTE, CONTROLLER_ROUTE, new { arg = taskLink });
        }

        [Route(DELETE_SCHEDULE_TASK_ROUTE + "/{link}")]
        public async Task<IActionResult> DeleteScheduleTaskAsync(string link)
        {
            string ownerLogin = HttpContext.User?.Identity?.Name;
            
            var taskResult = await _scheduleTaskService.DeleteScheduleTaskAsync(new LoginModel(ownerLogin), link);

            var badStatusCodeHandle = this.HandleBadStatusCode(taskResult);
            if (badStatusCodeHandle is not null)
                return badStatusCodeHandle;

            if (taskResult.IsFail)
                return this.GetUnexpectedFailResult(_logger, taskResult);

            TempData[WebCommon.SuccessMessageName] = new string[] { "Task was successfully deleted" };

            return RedirectToAction(HomeController.INDEX_ROUTE, HomeController.CONTROLLER_ROUTE);
        }

        [Route(GET_SCHEDULE_TASK_ROUTE + "/{link}")]
        public async Task<IActionResult> GetScheduleTaskAsync(string link)
        {
            var taskResult = await _scheduleTaskService.GetScheduleTaskAsync(link);

            var badStatusCodeHandle = this.HandleBadStatusCode(taskResult);
            if (badStatusCodeHandle is not null)
                return badStatusCodeHandle;

            if (taskResult.IsFail)
                return this.GetUnexpectedFailResult(_logger, taskResult);

            return View(taskResult.SuccessResult);
        }

        [Route(PARTICIPATE_SCHEDULE_TASK_ROUTE + "/{link}")]
        public async Task<IActionResult> ParticipateScheduleTaskAsync(string link)
        {
            string ownerLogin = HttpContext.User?.Identity?.Name;

            var taskResult = await _scheduleTaskService.ParticipateScheduleTaskAsync(new LoginModel(ownerLogin), link);

            var badStatusCodeHandle = this.HandleBadStatusCode(taskResult);
            if (badStatusCodeHandle is not null)
                return badStatusCodeHandle;

            if (taskResult.IsFail)
                return this.GetUnexpectedFailResult(_logger, taskResult);

            TempData[WebCommon.SuccessMessageName] = new string[] { "You successfully participated this task." };

            return RedirectToAction(GET_SCHEDULE_TASK_ROUTE, CONTROLLER_ROUTE, new { arg = link });
        }

        [Route(LEAVE_SCHEDULE_TASK_ROUTE + "/{link}")]
        public async Task<IActionResult> LeaveScheduleTaskAsync(string link)
        {
            string ownerLogin = HttpContext.User?.Identity?.Name;

            var taskResult = await _scheduleTaskService.LeaveScheduleTaskAsync(new LoginModel(ownerLogin), link);

            var badStatusCodeHandle = this.HandleBadStatusCode(taskResult);
            if (badStatusCodeHandle is not null)
                return badStatusCodeHandle;

            if (taskResult.IsFail)
                return this.GetUnexpectedFailResult(_logger, taskResult);

            TempData[WebCommon.SuccessMessageName] = new string[] { "You successfully left this task." };

            return RedirectToAction(GET_SCHEDULE_TASK_ROUTE, CONTROLLER_ROUTE, new { arg = link });
        }
    }
}
