using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Errors
{
    public class ErrorMessages
    {
        #region General Error Messages 

        public static readonly string ERR_UNKNOWN = "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        #endregion

        #region Generic - Configuration Error Messages

        public static readonly string ERR_ENDPOINT_NONEXISTANT = "Unable to load the web service configuration. The configured endpoint for this service does not exist ({0}).";

        #endregion

        #region User Management Error Messages

        public static readonly string ERR_DUPE_USERNAME = "This username already exists. Please enter a different user name.";
        public static readonly string ERR_DUPE_EMAIL = "A user with this e-mail address already exists. Please enter a different email address.";
        public static readonly string ERR_INVALID_PASSWORD = "Invalid password entered. Please ensure that your password has a minimum length of 7 characters.";
        public static readonly string ERR_INVALID_EMAIL = "The email provided is invalid. Please check and try again.";
        public static readonly string ERR_INVALID_USERNAME = "The username provided is invalid. Please check the value and try again.";

        public static readonly string ERR_SITE_USERADDED = "The user {0} has already been added to this SharePoint site.";

        public static readonly string ERR_SQLMEMBER_DEL_NOT_EXIST = "Unable to process deletion request. The user {0} does not exist.";
        public static readonly string ERR_SQLMEMBER_DEL_UNKNOWN = "Unable to process deletion request. Unknown error.";

        public static readonly string ERR_DUPE_GROUP = "A group with the name, '{0}' already exists in SharePoint. Please consider renaming the group.";
        public static readonly string ERR_USER_NONEXISTANT = "The user ({0}) does not exist.";
        public static readonly string ERR_USER_AD_NONEXISTANT = "The user ({0}) does not exist in Active Directory.";
        public static readonly string ERR_USER_NOT_IN_GROUP = "The user ({0}) does not belong in this group ({1}).";

        public static readonly string ERR_GROUP_NONEXISTANT = "The group ({0}) does not exist.";
        public static readonly string ERR_GROUP_GET = "Unable to fetch group {0} from SharePoint.";

        public static readonly string ERR_GROUPRELATION_NONEXISTANT = "Unable to find GroupRelation list item with ExternalID {0} and RoleDefinition {1}.";
        public static readonly string ERR_PAGE_NONEXISTANT = "Cannot find page {0} on SharePoint Site Page to grant authority to group. Please contact the System Administrator.";

        #endregion

        #region CRM Access Layer Error Messages

        public static readonly string ERR_CRM_NO_USER_FOUND = "Unable to find user with username {0}.";
        public static readonly string ERR_CRM_NO_USER_FOUND_WITH_ID = "Unable to find user with user id {0}.";
        public static readonly string ERR_CRM_NO_EXTERNAL_ENTITY_FOUND = "Unable to find external entity with key '{0}'.";
        public static readonly string ERR_CRM_CANNOT_CREATE_RECORD = "Unable to create {0} record in CRM.";
        public static readonly string ERR_CRM_NO_RECORD_FOUND = "No record found with ID '{0}' when searching against CRM entity '{1}'";
        public static readonly string ERR_CRM_REQUEST_CONTACT_ALREADY_EXISTS = "This contact has already been attached to this request.";
        public static readonly string ERR_CRM_AGENDA_CANNOT_FIND_SCHEDULED_START_END_DATETIME = "Cannot find the scheduled start and end time for this agenda";

        #endregion

        #region CRM Web Error Messages

        public static readonly string ERR_CRM_WEB_CONFIGURATION_NOT_FOUND = "Unable to correctly load page. The CRM configuration setting ({0}) was not found.";
        public static readonly string ERR_CRM_WEB_QUERY_STRING_NOT_FOUND = "Unable to correctly load page. The query string ({0}) was not found.";
        public static readonly string ERR_CRM_WEB_USER_MISMATCH = "Security autorization failure. Unable to proceed. The current logged on ({0}) user and the CRM user ({1}) do not match.";
        public static readonly string ERR_CRM_WEB_QUERY_STRING_INVALID_TYPE = "Unable to correctly load page. The query string ({0}) is not match the expected type.";

        #endregion

        #region Document Management Error Messages

        public static readonly string ERR_DOC_LIB_NONEXISTANT = "The document library ({0}) does not exist.";

        #endregion

        #region Notification Error Messages

        public static readonly string ERR_NOTIFICATION_SMS_MSG_EXCEEDED_LENGTH = "The SMS message cannot be greater than 5000 characters.";
        public static readonly string ERR_NOTIFICATION_SMS_LOGON_FAILURE = "Unable to logon to SMS wev service.";
        public static readonly string ERR_NOTIFICATION_CAL_DELIVERY_RECEIPT_NOT_EMPTY = "Delivery Receipt should be empty when sending new calendar invitations.";
        public static readonly string ERR_NOTIFICATION_CAL_DELIVERY_RECEIPT_EMPTY = "Delivery Receipt cannot be empty when sending new calendar invitations.";

        #endregion

        #region Active Directory

        public static readonly string ERR_AD_CANNOT_CONNECT = "Unable to connect to Active Directory to retrieve user information for: {0}.";
        public static readonly string ERR_AD_CANNOT_FIND_USER = "Cannot find user in Active Directory with username {0}.";

        #endregion

        #region Profile Management Messages

        public static readonly string ERR_NULL_SESSION_PROFILE = "Unable to retrieve user's profile from the session. Please contact the System Administrator.";

        #endregion

        #region Public Methods

        internal static string ParseErrorTokens(string message, params string[] values)
        {
            string parsedErrorMessage = message;
            try
            {
                parsedErrorMessage = string.Format(message, values);
            }
            catch (Exception)
            {
                throw;
            }
            return parsedErrorMessage;
        }

        #endregion

        #region Image Uploader Methods
        public static readonly string ERR_CRM_WRONG_GUID_FORMAT = "Wrong Guid format with the passed query string user id [{0}].";
        public static readonly string ERR_CRM_WEB_USER_NOT_FOUND = "Can't find a internal user with the passed query string user id [{0}].";
        public static readonly string ERR_CRM_IMAGE_NOT_FOUND = "There is no Image to be uploaded";        
        #endregion

        #region Digital Signatures
        public static readonly string ERR_SIGN_SMART_CARD_MISSING = "There is no smart card available to proceed with signing, please insert a smart card in card reader and retry";     
        #endregion

        #region Office

        public static readonly string ERR_OFFICE_DIR_NOT_EXISTS = "No directory exists at {0}";

        #endregion
    }
}
