using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using System.Collections;
using System.DirectoryServices.AccountManagement;
using Utility.Core.Utitlites;
using Utility.Errors;
using Utility.Core.Logging;

namespace Utility.Core.ActiveDirectory
{
    public class ADManager
    {
        #region Fields

        private DirectoryEntry LDAPConnection;

        #endregion

        #region Ctor

        public ADManager()
        {

        }

        public ADManager(ConnectionDetails connectionDetail)
        {
            
            try
            {
                if (connectionDetail != null)
                {
                    LDAPConnection = CreateDirectoryEntry(connectionDetail);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            
        }

        #endregion

        #region User Details

        public Hashtable RetrieveUserDetails(string usernameToRetrieve, params string[] ldapFields)
        {
            
            Hashtable retrievedDetails = null;
            try
            {
                retrievedDetails = RetrieveUserDetails(LDAPConnection, usernameToRetrieve, ldapFields);
            }
            catch (Exception)
            {
                
                throw;
            }

            
            return retrievedDetails;
        }

        public static Hashtable RetrieveUserDetails(ConnectionDetails connectionDetail, string usernameToRetrieve, params string[] ldapFields)
        {
            
            Hashtable retrievedDetails = null;
            try
            {
                // Create the LDAP connection
                DirectoryEntry connection = CreateDirectoryEntry(connectionDetail);
                if (connection == null)
                {
                    throw new GenException(ErrorCodes.ERR_AD_CANNOT_CONNECT, ErrorMessages.ERR_AD_CANNOT_CONNECT, usernameToRetrieve);
                }
                retrievedDetails = RetrieveUserDetails(connection, usernameToRetrieve, ldapFields);
            }
            catch (Exception)
            {
                
                throw;
            }

            
            return retrievedDetails;
        }

        private static Hashtable RetrieveUserDetails(DirectoryEntry connection, string usernameToRetrieve, params string[] ldapFields)
        {
            
            Hashtable retrievedDetails = null;
            try
            {                
                // Strip the username if it is present
                if (usernameToRetrieve.Contains("\\"))
                {
                    usernameToRetrieve = usernameToRetrieve.Substring(usernameToRetrieve.LastIndexOf("\\") + 1);
                }
                else if (usernameToRetrieve.Contains("@"))
                {
                    usernameToRetrieve = usernameToRetrieve.Substring(0, usernameToRetrieve.IndexOf("@"));
                }

                // Create search object which operates on LDAP connection object
                // and set search object to only find the user specified
                DirectorySearcher search = new DirectorySearcher(connection);
                search.Filter = "(&((&(objectCategory=Person)(objectClass=User)))(samaccountname=" + usernameToRetrieve + "))";
                search.SearchScope = SearchScope.Subtree;

                // Create results objects from search object
                SearchResult result = search.FindOne();
                if (result == null)
                {
                    throw new GenException(ErrorCodes.ERR_AD_CANNOT_FIND_USER, ErrorMessages.ERR_AD_CANNOT_FIND_USER, usernameToRetrieve);
                }

                retrievedDetails = new Hashtable();

                // Retrieve the user's attributes
                ResultPropertyCollection fields = result.Properties;
                foreach (string key in ldapFields)
                {
                    // Fetch the details from AD
                    if (key != null)
                    {
                        string value = string.Empty;
                        if (fields[key] != null && fields[key].Count > 0)
                        {
                            value = fields[key][0].ToString();
                        }
                        retrievedDetails.Add(key, value);
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }

            
            return retrievedDetails;
        }

        public static DirectoryEntry CreateDirectoryEntry(ConnectionDetails connectionDetails)
        {
            

            Check.Argument.IsNotNull(connectionDetails, "connectionDetails");

            DirectoryEntry ldapConnection = null;
            try
            {
                // Create and return new LDAP connection
                ldapConnection = new DirectoryEntry(connectionDetails.DCHostNameOrIpAddress, connectionDetails.Username, connectionDetails.Password);
                //ldapConnection.Path = rootPath;
                //ldapConnection.AuthenticationType = AuthenticationTypes.Secure;

            }
            catch (Exception)
            {
                
                throw;
            }

            
            return ldapConnection;
        }

        #endregion

        #region Authentication

        public static bool ADAuthenticate(string domain, string username, string password)
        {
            FileTrace.WriteMemberEntry();
            Check.Argument.IsNotEmptyOrWhitespace(domain, "domain");
            Check.Argument.IsNotEmptyOrWhitespace(username, "username");
            Check.Argument.IsNotEmptyOrWhitespace(password, "password");

            var authenticated = false;
            try
            {
                // First validate against AD to check if this a valid AD user
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain))
                {
                    // Attempt to validate against Active Directory
                    authenticated = pc.ValidateCredentials(username, password);
                }
            }
            catch (Exception ex)
            {
                FileTrace.WriteException(ex);
                throw;
            }
            FileTrace.WriteMemberExit();
            return authenticated;
        }

        #endregion
    }
}
