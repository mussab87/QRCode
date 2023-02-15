using System;
using System.Collections.Generic;
using System.Text;

namespace RMS.Core.Utitlites.Configuration
{
    public class PrimaryPublication
    {
        #region CRM Environment Specific Configuration Keys
        
        public static readonly string KeyCRMServiceURL = "CRMServiceWebServiceEndPoint";
        public static readonly string KeyMetaDataServiceURL = "CRMMetabaseWebServiceEndPoint";
        
        public static readonly string KeyOrgName = "CRMOrgName";
        public static readonly string KeySDKDomain = "CRMSDKDomain";
        public static readonly string KeySDKUsername = "CRMSDKUsername";
        public static readonly string KeySDKPassword = "CRMSDKPassword";

        public static readonly string KeyPicklistNames = "CRMPicklistNames";

        #endregion
    }
}
