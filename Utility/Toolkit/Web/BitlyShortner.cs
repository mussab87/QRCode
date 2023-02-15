using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Utility.Toolkit.Web
{
    public class BitlyShortner
    {
        public static string Shorten(string url, string login, string key, bool xml)
        {
            url = Uri.EscapeUriString(url);
            string reqUri =
                String.Format("http://api.bit.ly/v3/shorten?" +
                "login={0}&apiKey={1}&format={2}&longUrl={3}",
                login, key, xml ? "xml" : "txt", url);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(reqUri);
            req.Timeout = 10000; // 10 seconds

            // if the function fails and format==txt throws an exception
            Stream stm = req.GetResponse().GetResponseStream();

            if (xml)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(stm);

                // error checking for xml
                if (doc["response"]["status_code"].InnerText != "200")
                    throw new WebException(doc["response"]["status_txt"].InnerText);

                return doc["response"]["data"]["url"].InnerText;
            }
            else // Text
                using (StreamReader reader = new StreamReader(stm))
                    return reader.ReadLine();
        }
    }
}
