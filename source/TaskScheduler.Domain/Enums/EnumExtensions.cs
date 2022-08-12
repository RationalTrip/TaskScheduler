using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler.Domain
{
    public static class EnumExtensions
    {
        public static DateTime AddScheduleTaskPeriod(this DateTime date, TaskRepetitivePeriod period, int periodsToAddCount = 1) =>
            period switch
            {
                TaskRepetitivePeriod.Daily => date.AddDays(1 * periodsToAddCount),
                TaskRepetitivePeriod.Weekly => date.AddDays(7 * periodsToAddCount),
                TaskRepetitivePeriod.Monthly => date.AddMonths(1 * periodsToAddCount),
                TaskRepetitivePeriod.Yearly => date.AddYears(1 * periodsToAddCount),
                _ => throw new ArgumentOutOfRangeException($"Such period \"{period}\" not exists!")
            };

        public static int PeriodsCountBetweenDates(this TaskRepetitivePeriod period, DateTime dateStart, DateTime dateEnd) =>
            period switch
            {
                TaskRepetitivePeriod.Daily => (int)(dateEnd - dateStart).TotalDays,
                TaskRepetitivePeriod.Weekly => ((int)(dateEnd - dateStart).TotalDays) / 7,
                TaskRepetitivePeriod.Monthly => MonthsBetweenDates(dateStart, dateEnd),
                TaskRepetitivePeriod.Yearly => YearsBetweenDates(dateStart, dateEnd),
                _ => throw new ArgumentOutOfRangeException($"Such period \"{period}\" not exists!")
            };

        static int MonthsBetweenDates(DateTime dateStart, DateTime dateEnd)
        {
            int months = (dateEnd.Year - dateStart.Year) * 12 + dateEnd.Month - dateStart.Month;

            if (dateStart.Day > dateEnd.Day)
                return months - 1;

            return dateStart.TimeOfDay > dateEnd.TimeOfDay ? months - 1 : months;
        }

        static int YearsBetweenDates(DateTime dateStart, DateTime dateEnd)
        {
            int years = dateEnd.Year - dateStart.Year;

            return dateStart.AddYears(years) > dateEnd ? years - 1 : years;
        }
    }
}
