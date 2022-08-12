using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TaskScheduler.Domain;

namespace TaskScheduler.Web
{
    static class ControllerExtensions
    {
        public static IActionResult HandleBadStatusCode<T>(this Controller controller, Result<T> result)
        {
            controller.Response.StatusCode = result.StatusCode;

            if (result.StatusCode == StatusCodes.Status401Unauthorized)
            {
                controller.TempData[WebCommon.DangerMessageName] = result.ToErrorMessageEnumerable();

                return controller.RedirectToAction(AuthController.SIGN_IN_ROUTE, AuthController.CONTROLLER_ROUTE);
            }

            if (result.StatusCode == StatusCodes.Status403Forbidden)
            {
                controller.TempData[WebCommon.DangerMessageName] = result.ToErrorMessageEnumerable();

                return controller.RedirectToAction(HomeController.ERROR_PAGE_ROUTE, HomeController.CONTROLLER_ROUTE);
            }

            if (result.StatusCode == StatusCodes.Status404NotFound)
            {
                controller.TempData[WebCommon.DangerMessageName] = result.ToErrorMessageEnumerable();

                return controller.RedirectToAction(HomeController.ERROR_PAGE_ROUTE, HomeController.CONTROLLER_ROUTE);
            }

            return null;
        }
        public static IActionResult GetUnexpectedFailResult<Tlog>(this Controller controller, ILogger<Tlog> logger, string logMessage)
        {
            controller.TempData[WebCommon.DangerMessageName] = new string[] { WebCommon.SomethingBadHappend };
            controller.Response.StatusCode = StatusCodes.Status500InternalServerError;


            StackTrace stackTrace = new StackTrace();

            string methodName = stackTrace.GetFrame(1).GetMethod().Name;
            int callLine = stackTrace.GetFrame(1).GetFileLineNumber();

            logger.LogError($"{logMessage} in method \"{methodName}\" at line \"{callLine}\"");

            return controller.RedirectToAction(HomeController.ERROR_PAGE_ROUTE, HomeController.CONTROLLER_ROUTE);
        }
        public static IActionResult GetUnexpectedFailResult<Tlog, Tres>(this Controller controller, ILogger<Tlog> logger, Result<Tres> unsuportedResult)
        {
            string logMessage = $"On this method fail code \"{unsuportedResult.FailCode}\" with status code \"{unsuportedResult.StatusCode}\" is not supported result.";

            controller.TempData[WebCommon.DangerMessageName] = new string[] { WebCommon.SomethingBadHappend };
            controller.Response.StatusCode = StatusCodes.Status500InternalServerError;


            StackTrace stackTrace = new StackTrace();

            string methodName = stackTrace.GetFrame(1).GetMethod().Name;
            int callLine = stackTrace.GetFrame(1).GetFileLineNumber();

            logger.LogError($"{logMessage} in method \"{methodName}\" at line \"{callLine}\"");

            return controller.RedirectToAction(HomeController.ERROR_PAGE_ROUTE, HomeController.CONTROLLER_ROUTE);
        }
    }
}
