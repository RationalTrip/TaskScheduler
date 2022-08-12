using Microsoft.AspNetCore.Http;
using TaskScheduler.Domain;

namespace TaskScheduler.Application
{
    static class ApplicationCommonResults
    {
        public static Result<T> UnauthorizedUserAccess<T>() =>
            new Result<T>
            {
                IsSuccess = false,
                StatusCode = StatusCodes.Status401Unauthorized,
                FailCode = ResultFailCode.OperatinRequireSignIn,
                FailCodeMessages = new ()
                {
                    {ResultFailCode.OperatinRequireSignIn, new string[]{"Operation require sign in."} }
                }
            };

        public static Result<T> BadAuthCookieLoginResult<T>() =>
            new Result<T>
            {
                IsSuccess = false,
                StatusCode = StatusCodes.Status401Unauthorized,
                FailCode = ResultFailCode.BadAuthCookie,
                FailCodeMessages = new ()
                {
                    {ResultFailCode.BadAuthCookie, new string[]{ "Problem occurred due to your session, please try to sign in again." } }
                }
            };

        public static Result<T> GetBadSelectedDateResult<T>() =>
            new Result<T>
            {
                IsSuccess = false,
                StatusCode = StatusCodes.Status404NotFound,
                FailCode = ResultFailCode.BadSelectedDate,
                FailCodeMessages = new()
                {
                    { ResultFailCode.BadSelectedDate, new string[] { "Selected date is not valid." } }
                }
            };

        public static Result<T> GetEmptyScheduleTaskLinkResult<T>() =>
            new Result<T>
            {
                IsSuccess = false,
                StatusCode = StatusCodes.Status404NotFound,
                FailCode = ResultFailCode.BadScheduleTaskLink,
                FailCodeMessages = new ()
                {
                    {ResultFailCode.BadScheduleTaskLink, new string[]{"Task link can not be empty."} }
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
    }
}
