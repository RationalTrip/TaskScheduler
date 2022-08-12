using System;

namespace TaskScheduler.Domain
{
    [Flags]
    public enum ResultFailCode
    {
        None = 0,
        BadLogin = 1,
        BadPassword = 1 << 1,
        BadLoginOrPassword = 1 << 2,
        BadConfirmPassword = 1 << 3,
        BadAuthCookie = 1 << 4,
        OperatinRequireSignIn = 1 << 5,
        BadScheduleTaskLink = 1 << 6,
        BadScheduleTaskTitle = 1 << 7,
        BadScheduleTaskTime = 1 << 8,
        BadScheduleTaskPriority = 1 << 9,
        BadScheduleTaskPeriod = 1 << 10,
        BadSelectedDate = 1 << 11,
        TaskNotFound = 1 << 12,
        AccessDenied = 1 << 13
    }
}
