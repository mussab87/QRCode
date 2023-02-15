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
    public static class StringExtension
    {
        private static readonly Regex WebUrlExpression = new Regex(@"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex EmailExpression = new Regex(@"^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex StripHTMLExpression = new Regex("<\\S[^><]*>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private static readonly char[] IllegalUrlCharacters = new[] { ';', '/', '\\', '?', ':', '@', '&', '=', '+', '$', ',', '<', '>', '#', '%', '.', '!', '*', '\'', '"', '(', ')', '[', ']', '{', '}', '|', '^', '`', '~', '–', '‘', '’', '“', '”', '»', '«' };
        
        public static bool IsWebUrl(this string target)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(target, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }

        public static bool IsEmail(this string target)
        {
            return !string.IsNullOrEmpty(target) && EmailExpression.IsMatch(target);
        }
        
        public static string NullSafe(this string target)
        {
            return (target ?? string.Empty).Trim();
        }
        
        public static string FormatWith(this string target, params object[] args)
        {
            Check.Argument.IsNotEmpty(target, "target");

            return string.Format(CultureInfo.CurrentCulture, target, args);
        }
        
        public static string Hash(this string target)
        {
            Check.Argument.IsNotEmpty(target, "target");

            using (MD5 md5 = MD5.Create())
            {
                byte[] data = Encoding.Unicode.GetBytes(target);
                byte[] hash = md5.ComputeHash(data);

                return Convert.ToBase64String(hash);
            }
        }
        
        public static string WrapAt(this string target, int index)
        {
            const int DotCount = 3;

            Check.Argument.IsNotEmpty(target, "target");
            Check.Argument.IsNotNegativeOrZero(index, "index");

            return (target.Length <= index) ? target : string.Concat(target.Substring(0, index - DotCount), new string('.', DotCount));
        }
        
        public static string StripHtml(this string target)
        {
            return StripHTMLExpression.Replace(target, string.Empty);
        }
        
        public static Guid ToGuid(this string target)
        {
            Guid result = Guid.Empty;

            if ((!string.IsNullOrEmpty(target)) && (target.Trim().Length == 22))
            {
                string encoded = string.Concat(target.Trim().Replace("-", "+").Replace("_", "/"), "==");

                try
                {
                    byte[] base64 = Convert.FromBase64String(encoded);

                    result = new Guid(base64);
                }
                catch (FormatException)
                {
                }
            }

            return result;
        }

        public static T ToEnum<T>(this string target, T defaultValue) where T : IComparable, IFormattable
        {
            T convertedValue = defaultValue;

            if (!string.IsNullOrEmpty(target))
            {
                try
                {
                    convertedValue = (T)Enum.Parse(typeof(T), target.Trim(), true);
                }
                catch (ArgumentException)
                {
                }
            }

            return convertedValue;
        }

        public static string ToLegalUrl(this string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                return target;
            }

            target = target.Trim();

            if (target.IndexOfAny(IllegalUrlCharacters) > -1)
            {
                foreach (char character in IllegalUrlCharacters)
                {
                    target = target.Replace(character.ToString(CultureInfo.CurrentCulture), string.Empty);
                }
            }

            target = target.Replace(" ", "-");

            while (target.Contains("--"))
            {
                target = target.Replace("--", "-");
            }

            return target;
        }
        
        public static string UrlEncode(this string target)
        {
            return HttpUtility.UrlEncode(target);
        }
        
        public static string UrlDecode(this string target)
        {
            return HttpUtility.UrlDecode(target);
        }
        
        public static string AttributeEncode(this string target)
        {
            return HttpUtility.HtmlAttributeEncode(target);
        }
        
        public static string HtmlEncode(this string target)
        {
            return HttpUtility.HtmlEncode(target);
        }

        public static string HtmlDecode(this string target)
        {
            return HttpUtility.HtmlDecode(target);
        }

        public static TimeSpan? Parse12HourTimeString(this string target)
        {
            TimeSpan? parsed = null;
            try
            {
                DateTime t = DateTime.Now;
                if (target.Length == 7)
                    t = DateTime.ParseExact(target, "h:mm tt", CultureInfo.InvariantCulture);
                else if (target.Length == 8)
                    t = DateTime.ParseExact(target, "h:mm tt", CultureInfo.InvariantCulture);
                else
                    throw new Exception(string.Format("Unable to parse '{0}' to TimeSpan", target));

                parsed = t.TimeOfDay;
            }
            catch (Exception)
            {
                throw;
            }
            return parsed;
        }

        public static string ParseTimeSpanToString(this TimeSpan target)
        {
            var hours = target.Hours;
            var minutes = target.Minutes;
            var amPmDesignator = "AM";
            if (hours == 0)
                hours = 12;
            else if (hours == 12)
                amPmDesignator = "PM";
            else if (hours > 12)
            {
                hours -= 12;
                amPmDesignator = "PM";
            }
            var formattedTime = String.Format("{0}:{1:00} {2}", hours, minutes, amPmDesignator);
            return formattedTime;
        }

        public static byte[] HexStringToByteArray(this string target)
        {
            int NumberChars = target.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(target.Substring(i, 2), 16);
            
            return bytes;
        }

        public static bool ToBoolean(this string s)
        {
            string[] trueStrings = { "1", "y", "yes", "true" };
            string[] falseStrings = { "0", "n", "no", "false" };

            if (trueStrings.Contains(s.ToLower()))
                return true;
            if (falseStrings.Contains(s.ToLower()))
                return false;

            throw new InvalidCastException("only the following are supported for converting strings to boolean: "
                + string.Join(",", trueStrings)
                + " and "
                + string.Join(",", falseStrings));
        }

        public static string ToWesternArabicNumerals(this string target)
        {
            var result = new StringBuilder(target.Length);

            foreach (char c in target.ToCharArray())
            {
                //Check if the characters is recognized as UNICODE numeric value if yes
                if (char.IsNumber(c))
                {
                    // using char.GetNumericValue() convert numeric Unicode to a double-precision 
                    // floating point number (returns the numeric value of the passed char)
                    // apend to final string holder
                    result.Append(char.GetNumericValue(c));
                }
                else
                {
                    // apend non numeric chars to recreate the orignal string with the converted numbers
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        public static string ReplaceSpecialCharacters(this string target, string replaceValue)
        {
            return Regex.Replace(target, "[^a-zA-Z0-9_.]+", replaceValue, RegexOptions.Compiled);
        }

        public static string ReplaceMoreThanOneSpaceWithSingleSpace(this string target)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            return regex.Replace(target, " ");
        }
    }
}