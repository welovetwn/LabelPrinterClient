using System.Drawing;
using LabelPrinterClient.Models;

namespace LabelPrinterClient.Services
{
    public class LabelRenderer
    {
        private readonly LabelTemplate _template;
        private readonly FieldResolver? _resolver;
        
        public LabelRenderer(LabelTemplate template, FieldResolver? resolver = null)
        {
            _template = template;
            _resolver = resolver;
        }
        
        public void Render(Graphics g)
        {
            g.Clear(System.Drawing.Color.White);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            
            foreach (var item in _template.Items)
            {
                try
                {
                    item.Render(g, _resolver);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"渲染項目時發生錯誤: {ex.Message}");
                }
            }
        }
        
        public Bitmap RenderToBitmap(int dpi = 96)
        {
            var bitmap = new Bitmap(_template.Width, _template.Height);
            bitmap.SetResolution(dpi, dpi);
            
            using (var g = Graphics.FromImage(bitmap))
            {
                Render(g);
            }
            
            return bitmap;
        }
        
        public void ExportToPng(string filePath, int dpi = 300)
        {
            using (var bitmap = RenderToBitmap(dpi))
            {
                bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
    }
}
