using System.Drawing;
using QRCoder;

namespace LabelPrinterClient.Utils
{
    public static class BarcodeGenerator
    {
        public static Bitmap? Generate(string data, string type, int width, int height)
        {
            return type.ToUpper() switch
            {
                "QRCODE" => GenerateQRCode(data, width),
                "CODE128" => GenerateCode128(data, width, height),
                "EAN13" => GenerateEAN13(data, width, height),
                _ => null
            };
        }
        
        private static Bitmap GenerateQRCode(string data, int size)
        {
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q))
            using (var qrCode = new QRCode(qrCodeData))
            {
                return qrCode.GetGraphic(20);
            }
        }
        
        private static Bitmap GenerateCode128(string data, int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(System.Drawing.Color.White);
                g.DrawString($"Code128: {data}", new Font("Arial", 10), Brushes.Black, 5, height / 2);
            }
            return bitmap;
        }
        
        private static Bitmap GenerateEAN13(string data, int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(System.Drawing.Color.White);
                g.DrawString($"EAN13: {data}", new Font("Arial", 10), Brushes.Black, 5, height / 2);
            }
            return bitmap;
        }
    }
}
