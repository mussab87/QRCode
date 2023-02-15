using Utility.Core.Helpers;
using Utility.Toolkit.Security;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace Utility.Core.Utitlites
{
    public static class QueryParamsExtensions
    {
        public static NameValueCollection GetDecryptedQueryParams(string encryptedParams)
        {
            
            NameValueCollection result = new NameValueCollection();
            try
            {
                var decryptedString = Crypto.DecryptStringDES(HttpUtility.UrlDecode(encryptedParams), CoreConfigurationSettings.EncryptionKey);
                var spiltValues = decryptedString.Split(new string[] { "&", "=" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < spiltValues.Length; i = i + 2)
                {
                    result.Add(spiltValues[i], spiltValues[i + 1]);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            
            return result;
        }

        public static string GetEncryptedQueryParams(NameValueCollection queryParams)
        {                       
            
            Check.Argument.IsNotNull(queryParams,"queryParams");
            Check.Argument.IsNotNegativeOrZero(queryParams.Keys.Count,"queryParams.Keys.Count");
            string encryptedQueryParams = string.Empty;
            try
            {
                StringBuilder strQueryParamBuilder = new StringBuilder();                
                foreach(var key in queryParams.AllKeys)
                {
                    strQueryParamBuilder.AppendFormat("{0}={1}&", key, queryParams[key]);
                }
                strQueryParamBuilder.ToString();
                encryptedQueryParams = HttpUtility.UrlEncode(Crypto.EncryptStringDES(strQueryParamBuilder.ToString(), CoreConfigurationSettings.EncryptionKey));
            }
            catch (Exception)
            {
                
                throw;
            }
            
            return encryptedQueryParams;
        }
    }
}
