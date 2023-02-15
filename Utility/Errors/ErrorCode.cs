using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Utility.Errors
{
    [DataContract]
    [Flags]
    public enum ErrorCodes
    {
        [EnumMember]
        Empty = 0,

        [EnumMember]
        ERR_UNKNOWN = 0,

        #region General
        [EnumMember]
        // Unable to load the web service configuration. The configured endpoint for this service does not exist ({0}).
        ERR_ENDPOINT_NONEXISTANT = ErrorCategories.CAT_GENERIC_ERROR + ErrorCategories.FUNC_CONFIGURATION + 1,

        ERR_CONFIGURATION_VALUE_REQUIRED = ErrorCategories.CAT_GENERIC_ERROR + ErrorCategories.FUNC_CONFIGURATION + 2,

        ERR_INVALID_QUERY_PARAMS = ErrorCategories.CAT_GENERIC_ERROR + ErrorCategories.FUNC_CONFIGURATION + 3,        

        #endregion
        
        #region User Management

        // This username already exists. Please enter a different user name.
        [EnumMember]
        ERR_DUPE_USERNAME = ErrorCategories.CAT_SP_ERROR + ErrorCategories.FUNC_USER_MGMT + 1,

        // A user with this e-mail address already exists. Please enter a different email address.
        [EnumMember]
        ERR_DUPE_EMAIL = ErrorCategories.CAT_SP_ERROR + ErrorCategories.FUNC_USER_MGMT + 2,

        // Invalid password entered. Please ensure that your password has a minimum length of 7 characters.
        [EnumMember]
        ERR_INVALID_PASSWORD = ErrorCategories.CAT_SP_ERROR + ErrorCategories.FUNC_USER_MGMT + 3,

        // The email provided is invalid. Please check and try again.
        [EnumMember]
        ERR_INVALID_EMAIL = ErrorCategories.CAT_SP_ERROR + ErrorCategories.FUNC_USER_MGMT + 4,

        // The username provided is invalid. Please check the value and try again.
        [EnumMember]
        ERR_INVALID_USERNAME = ErrorCategories.CAT_SP_ERROR + ErrorCategories.FUNC_USER_MGMT + 5,

        // The user (%v) has already been added to this SharePoint site.
        [EnumMember]
        ERR_SITE_USERADDED = ErrorCategories.CAT_SP_ERROR + ErrorCategories.FUNC_USER_MGMT + 6,

        // Unable to process deletion request. The user (%v) does not exist.
        [EnumMember]
        ERR_SQLMEMBER_DEL_NOT_EXIST = ErrorCategories.CAT_SP_ERROR + ErrorCategories.FUNC_USER_MGMT + 7,

        // A group with the name, '{0}' already exists in SharePoint. Please consider renaming the group
        [EnumMember]
        ERR_DUPE_GROUP = ErrorCategories.CAT_SP_ERROR + ErrorCategories.FUNC_USER_MGMT + 8,

        // The user ({0}) does not exist.
        [EnumMember]
        ERR_USER_NONEXISTANT = ErrorCategories.CAT_SP_ERROR + ErrorCategories.FUNC_USER_MGMT + 9,

        // The group ({0}) does not exist.
        [EnumMember]
        ERR_GROUP_NONEXISTANT = ErrorCategories.CAT_SP_ERROR + ErrorCategories.FUNC_USER_MGMT + 10,

        // Unable to fetch group {0} from SharePoint
        [EnumMember]
        ERR_GROUP_GET = ErrorCategories.CAT_SP_ERROR + ErrorCategories.FUNC_USER_MGMT + 11,

        // The user ({0}) does not belong in this group ({1})
        [EnumMember]
        ERR_USER_NOT_IN_GROUP = ErrorCategories.CAT_SP_ERROR + ErrorCategories.FUNC_USER_MGMT + 12,

        // The user ({0}) does not exist in Active Directory
        [EnumMember]
        ERR_USER_AD_NONEXISTANT = ErrorCategories.CAT_SP_ERROR + ErrorCategories.FUNC_USER_MGMT + 13,

        // Unable to find GroupRelation list item with ExternalID {0} and RoleDefinition {1}.
        [EnumMember]
        ERR_GROUPRELATION_NONEXISTANT = ErrorCategories.CAT_SP_ERROR + ErrorCategories.FUNC_USER_MGMT + 14,

        [EnumMember]
        ERR_PAGE_NONEXISTANT = ErrorCategories.CAT_SP_ERROR + ErrorCategories.FUNC_USER_MGMT + 15,

        // Unable to process deletion request for username (%v) (Unknown error).
        [EnumMember]
        ERR_SQLMEMBER_DEL_UNKNOWN = ErrorCategories.CAT_UNKNOWN_ERROR,

        #endregion

        #region Document Management

        [EnumMember]
        ERR_DOC_LIB_NONEXISTANT = ErrorCategories.CAT_SP_ERROR + ErrorCategories.FUNC_DOC_MGMT + 1,

        #endregion

        #region CRM Access Layer / Extensions
                       
        [EnumMember]
        ERR_CRM_PLUGIN_BUSINESS_FAULT = ErrorCategories.CAT_CRM_ERROR + 1111,

        [EnumMember]
        ERR_CRM_PLUGIN_INVALID_REGISTRATION = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_PLUGINS + 1,

        [EnumMember]
        ERR_CRM_WORKFLOW_ENTITY_MODEL_NOT_IMPLEMENT_IWORKFLOW = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_PLUGINS + 2,

        [EnumMember]
        ERR_CRM_NO_ADMIN_FOUND = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_ACCESSLAYER + 1,

        [EnumMember]
        ERR_CRM_NO_REQUEST_SP_SITE_FOUND = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_ACCESSLAYER + 2,

        [EnumMember]
        ERR_CRM_NO_USER_FOUND = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_ACCESSLAYER + 3,

        [EnumMember]
        ERR_CRM_NO_EXTERNAL_ENTITY_FOUND = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_ACCESSLAYER + 4,

        [EnumMember]
        ERR_CRM_CANNOT_CREATE_RECORD = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_ACCESSLAYER + 5,

        [EnumMember]
        ERR_CRM_NO_RECORD_FOUND = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_ACCESSLAYER + 6,

        [EnumMember]
        ERR_CRM_INVALID_PARAMETER_VALUE = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_ACCESSLAYER + 7,

        [EnumMember]
        ERR_CRM_REQUEST_CONTACT_ALREADY_EXISTS = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_ACCESSLAYER + 8,

        [EnumMember]
        ERR_CRM_AGENDA_CANNOT_FIND_SCHEDULED_START_END_DATETIME = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_ACCESSLAYER + 9,

        [EnumMember]
        ERR_CRM_ACTION_IS_VALID_IN_THIS_STATUS = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_ACCESSLAYER + 10,

        [EnumMember]
        ERR_CRM_INVALID_AGENDA_ITEM_DECISION = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_ACCESSLAYER + 11,

        [EnumMember]
        ERR_CRM_ATTRIBUTE_NOT_FOUND = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_ACCESSLAYER + 12,

        [EnumMember]
        ERR_CRM_USER_SHOULD_BE_INTERNAL = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_ACCESSLAYER + 13,

        [EnumMember]
        ERR_CRM_USER_ENTITY_CANNOT_BE_NULL = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_ACCESSLAYER + 14,

        [EnumMember]
        ERR_CRM_USER_IS_NOT_AUTHORIZED_TO_CREATE_REQUEST = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_ACCESSLAYER + 15,

        [EnumMember]
        ERR_CRM_INVALID_CONFIGURATION_VALUE = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_ACCESSLAYER + 16,

        [EnumMember]
        ERR_CRM_CONFIGURATION_VALUE_REQUIRED = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_ACCESSLAYER + 17,

        [EnumMember]
        ERR_CRM_ONLY_INTERNAL_USERS_IS_AUTHORIZED_TO_START_AGENDA = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_ACCESSLAYER + 18,
        #endregion

        #region CRM Web

        [EnumMember]
        ERR_CRM_WEB_QUERY_STRING_NOT_FOUND = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_WEB + 1,

        [EnumMember]
        ERR_CRM_WEB_USER_MISMATCH = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_WEB + 2,

        [EnumMember]
        ERR_CRM_WEB_USER_NOT_FOUND = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_WEB + 3,

        [EnumMember]
        ERR_CRM_IMAGE_NOT_FOUND = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_WEB + 4,

        [EnumMember]
        ERR_CRM_WRONG_GUID_FORMAT = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_WEB + 5,

        [EnumMember]
        ERR_CRM_WRONG_TYPE_FORMAT = ErrorCategories.CAT_CRM_ERROR + ErrorCategories.FUNC_CRM_WEB + 6,
        
        #endregion

        #region Notifications

        [EnumMember]
        ERR_NOTIFICATION_SMS_MSG_EXCEEDED_LENGTH = ErrorCategories.CAT_NOTIFICATION_ERROR + ErrorCategories.FUNC_SMS + 1,

        [EnumMember]
        ERR_NOTIFICATION_SMS_LOGON_FAILURE = ErrorCategories.CAT_NOTIFICATION_ERROR + ErrorCategories.FUNC_SMS + 2,

        [EnumMember]
        ERR_NOTIFICATION_CAL_DELIVERY_RECEIPT_NOT_EMPTY = ErrorCategories.CAT_NOTIFICATION_ERROR + ErrorCategories.FUNC_CAL + 3,

        [EnumMember]
        ERR_NOTIFICATION_CAL_DELIVERY_RECEIPT_EMPTY = ErrorCategories.CAT_NOTIFICATION_ERROR + ErrorCategories.FUNC_CAL + 4,

        [EnumMember]
        ERR_NOTIFICATION_CANNOT_FIND_TEMPLATE = ErrorCategories.CAT_NOTIFICATION_ERROR + ErrorCategories.FUNC_CAL + 5,

        #endregion

        #region Active Directory

        [EnumMember]
        ERR_AD_CANNOT_CONNECT = ErrorCategories.CAT_AD_ERROR + ErrorCategories.FUNC_USER_RETRIEVAL + 1,

        [EnumMember]
        ERR_AD_CANNOT_FIND_USER = ErrorCategories.CAT_AD_ERROR + ErrorCategories.FUNC_USER_RETRIEVAL + 1,

        #endregion

        #region Profile Management

        [EnumMember]
        ERR_NULL_SESSION_PROFILE = ErrorCategories.CAT_SP_ERROR + ErrorCategories.FUNC_PROFILE_MANAGEMENT + 1,

        #endregion

        #region Printing

        [EnumMember]
        ERR_OFFICE_DIR_NOT_EXISTS = ErrorCategories.CAT_PRINTING_ERROR + ErrorCategories.FUNC_PROFILE_MANAGEMENT + 1,

        #endregion
    }
}
