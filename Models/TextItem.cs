using System.Drawing;

namespace LabelPrinterClient.Models
{
    public class TextItem : LabelItem
    {
        public string Text { get; set; } = string.Empty;
        public string FontFamily { get; set; } = "Microsoft JhengHei";
        public float FontSize { get; set; } = 12;
        public bool Bold { get; set; }
        public string Color { get; set; } = "#000000";
        public string Alignment { get; set; } = "Left";
        
        public TextItem()
        {
            Type = "Text";
        }
        
        public override void Render(Graphics g, Services.FieldResolver? resolver)
        {
            var text = resolver?.Resolve(Text) ?? Text;
            
            var font = new Font(FontFamily, FontSize, Bold ? FontStyle.Bold : FontStyle.Regular);
            var color = ColorTranslator.FromHtml(Color);
            var brush = new SolidBrush(color);
            
            var format = new StringFormat
            {
                Alignment = Alignment switch
                {
                    "Center" => StringAlignment.Center,
                    "Right" => StringAlignment.Far,
                    _ => StringAlignment.Near
                },
                LineAlignment = StringAlignment.Near
            };
            
            var rect = new RectangleF(X, Y, Width, Height);
            g.DrawString(text, font, brush, rect, format);
            
            font.Dispose();
            brush.Dispose();
            format.Dispose();
        }
    }
}
