using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Core.Utitlites
{
    public static class GuidExtensions
    {
        public static bool IsNullOrEmpty(Guid guid)
        {
            if (guid == null || guid == Guid.Empty)
            {
                return true;
            }
            return false;
        }

        public static bool IsValid(string strValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(strValue))
                {
                    var value = new Guid(strValue);
                    return true;
                }
            }
            catch (FormatException)
            {
            }            
            return false;
        }

        public static bool TryParse(string strValue, out Guid value)
        {
            try
            {
                if (!string.IsNullOrEmpty(strValue))
                {
                    value = new Guid(strValue);
                    return true;
                }
            }
            catch (FormatException)
            {
            }
            value = Guid.Empty;
            return false;
        }
    }
}
