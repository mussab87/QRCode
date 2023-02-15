using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Errors
{
    /// <summary>
    /// Category class defining different topics of errors.
    /// </summary>
    internal class ErrorCategories
    {
        #region Systems

        public const int CAT_GENERIC_ERROR = 90000;
        public const int CAT_CRM_ERROR = 10000;
        public const int CAT_SP_ERROR = 20000;
        public const int CAT_NOTIFICATION_ERROR = 30000;
        public const int CAT_AD_ERROR = 40000;
        public const int CAT_PRINTING_ERROR = 50000;
        public const int CAT_UNKNOWN_ERROR = 99999;

        #endregion

        #region Function Categories

        public const int FUNC_CONFIGURATION = 100;

        public const int FUNC_USER_MGMT = 200;
        public const int FUNC_DOC_MGMT = 300;
        public const int FUNC_PROFILE_MANAGEMENT = 400;

        public const int FUNC_SMS = 500;
        public const int FUNC_CAL = 600;

        public const int FUNC_USER_RETRIEVAL = 700;
        public const int FUNC_OFFICE_HTML = 800;

        #endregion
         
        #region CRM Functions

        public const int FUNC_CRM_ACCESSLAYER = 200;
        public const int FUNC_CRM_PLUGINS = 300;
        public const int FUNC_CRM_WEB = 400;
        
        #endregion
    }
}
