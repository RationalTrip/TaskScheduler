using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace TaskScheduler.Domain
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; } = true;
        public bool IsFail => !IsSuccess;
        public int StatusCode { get; set; } = StatusCodes.Status200OK;
        public ResultFailCode FailCode { get; set; } = ResultFailCode.None;
        public Dictionary<ResultFailCode, IEnumerable<string>> FailCodeMessages { get; set; }
        public T SuccessResult { get; set; }
        public Result<TOut> ToResult<TOut>(TOut sucessResult = default) =>
            new Result<TOut>
            {
                IsSuccess = this.IsSuccess,
                StatusCode = this.StatusCode,
                FailCode = this.FailCode,
                FailCodeMessages = this.FailCodeMessages,
                SuccessResult = sucessResult
            };
        public IEnumerable<string> ToErrorMessageEnumerable()
        {
            var errors = new List<string>();

            foreach (var message in FailCodeMessages.Values)
                errors.AddRange(message);

            return errors;
        }
    }
}