using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TaskScheduler.Database.Tests
{
    static class CommonTests
    {
        public const string TEST_DATA_FOLDER = "TestData";

        public const string LOGIN_AUTH_REGISTER_FILENAME = "TestData\\LoginAuthRegisterModelInValidConfirmPasswordData.json";

        public const string REPOSITORY_DATA_FNAME = "TestData\\RepositoryData.json";

        public const string SCHEDULE_TASK_CREATE_MODEL_FNAME = "TestData\\ScheduleTaskCreateModelValidData.json";

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

        public static IEnumerable<(DateTime dateStart, DateTime dateEnd)> GetExistedDatePeriods()
        {
            yield return (new DateTime(2022, 1, 1), new DateTime(2023, 1, 1));
            yield return (new DateTime(2022, 6, 1), new DateTime(2022, 6, 15));
            yield return (new DateTime(2022, 6, 15), new DateTime(2022, 6, 30));
            yield return (new DateTime(2022, 4, 15), new DateTime(2022, 8, 15));
            yield return (new DateTime(2022, 3, 13), new DateTime(2022, 9, 25));
            yield return (new DateTime(2022, 11, 5), new DateTime(2022, 12, 7));
            yield return (new DateTime(2022, 2, 26), new DateTime(2022, 5, 9));
        }

        public static IEnumerable<(DateTime dateStart, DateTime dateEnd)> GetNotExistedDatePeriods()
        {
            yield return (new DateTime(2023, 1, 1), new DateTime(2022, 1, 1));
            yield return (new DateTime(2022, 6, 1), new DateTime(2022, 5, 1));
            yield return (new DateTime(2023, 2, 1), new DateTime(2024, 1, 1));
            yield return (new DateTime(2021, 1, 1), new DateTime(2021, 12, 1));
            yield return (new DateTime(2021, 1, 1), new DateTime(2021, 12, 1));
            yield return (new DateTime(2022, 1, 1), new DateTime(2022, 1, 1, 15, 0, 0));
        }
    }
}
