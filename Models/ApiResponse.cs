namespace LabelPrinterClient.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
    
    public class LabelDataResponse
    {
        public Dictionary<string, string> Fields { get; set; } = new Dictionary<string, string>();
    }
}
