using System.Collections.Generic;

namespace LabelPrinterClient.Models
{
    public class LabelTemplate
    {
        public string Name { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public List<LabelItem> Items { get; set; } = new List<LabelItem>();
        
        public static LabelTemplate? FromJson(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<LabelTemplate>(json);
        }
        
        public static LabelTemplate? LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"標籤檔案不存在: {filePath}");
            }
            var json = File.ReadAllText(filePath);
            return FromJson(json);
        }
        
        public void SaveToFile(string filePath)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}
