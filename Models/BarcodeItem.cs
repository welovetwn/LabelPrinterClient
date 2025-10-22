// 檔案路徑：LabelPrinterClient/Models/BarcodeItem.cs
using System.Drawing;

namespace LabelPrinterClient.Models
{
    public class BarcodeItem : LabelItem
    {
        public string Data { get; set; } = string.Empty;
        public string BarcodeType { get; set; } = "QRCode";
        public bool ShowText { get; set; }

        public BarcodeItem()
        {
            Type = "Barcode";
        }

        public override void Render(Graphics g, Services.FieldResolver? resolver)
        {
            // ✅ 加入解析資料
            var resolvedData = resolver?.Resolve(Data) ?? Data;

            Console.WriteLine($"📌 BarcodeItem: 原始='{Data}', 解析後='{resolvedData}'");

            if (string.IsNullOrWhiteSpace(resolvedData))
            {
                Console.WriteLine("⚠️ 條碼資料為空，略過渲染。");
                return;
            }

            using (var barcodeBitmap = Utils.BarcodeGenerator.Generate(resolvedData, BarcodeType, Width, Height))
            {
                if (barcodeBitmap != null)
                {
                    g.DrawImage(barcodeBitmap, X, Y, Width, Height);
                }
                else
                {
                    Console.WriteLine("⚠️ 條碼產生失敗");
                }
            }

            if (ShowText)
            {
                using (var font = new Font("Arial", 8))
                using (var brush = new SolidBrush(Color.Black))
                {
                    var format = new StringFormat { Alignment = StringAlignment.Center };
                    g.DrawString(resolvedData, font, brush, X + Width / 2, Y + Height + 2, format);
                    format.Dispose();
                }
            }
        }
    }
}
