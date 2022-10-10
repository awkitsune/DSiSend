using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DSiSend.Core
{
    public static class QrGen
    {
        static MemoryStream ms = new MemoryStream();
        static QRCodeGenerator qrGenerator = new QRCodeGenerator();

        public static ImageSource GetImageForLink(string link)
        {
            ms = new MemoryStream();
            qrGenerator = new QRCodeGenerator();

            QRCodeData qrCodeData =
                qrGenerator.CreateQrCode(link,
                QRCodeGenerator.ECCLevel.L);

            var qr = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qr.GetGraphic(20, "#000", "#fff", false);

            qrCodeImage.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();

            return bi;
        }
    }
}
