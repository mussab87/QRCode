using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Utility.Toolkit;

namespace QRCode.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(string str, string filename)
        {
            /*var whatsAppImageBaseFolder = @"\\datasever2\Work_share\QR";*/   //datasever2\Work_share\QR
            var whatsAppImageBaseFolder = @"D:\Barcodes";
            string before = "إختبار طباعة كود ثلاثي الابعاد" + "\r\n" + "السنة : 2023" + "\r\n";

            if (string.IsNullOrWhiteSpace(str))
                throw new Exception("Error in HLN: Unable to generate String.");

            str = before + str;

            var link = string.Format("{0}", str);
            var barcode = BarcodeGenerator.Generate3DCode(str);

            if (barcode == null)
                throw new Exception("Error in HLN: Unable to generate barcode");

            // Save the barcode to the folder specified.
            //var uniqueFilename = string.Concat(filename, ".png");
            var uniqueFilename = string.Concat(filename, ".jpeg");
            var filePath = string.Concat(whatsAppImageBaseFolder, @"\", uniqueFilename);
            barcode.Save(filePath);

            return "Done";
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
