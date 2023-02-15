using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Core.ActiveDirectory
{
    public class ConnectionDetails
    {
        public string DCHostNameOrIpAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public ConnectionDetails(string dcHostNameOrIpAddress, string username, string password)
        {
            DCHostNameOrIpAddress = dcHostNameOrIpAddress;
            Username = username;
            Password = password;
        }
    }
}
