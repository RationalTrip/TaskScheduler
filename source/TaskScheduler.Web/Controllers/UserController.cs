using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TaskScheduler.Application;
using TaskScheduler.Domain;
using TaskScheduler.Model;

namespace TaskScheduler.Web
{
    [Route(CONTROLLER_ROUTE)]
    public class UserController : Controller
    {
        public const string CONTROLLER_ROUTE = "user";

        public const string GET_PARTICIPATED_SCHEDULE_TASKS_ROUTE = "participated-tasks";
        public const string GET_OWNED_SCHEDULE_TASKS_ROUTE = "my-tasks";
        public const string GET_CALENDAR_DAY_ROUTE = "calendar";
        public const string GET_CALENDAR_MONTH_ROUTE = "calendar";
        public const string GET_CALENDAR_YEAR_ROUTE = "calendar";

        readonly IUserService _userService;
        readonly ILogger<UserController> _logger;
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [Route(GET_PARTICIPATED_SCHEDULE_TASKS_ROUTE)]
        public async Task<IActionResult> GetParticipatedScheduleTasksAsync()
        {
            string login = HttpContext.User?.Identity?.Name;
            var participatedTaskResult = await _userService.GetParticipatedScheduleTasksAsync(new LoginModel(login));

            var badStatusCodeHandle = this.HandleBadStatusCode(participatedTaskResult);
            if (badStatusCodeHandle is not null)
                return badStatusCodeHandle;

            if (participatedTaskResult.IsFail)
            {
                TempData[WebCommon.DangerMessageName] = WebCommon.SomethingBadHappend;
                return RedirectToAction(nameof(HomeController.ErrorPage), nameof(HomeController));
            }

            return View(participatedTaskResult.SuccessResult);
        }

        [Route(GET_OWNED_SCHEDULE_TASKS_ROUTE)]
        public async Task<IActionResult> GetOwnedScheduleTasksAsync()
        {
            string login = HttpContext.User?.Identity?.Name;
            var ownedTaskResult = await _userService.GetOwnedScheduleTasksAsync(new LoginModel(login));

            var badStatusCodeHandle = this.HandleBadStatusCode(ownedTaskResult);
            if (badStatusCodeHandle is not null)
                return badStatusCodeHandle;

            if (ownedTaskResult.IsFail)
                return this.GetUnexpectedFailResult(_logger, ownedTaskResult);

            return View(ownedTaskResult.SuccessResult);
        }

        [Route(GET_CALENDAR_DAY_ROUTE + "/{day:int:min(1):max(31)}-{month:int:min(1):max(12)}-{year:int:min(1990):max(2100)}")]
        public async Task<IActionResult> GetCalendarDayAsync(int year, int month, int day)
        {
            string login = HttpContext.User?.Identity?.Name;
            var ownedTaskResult = await _userService.GetParticipatedScheduleTasksInDayAsync(new LoginModel(login), year, month, day);

            if (!HttpContext.User.Identity.IsAuthenticated && ownedTaskResult.StatusCode != StatusCodes.Status404NotFound)
            {
                TempData[WebCommon.WarningMessageName] = new string[] { WebCommon.AuthorizedCalendarUsageWarning };
                ViewData[WebCommon.CurrentCalendarDate] = new DateTime(year, month, day);

                return View(null);
            }

            var badStatusCodeHandle = this.HandleBadStatusCode(ownedTaskResult);
            if (badStatusCodeHandle is not null)
                return badStatusCodeHandle;

            if (ownedTaskResult.IsFail)
                return this.GetUnexpectedFailResult(_logger, ownedTaskResult);

            ViewData[WebCommon.CurrentCalendarDate] = new DateTime(year, month, day);
            return View(ownedTaskResult.SuccessResult);
        }

        [Route(GET_CALENDAR_MONTH_ROUTE + "/{month:int:min(1):max(12)}-{year:int:min(1990):max(2100)}")]
        public async Task<IActionResult> GetCalendarMonthAsync(int year, int month)
        {
            string login = HttpContext.User?.Identity?.Name;
            var ownedTaskResult = await _userService.GetParticipatedScheduleTasksInMonthAsync(new LoginModel(login), year, month);

            if (!HttpContext.User.Identity.IsAuthenticated && ownedTaskResult.StatusCode != StatusCodes.Status404NotFound)
            {
                TempData[WebCommon.WarningMessageName] = new string[] { WebCommon.AuthorizedCalendarUsageWarning };
                ViewData[WebCommon.CurrentCalendarDate] = new DateTime(year, month, 1);

                return View(null);
            }

            var badStatusCodeHandle = this.HandleBadStatusCode(ownedTaskResult);
            if (badStatusCodeHandle is not null)
                return badStatusCodeHandle;

            if (ownedTaskResult.IsFail)
                return this.GetUnexpectedFailResult(_logger, ownedTaskResult);

            ViewData[WebCommon.CurrentCalendarDate] = new DateTime(year, month, 1);
            return View(ownedTaskResult.SuccessResult);
        }


        [Route(GET_CALENDAR_YEAR_ROUTE + "/{year:int:min(1990):max(2100)}")]
        public async Task<IActionResult> GetCalendarYearAsync(int year)
        {
            string login = HttpContext.User?.Identity?.Name;
            var ownedTaskResult = await _userService.GetParticipatedScheduleTasksInYearAsync(new LoginModel(login), year);

            if (!HttpContext.User.Identity.IsAuthenticated && ownedTaskResult.StatusCode != StatusCodes.Status404NotFound)
            {
                TempData[WebCommon.WarningMessageName] = new string[] { WebCommon.AuthorizedCalendarUsageWarning };
                ViewData[WebCommon.CurrentCalendarDate] = new DateTime(year, 1, 1);

                return View(null);
            }

            var badStatusCodeHandle = this.HandleBadStatusCode(ownedTaskResult);
            if (badStatusCodeHandle is not null)
                return badStatusCodeHandle;

            if (ownedTaskResult.IsFail)
                return this.GetUnexpectedFailResult(_logger, ownedTaskResult);

            ViewData[WebCommon.CurrentCalendarDate] = new DateTime(year, 1, 1);
            return View(ownedTaskResult.SuccessResult);
        }
    }
}
