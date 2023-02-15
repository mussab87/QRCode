using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;

namespace Utility.Toolkit.Security
{
    public class ADManager
    {
        public DirectorySearcher DirectorySearcher { get; set; }

        private string _domain = null;
        private string _username = null;
        private string _password = null;

        public ADManager(string domain, string username, string password)
        {
            
            try
            {
                _domain = domain;
                _username = username;
                _password = password;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public void Connect()
        {
            
            try
            {
                var message = string.Empty;
                if (string.IsNullOrWhiteSpace(_domain))
                    message = "Domain is missing ";
                if (string.IsNullOrWhiteSpace(_username))
                    message = message + " Username is missing";
                if (string.IsNullOrWhiteSpace(_password))
                    message = message + " Password is missing";

                if (!string.IsNullOrEmpty(message))
                {
                    message = message.Replace("  ", " ");
                    throw new Exception(message);
                }

                var ldapUrl = "LDAP://" + _domain;
                DirectorySearcher = new DirectorySearcher(new DirectoryEntry(ldapUrl, _username, _password));
                if (DirectorySearcher == null)
                    throw new Exception(string.Format("Unable to connect to Active Directory with LDAP url: '{0}', username '{1}' and password '{2}'", ldapUrl, _username, _password));
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public static ADUser SearchByUsername(string username)//, string domain)
        {
            

            ADUser adUser = null;
            try
            {
                //var ldapUrl = "LDAP://" + domain;
                DirectorySearcher ds = new DirectorySearcher(new DirectoryEntry());

                ds.Filter = "(&((&(objectCategory=Person)(objectClass=User)))(samaccountname=" + username + "))";

                ds.SearchScope = SearchScope.Subtree;
                ds.ServerTimeLimit = TimeSpan.FromSeconds(90);

                SearchResult searchResult = ds.FindOne();

                if (searchResult != null)
                {
                    adUser = new ADUser();

                    if (searchResult.GetDirectoryEntry().Properties["samaccountname"].Value != null)
                        adUser.Username = searchResult.GetDirectoryEntry().Properties["samaccountname"].Value.ToString();
                    if (searchResult.GetDirectoryEntry().Properties["givenName"].Value != null)
                        adUser.FirstName = searchResult.GetDirectoryEntry().Properties["givenName"].Value.ToString();
                    if (searchResult.GetDirectoryEntry().Properties["initials"].Value != null)
                        adUser.Initials = searchResult.GetDirectoryEntry().Properties["initials"].Value.ToString();
                    if (searchResult.GetDirectoryEntry().Properties["sn"].Value != null)
                        adUser.LastName = searchResult.GetDirectoryEntry().Properties["sn"].Value.ToString();
                    if (searchResult.GetDirectoryEntry().Properties["mail"].Value != null)
                        adUser.Email = searchResult.GetDirectoryEntry().Properties["mail"].Value.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return adUser;
        }

        public static ADUser SearchByUsername(string username, string domain)
        {

            ADUser adUser = null;
            try
            {
                var ldap = "LDAP://" + domain;
                DirectorySearcher ds = new DirectorySearcher(ldap);

                ds.Filter = "(&((&(objectCategory=Person)(objectClass=User)))(samaccountname=" + username + "))";

                ds.SearchScope = SearchScope.Subtree;
                ds.ServerTimeLimit = TimeSpan.FromSeconds(90);

                SearchResult searchResult = ds.FindOne();

                if (searchResult != null)
                {
                    adUser = new ADUser();

                    if (searchResult.GetDirectoryEntry().Properties["samaccountname"].Value != null)
                        adUser.Username = searchResult.GetDirectoryEntry().Properties["samaccountname"].Value.ToString();
                    if (searchResult.GetDirectoryEntry().Properties["givenName"].Value != null)
                        adUser.FirstName = searchResult.GetDirectoryEntry().Properties["givenName"].Value.ToString();
                    if (searchResult.GetDirectoryEntry().Properties["initials"].Value != null)
                        adUser.Initials = searchResult.GetDirectoryEntry().Properties["initials"].Value.ToString();
                    if (searchResult.GetDirectoryEntry().Properties["sn"].Value != null)
                        adUser.LastName = searchResult.GetDirectoryEntry().Properties["sn"].Value.ToString();
                    if (searchResult.GetDirectoryEntry().Properties["mail"].Value != null)
                        adUser.Email = searchResult.GetDirectoryEntry().Properties["mail"].Value.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return adUser;
        }
    }
}