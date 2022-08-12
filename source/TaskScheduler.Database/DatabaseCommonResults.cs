using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TaskScheduler.Domain;

namespace TaskScheduler.Database
{
    static class DatabaseCommonResults
    {
        public static Result<T> GetAuthenticatedUserNotFoundResult<T>(ILogger logger, string login)
        {
            StackTrace stackTrace = new StackTrace();

            string methodName = stackTrace.GetFrame(1).GetMethod().Name;
            int callLine = stackTrace.GetFrame(1).GetFileLineNumber();

            logger.LogError($"BadAuthCookie login \"{login}\" in method \"{methodName}\" at line \"{callLine}\"");

            return new Result<T>
            {
                IsSuccess = false,
                StatusCode = StatusCodes.Status401Unauthorized,
                FailCode = ResultFailCode.BadAuthCookie,
                FailCodeMessages = new()
                {
                    { ResultFailCode.BadAuthCookie, new string[] { "Authenticated user not found. Please try sign in again." } }
                }
            };
        }

        public static Result<T> GetScheduleTaskNotFoundResult<T>() => 
            new Result<T>
            {
                IsSuccess = false,
                StatusCode = StatusCodes.Status404NotFound,
                FailCode = ResultFailCode.TaskNotFound,
                FailCodeMessages = new()
                {
                    { ResultFailCode.TaskNotFound, new string[] { "This task not exists. It may have been removed." } }
                }
            };

        public static Result<T> GetAccessDeniedResult<T>() =>
            new Result<T>
            {
                IsSuccess = false,
                StatusCode = StatusCodes.Status403Forbidden,
                FailCode = ResultFailCode.AccessDenied,
                FailCodeMessages = new()
                {
                    { ResultFailCode.AccessDenied, new string[] { "This operation is not permitted for you." } }
                }
            };

        public static Result<T> GetLoginAlreadyExistsResult<T>() =>
            new Result<T>
            {
                IsSuccess = false,
                FailCode = ResultFailCode.BadLogin,
                FailCodeMessages = new()
                    {
                        {ResultFailCode.BadLogin, new string[]{"Login already exists."} }
                    }
            };

        public static Result<T> GetBadLoginOrPasswordResult<T>() =>
            new Result<T>
            {
                IsSuccess = false,
                FailCode = ResultFailCode.BadLoginOrPassword,
                FailCodeMessages = new()
                {
                    { ResultFailCode.BadLoginOrPassword, new string[] { "Login or Password incorrect." } }
                }
            };

        public static Result<T> GetSuccessResult<T>(T successResult) =>
            new Result<T>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                SuccessResult = successResult
            };
    }
}
