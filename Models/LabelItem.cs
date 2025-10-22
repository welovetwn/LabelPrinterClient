using System.Drawing;
using Newtonsoft.Json;

namespace LabelPrinterClient.Models
{
    [JsonConverter(typeof(LabelItemConverter))]
    public abstract class LabelItem
    {
        public string Type { get; set; } = string.Empty;
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        
        public abstract void Render(Graphics g, Services.FieldResolver? resolver);
    }
    
    public class LabelItemConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(LabelItem);
        }
        
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var obj = Newtonsoft.Json.Linq.JObject.Load(reader);
            var type = obj["Type"]?.ToString();
            
            LabelItem? item = type switch
            {
                "Text" => new TextItem(),
                "Barcode" => new BarcodeItem(),
                "Image" => new ImageItem(),
                _ => null
            };
            
            if (item != null)
            {
                serializer.Populate(obj.CreateReader(), item);
            }
            
            return item;
        }
        
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
