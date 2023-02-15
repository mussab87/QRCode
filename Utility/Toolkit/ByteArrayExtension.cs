using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Utility.Toolkit
{
    public static class ByteArrayExtension
    {
        public static string ToBase64String(string imageName)
        {
            var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/UploadedFiles"), imageName);
            if (File.Exists(fileSavePath))
            {
                var webClient = new WebClient();
                byte[] imageBytes = webClient.DownloadData(fileSavePath);
                return Convert.ToBase64String(imageBytes);
            }

            return "";
        }

        public static string ToStringBase64(string encodedString)
        {
            var base64EncodedBytes = Convert.FromBase64String(encodedString);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
