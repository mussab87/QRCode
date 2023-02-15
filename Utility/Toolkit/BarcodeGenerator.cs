using System;
using System.Drawing;
using QRCoder;
using Utility.Core.Logging;
using Utility.Core.Utitlites;

namespace Utility.Toolkit
{
    public class BarcodeGenerator
    {
        public static Bitmap Generate3DCode(string link)
        {
            FileTrace.WriteMemberEntry();
            Check.Argument.IsNotEmptyOrWhitespace(link, "link");

            Bitmap barcodeImage = null;
            try
            {
                //QRCoder.PayloadGenerator.Url generator = new QRCoder.PayloadGenerator.Url(link);
                string payload = link.ToString();

                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                barcodeImage = qrCode.GetGraphic(5, Color.Black, Color.White, (Bitmap)Bitmap.FromFile(@"D:\Work\QRCode\QRCode\QRCode\Icon\logo.png"));//qrCode.GetGraphic(20);
            }
            catch (Exception ex)
            {
                FileTrace.WriteException(ex);
                throw;
            }
            FileTrace.WriteMemberExit();
            return barcodeImage;
        }
    }
}