using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Utility.Toolkit.Web
{
    public class UserToken
    {
        public static string GenerateUserToken(string username, string CreateTokenUrl)
        {
            string password = "!#MyClinic2020==";

            string reqUri =
                String.Format(CreateTokenUrl +
                "username={0}&password={1}",
                username, password);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(reqUri);
            req.Timeout = 10000; // 10 seconds

            Stream stm = req.GetResponse().GetResponseStream();

            //read text token
            using (StreamReader reader = new StreamReader(stm))
                return reader.ReadLine();
        }

        public static string CheckSelectedUserToken(string token, string Tokenusername, string jwtTokenURL)
        {

            var DeSerializedtoken = JsonConvert.DeserializeObject<string>(token);

            string reqUri =
                String.Format(jwtTokenURL +
                "token={0}&username={1}",
                DeSerializedtoken, Tokenusername);


            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(reqUri);
            //req.Method = "Post";
            req.Timeout = 10000; // 10 seconds


            Stream stm = req.GetResponse().GetResponseStream();

            //read text token
            using (StreamReader reader = new StreamReader(stm))
                return reader.ReadLine();
        }
    }
}
