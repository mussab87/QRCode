using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Helpers
{
    public class MobileUnformatter
    {
        public static string StripMobileCharacters(string mobile)
        {
            var cleanedMobile = mobile;
            mobile = mobile.Replace("00", string.Empty);
            mobile = mobile.Replace("+", string.Empty);
            mobile = mobile.Replace("-", string.Empty);
            mobile = mobile.Replace(" ", string.Empty);
            mobile = mobile.Replace(",", string.Empty);
            mobile = mobile.Replace("(", string.Empty);
            mobile = mobile.Replace(")", string.Empty);
            mobile = mobile.Replace("#", string.Empty);

            cleanedMobile = String.Join("", mobile.Where(c => !char.IsWhiteSpace(c)));

            return cleanedMobile;
        }
    }
}
