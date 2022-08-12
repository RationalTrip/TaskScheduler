using System.Collections.Generic;
using FluentValidation.Results;
using System.Linq;
using Microsoft.AspNetCore.Http;
using TaskScheduler.Domain;
using TaskScheduler.Model;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace TaskScheduler.Application
{
    static class Extensions
    {
        public static Result<Tout> AuthentificatedUserLoginModelValidation<Tout>(this LoginModel loginModel, ILogger logger, [CallerMemberName] string callerName = "")
        {
            if (loginModel?.Login == null)
                return ApplicationCommonResults.UnauthorizedUserAccess<Tout>();

            var loginValidator = new LoginModelValidator().Validate(loginModel);

            if (!loginValidator.IsValid)
            {
                logger.LogError($"BadAuthCookie login \"{loginModel.Login}\" in method \"{callerName}\"");

                return ApplicationCommonResults.BadAuthCookieLoginResult<Tout>();
            }
            return null;
        }
        public static Result<T> ToResult<T>(this ValidationResult validator, (ResultFailCode propertyFailCode, string propertyName)[] validationFailCheck)
        {
            ResultFailCode failCode = ResultFailCode.None;
            var failCodeMessages = new Dictionary<ResultFailCode, IEnumerable<string>>();

            var validationResult = validator.ToDictionary();

            foreach (var (validationFailCode, validationPropertyName) in validationFailCheck)
            {
                if (validationResult.ContainsKey(validationPropertyName))
                {
                    failCode |= validationFailCode;

                    if (failCodeMessages.ContainsKey(validationFailCode))
                        failCodeMessages[validationFailCode] = failCodeMessages[validationFailCode].Concat(validationResult[validationPropertyName]);
                    else
                        failCodeMessages[validationFailCode] = validationResult[validationPropertyName];
                }
            }

            return new Result<T> { FailCode = failCode, FailCodeMessages = failCodeMessages, IsSuccess = false};
        }
    }
}
