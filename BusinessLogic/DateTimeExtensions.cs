using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public static class DateTimeExtensions
    {
        public static DateTime Min(params DateTime[] dates)
        {
            return dates.Min();
        }

        public static DateTime Max(params DateTime[] dates)
        {
            return dates.Max();
        }

        public static DateTime FirstMonthDay(this DateTime value)
        {
            var result = new DateTime(value.Year, value.Month, 1);
            return result;
        }

        public static DateTime LastMonthDay(this DateTime value)
        {
            var result = value.FirstMonthDay().AddMonths(1).AddDays(-1);
            return result;
        }

        public static bool IsHoliday(this DateTime value)
        {
            return (value.DayOfWeek == DayOfWeek.Saturday || value.DayOfWeek == DayOfWeek.Sunday);
        }

        public static int DifferenceInFullDays(this DateTime later, DateTime before)
        {
            return (int)((later - before).TotalDays);
        }


    }
}
