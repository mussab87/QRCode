using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Security.Cryptography;
using System.Web;

namespace Utility.Core.Utitlites
{
    public static class TimeSpanExtension
    {
        public static DateTime TimeSpanToDate(this TimeSpan target)
        {
            return target.TimeSpanToDate(false);
        }

        public static DateTime TimeSpanToDate(this TimeSpan target, bool nextDayIfDateInPast)
        {
            var date = DateTime.Today;
            date = date.AddHours(target.Hours);
            date = date.AddMinutes(target.Minutes);

            if (nextDayIfDateInPast && date < DateTime.Now)
                    date = date.AddDays(1);
            return date;
        }

        public static DateTime TimeSpanToDate(this TimeSpan target, DateTime dateToSetTimeWith)
        {
            var date = dateToSetTimeWith;
            date = date.AddHours(target.Hours);
            date = date.AddMinutes(target.Minutes);

            return date;
        }
    }
}