using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool IsDate(this string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                DateTime dt;
                var parsed = DateTime.TryParseExact(input, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);

                return parsed;
            }
            else
            {
                return false;
            }
        }

        public static bool IsWeekend(this DateTime value)
        {
            return (value.DayOfWeek == DayOfWeek.Sunday || value.DayOfWeek == DayOfWeek.Saturday);
        }

        public static DateTime? ToDateTime(this string s)
        {
            DateTime dt;
            var parsed = DateTime.TryParseExact(s, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
            if (parsed)
                return dt;
            else
                return new DateTime?();
        }

        public static bool IsDateTime(this Type type)
        {
            return type.Equals(typeof(DateTime));
        }

        public static string NullDateToString(this DateTime? dt, string format = "M/d/yyyy", string nullResult = "")
        {
            if (dt.HasValue)
                return dt.Value.ToString(format);
            else
                return nullResult;
        }

        public static DateTime AddTime(this DateTime date, int hour, int minutes)
        {
            return date + new TimeSpan(hour, minutes, 0);
        }

        public static bool EqualsUpToSeconds(this DateTime dt1, DateTime dt2)
        {
            return dt1.Year == dt2.Year && dt1.Month == dt2.Month && dt1.Day == dt2.Day &&
                   dt1.Hour == dt2.Hour && dt1.Minute == dt2.Minute && dt1.Second == dt2.Second;
        }

        public static bool EqualsUpToMinutes(this DateTime dt1, DateTime dt2)
        {
            return dt1.Year == dt2.Year && dt1.Month == dt2.Month && dt1.Day == dt2.Day &&
                   dt1.Hour == dt2.Hour && dt1.Minute == dt2.Minute;
        }

        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
    }
}