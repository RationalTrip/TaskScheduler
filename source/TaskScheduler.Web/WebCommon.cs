using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace TaskScheduler.Web
{
    public static class WebCommon
    {
        public static readonly string CookieName = "TaskShedulerCookie";

        public static readonly string SuccessMessageName = "HeaderSuccess";
        public static readonly string DangerMessageName = "HeaderDanger";
        public static readonly string WarningMessageName = "HeaderWarning";

        public static readonly string ErrorBag = "ErrorBag";

        public static readonly string CurrentCalendarDate = "CalendarDate";

        public static readonly string SomethingBadHappend = "Something bad happend. Please try again later.";
        public static readonly string AuthorizedCalendarUsageWarning = "Unless you authorize, you won't be able see your tasks. For watching your tasks on the table, you should sign in or register.";
    }
}
