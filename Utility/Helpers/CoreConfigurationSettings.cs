using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

using Utility.Core.Caching;
using Utility.Core.Utitlites;
using Utility.Errors;

namespace Utility.Core.Helpers
{
    public static class CoreConfigurationSettings
    {
        public static int UploaderMaxFileSize
        { 
            get
            {
                int maxFileSize = 20971520;
                try
                {
                    string maxFileSizeRaw = ConfigurationManager.AppSettings["Uploader Max File Size"];
                    if (!string.IsNullOrEmpty(maxFileSizeRaw))
                    {
                        Int32.TryParse(maxFileSizeRaw, out maxFileSize);
                    }
                }
                catch (Exception)
                {
                }
                return maxFileSize;
            }
        }

        public static string[] UploaderAllowedFileExtensions
        {
            get
            {
                string[] extensions = new string[] { ".jpg", ".jpeg", ".gif", ".tif", ".bmp", ".ico", ".ppm", ".psd", ".png", ".tga", ".eps", ".wpg", ".pdf", ".img", ".autocad", ".docx", ".xlsx", ".pptx", ".rtf" };

                try
                {
                    string extensionsRaw = ConfigurationManager.AppSettings["Uploader Allowed File Extensions"];
                    if (!string.IsNullOrEmpty(extensionsRaw))
                        extensions = extensionsRaw.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                }
                catch (Exception)
                {
                }
                return extensions;
            }
        }

        public static string TemporaryFolder {
            get {
                return @"C:\Temp\UploadControl\UploadImages";
            } 
        }

        public static string EncryptionKey
        {
            get
            {
                return "Encrypti0n@K6y";
            }
        }           

        public static string CRMServiceUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["CRMServiceUrl"];
            }
        }

        public static string CRMServiceUser
        {
            get
            {
                return ConfigurationManager.AppSettings["CRMServiceUser"];
            }
        }

        public static string CRMServicePassword
        {
            get
            {
                var encryptedPassword = ConfigurationManager.AppSettings["CRMServicePassword"];
                if (string.IsNullOrEmpty(encryptedPassword))
                {
                    throw new GenException(ErrorCodes.ERR_CONFIGURATION_VALUE_REQUIRED, "Can't find configuration value [{0}] in the config file", "CRMServicePassword");
                }
                string password = Utility.Toolkit.Security.Crypto.DecryptStringDES(encryptedPassword, EncryptionKey);
                return password;
            }
        }

        public static string CRMDomain
        {
            get
            {
                return ConfigurationManager.AppSettings["CRMDomain"];
            }
        }

        public static Guid CRMAdministratorId
        {
            get
            {
                Guid cRMAdministratorId;
                var strValue = ConfigurationManager.AppSettings["CRMAdministratorId"];
                if (!GuidExtensions.TryParse(strValue, out cRMAdministratorId))
                {
                    throw new Exception("CRMAdministratorId configuration value shouldn't be empty and should have a valid Guid format.");
                }
                return cRMAdministratorId;
            }
        }        
    }
}
