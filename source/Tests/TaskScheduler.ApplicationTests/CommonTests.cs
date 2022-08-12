using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using System;

namespace TaskScheduler.Application.Tests
{
    static class CommonTests
    {
        public const string TEST_DATA_FOLDER = "TestData";

        public const string LOGIN_AUTH_REGISTER_INVALID_CONFIRM_PASSWORD_FNAME =    "TestData\\LoginAuthRegisterModelInValidConfirmPasswordData.json";
        public const string LOGIN_AUTH_REGISTER_INVALID_LOGIN_AND_PASSWORD_FNAME =  "TestData\\LoginAuthRegisterModelInValidLoginAndPasswordData.json";
        public const string LOGIN_AUTH_REGISTER_INVALID_LOGIN_FNAME =       "TestData\\LoginAuthRegisterModelInValidLoginData.json";
        public const string LOGIN_AUTH_REGISTER_INVALID_PASSWORD_FNAME =    "TestData\\LoginAuthRegisterModelInValidPasswordData.json";
        public const string LOGIN_AUTH_REGISTER_VALID__FNAME =     "TestData\\LoginAuthRegisterModelValidData.json";

        public const string LOGIN_AUTH_SIGNIN_INVALID_LOGIN_AND_PASSWORD_FNAME = "TestData\\LoginAuthSignInModelInValidLoginAndPasswordData.json";
        public const string LOGIN_AUTH_SIGNIN_INVALID_LOGIN_FNAME =    "TestData\\LoginAuthSignInModelInValidLoginData.json";
        public const string LOGIN_AUTH_SIGNIN_INVALID_PASSWORD_FNAME = "TestData\\LoginAuthSignInModelInValidPasswordData.json";
        public const string LOGIN_AUTH_SIGNIN_VALID_FNAME =            "TestData\\LoginAuthSignInModelValidData.json";

        public const string SCHEDULE_TASK_CREATE_INVALID_PERIOD_FNAME = "TestData\\ScheduleTaskCreateModelInValidPeriod.json";
        public const string SCHEDULE_TASK_CREATE_INVALID_PRIORITY_FNAME =               "TestData\\ScheduleTaskCreateModelInValidPriorityData.json";
        public const string SCHEDULE_TASK_CREATE_INVALID_START_REPETITIVE_TIME_FNAME =  "TestData\\ScheduleTaskCreateModelInValidStartRepetitiveTimeData.json";
        public const string SCHEDULE_TASK_CREATE_INVALID_START_TIME_FNAME =     "TestData\\ScheduleTaskCreateModelInValidStartTimeData.json";
        public const string SCHEDULE_TASK_CREATE_INVALID_TITLE_FNAME =          "TestData\\ScheduleTaskCreateModelInValidTitleData.json";
        public const string SCHEDULE_TASK_CREATE_VALID_FNAME =  "TestData\\ScheduleTaskCreateModelValidData.json";

        public const string SCHEDULE_TASK_LINK_VALID_FNAME = "TestData\\ScheduleTaskLinkValidData.json";
        public const string SCHEDULE_TASK_LINK_INVALID_FNAME = "TestData\\ScheduleTaskLinkInValidData.json";
        public static IEnumerable<T> DeserializeJson<T>(string jsonFName)
        {
            string jsonString = File.ReadAllText(jsonFName);

            var options = new JsonSerializerOptions
            {
                IgnoreReadOnlyProperties = true,
                WriteIndented = true
            };

            return JsonSerializer.Deserialize<T[]>(jsonString, options);
        }
        public static IEnumerable<object[]> JsonFileToMethodArgument<TArg>(string jsonFName) => 
            DeserializeJson<TArg>(jsonFName).Select(arg => new object[] { arg });


        public static IEnumerable<int> GetValidYear()
        {
            for (int year = 1900; year < 2200; year++)
                yield return year;
        }
        public static IEnumerable<int> GetInValidYear()
        {
            yield return 0;
            yield return -5;
            yield return -10;
            yield return -20;

            yield return 10000;
            yield return 10111;
        }
        public static IEnumerable<(int year, int month)> GetValidMonth()
        {
            var dateEnd = new DateTime(2027, 1, 1);
            var dateStart = new DateTime(2022, 1, 1);

            for (var date = dateStart; date < dateEnd; date = date.AddMonths(1))
                yield return (date.Year, date.Month);
        }
        public static IEnumerable<(int year, int month)> GetInValidMonth()
        {
            yield return (-5, 11);
            yield return (-10, 13);
            yield return (-20, 5);

            yield return (10000, 9);
            yield return (10000, -5);

            yield return (2021, 0);
            yield return (2025, -15);
            yield return (2022, 13);
            yield return (2021, -2);
            yield return (2025, 22);
            yield return (2022, 98);
        }
        public static IEnumerable<(int year, int month, int day)> GetValidDay()
        {
            var dateEnd = new DateTime(2025, 1, 1);
            var dateStart = new DateTime(2024, 1, 1);

            for (var date = dateStart; date < dateEnd; date = date.AddDays(1))
                yield return (date.Year, date.Month, date.Day);
        }
        public static IEnumerable<(int year, int month, int day)> GetInValidDay()
        {
            yield return (-5, 11, 15);
            yield return (-10, 13, 35);
            yield return (-20, 5, 3);

            yield return (10000, 9, -5);
            yield return (10000, -5, 8);

            yield return (2021, 0, 25);
            yield return (2025, -15, -6);
            yield return (2022, 13, 6);

            yield return (2022, 3, -5);
            yield return (2022, 8, 0);
            yield return (2022, 1, -15);

            yield return (2022, 2, 30);
            yield return (2022, 4, 31);
            yield return (2022, 6, 31);
            yield return (2022, 9, 31);
            yield return (2022, 11, 31);

            yield return (2021, 2, 29);
            yield return (2022, 2, 29);
            yield return (2023, 2, 29);
        }
    }
}
