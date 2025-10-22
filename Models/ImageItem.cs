using System.Drawing;

namespace LabelPrinterClient.Models
{
    public class ImageItem : LabelItem
    {
        public string ImagePath { get; set; } = string.Empty;
        public bool MaintainAspectRatio { get; set; } = true;
        
        public ImageItem()
        {
            Type = "Image";
        }
        
        public override void Render(Graphics g, Services.FieldResolver? resolver)
        {
            if (string.IsNullOrEmpty(ImagePath) || !File.Exists(ImagePath))
                return;
            
            using (var image = Image.FromFile(ImagePath))
            {
                var rect = new Rectangle(X, Y, Width, Height);
                
                if (MaintainAspectRatio)
                {
                    var aspectRatio = (float)image.Width / image.Height;
                    var targetAspectRatio = (float)Width / Height;
                    
                    if (aspectRatio > targetAspectRatio)
                    {
                        var newHeight = (int)(Width / aspectRatio);
                        rect.Y += (Height - newHeight) / 2;
                        rect.Height = newHeight;
                    }
                    else
                    {
                        var newWidth = (int)(Height * aspectRatio);
                        rect.X += (Width - newWidth) / 2;
                        rect.Width = newWidth;
                    }
                }
                
                g.DrawImage(image, rect);
            }
        }
    }
}
