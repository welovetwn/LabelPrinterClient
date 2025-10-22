using LabelPrinterClient.Models;
using LabelPrinterClient.Services;
using LabelPrinterClient.Forms;

namespace LabelPrinterClient.Examples
{
    public static class UsageExamples
    {
        private const string API_BASE_URL = "http://localhost:5000";
        
        public static void Example1_LoadAndPreview()
        {
            Console.WriteLine("=== 範例 1: 載入本地標籤並預覽 ===");
            
            var template = LabelTemplate.LoadFromFile("Templates/TSMC.label");
            if (template == null)
            {
                Console.WriteLine("❌ 無法載入標籤模板");
                return;
            }
            
            var fields = new Dictionary<string, string>
            {
                { "EmployeeID", "E12345" },
                { "Name", "張小明" },
                { "Department", "製造部" },
                { "Position", "工程師" }
            };
            
            var resolver = new FieldResolver(fields);
            var previewForm = new PreviewForm(template, resolver);
            previewForm.ShowDialog();
            
            Console.WriteLine("✅ 完成");
        }
        
        public static async Task Example2_ApiAndPrint()
        {
            Console.WriteLine("=== 範例 2: 從 API 取得資料並列印 ===");
            
            var apiClient = new LabelApiClient(API_BASE_URL);
            var labelData = await apiClient.GetLabelDataAsync("EMP-001");
            
            if (labelData == null)
            {
                Console.WriteLine("❌ 無法從 API 取得資料,使用範例資料");
                labelData = new Dictionary<string, string>
                {
                    { "EmployeeID", "E12345" },
                    { "Name", "張小明" },
                    { "Department", "製造部" },
                    { "Position", "工程師" }
                };
            }
            
            Console.WriteLine($"✅ 取得資料: {string.Join(", ", labelData.Select(x => $"{x.Key}={x.Value}"))}");
            
            var template = LabelTemplate.LoadFromFile("Templates/TSMC.label");
            if (template == null)
            {
                Console.WriteLine("❌ 無法載入標籤模板");
                return;
            }
            
            var resolver = new FieldResolver(labelData);
            var printService = new PrintService();
            printService.Print(template, resolver);
            
            Console.WriteLine("✅ 列印完成");
        }
        
        public static async Task Example3_BatchPrint()
        {
            Console.WriteLine("=== 範例 3: 批次列印 ===");
            
            var apiClient = new LabelApiClient(API_BASE_URL);
            var template = LabelTemplate.LoadFromFile("Templates/TSMC.label");
            
            if (template == null)
            {
                Console.WriteLine("❌ 無法載入標籤模板");
                return;
            }
            
            var employeeIds = new List<string> { "EMP-001", "EMP-002", "EMP-003" };
            var printService = new PrintService();
            var printedCount = 0;
            
            foreach (var empId in employeeIds)
            {
                var labelData = await apiClient.GetLabelDataAsync(empId);
                if (labelData == null)
                {
                    labelData = new Dictionary<string, string>
                    {
                        { "EmployeeID", empId },
                        { "Name", $"員工_{empId}" },
                        { "Department", "測試部門" },
                        { "Position", "測試職位" }
                    };
                }
                
                var resolver = new FieldResolver(labelData);
                printService.Print(template, resolver);
                printedCount++;
                Console.WriteLine($"✅ 已列印: {empId}");
                
                await Task.Delay(500);
            }
            
            Console.WriteLine($"✅ 批次列印完成,共 {printedCount} 張");
        }
        
        public static async Task Example4_ExportToImage()
        {
            Console.WriteLine("=== 範例 4: 匯出為圖片 ===");
            
            var apiClient = new LabelApiClient(API_BASE_URL);
            var labelData = await apiClient.GetLabelDataAsync("EMP-001");
            
            if (labelData == null)
            {
                Console.WriteLine("⚠️  API 無法連線,使用範例資料");
                labelData = new Dictionary<string, string>
                {
                    { "EmployeeID", "E12345" },
                    { "Name", "張小明" },
                    { "Department", "製造部" },
                    { "Position", "工程師" }
                };
            }
            
            var template = LabelTemplate.LoadFromFile("Templates/TSMC.label");
            if (template == null)
            {
                Console.WriteLine("❌ 無法載入標籤模板");
                return;
            }
            
            var resolver = new FieldResolver(labelData);
            var renderer = new LabelRenderer(template, resolver);
            
            var outputPath = "output/label_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
            Directory.CreateDirectory("output");
            
            renderer.ExportToPng(outputPath, dpi: 300);
            
            Console.WriteLine($"✅ 已匯出: {Path.GetFullPath(outputPath)}");
        }
        
        public static async Task Example5_PreviewBeforePrint()
        {
            Console.WriteLine("=== 範例 5: 預覽後列印 ===");
            
            var apiClient = new LabelApiClient(API_BASE_URL);
            var labelData = await apiClient.GetLabelDataAsync("EMP-001");
            
            if (labelData == null)
            {
                labelData = new Dictionary<string, string>
                {
                    { "EmployeeID", "E12345" },
                    { "Name", "張小明" },
                    { "Department", "製造部" },
                    { "Position", "工程師" }
                };
                Console.WriteLine("⚠️  API 無法連線,使用範例資料");
            }
            
            var template = LabelTemplate.LoadFromFile("Templates/TSMC.label");
            if (template == null)
            {
                Console.WriteLine("❌ 無法載入標籤模板");
                return;
            }
            
            var resolver = new FieldResolver(labelData);
            var previewForm = new PreviewForm(template, resolver);
            previewForm.ShowDialog();
            
            Console.WriteLine("✅ 完成");
        }
        
        public static void Example6_CustomPrinter()
        {
            Console.WriteLine("=== 範例 6: 指定印表機列印 ===");
            
            var printers = PrintService.GetAvailablePrinters();
            Console.WriteLine("可用印表機:");
            foreach (var printer in printers)
            {
                Console.WriteLine($"  - {printer}");
            }
            
            var defaultPrinter = PrintService.GetDefaultPrinter();
            Console.WriteLine($"\n預設印表機: {defaultPrinter}");
            
            var template = LabelTemplate.LoadFromFile("Templates/TSMC.label");
            if (template == null)
            {
                Console.WriteLine("❌ 無法載入標籤模板");
                return;
            }
            
            var fields = new Dictionary<string, string>
            {
                { "EmployeeID", "E99999" },
                { "Name", "測試員工" },
                { "Department", "測試部門" },
                { "Position", "測試職位" }
            };
            
            var resolver = new FieldResolver(fields);
            var printService = new PrintService();
            
            printService.Print(template, resolver);
            
            Console.WriteLine("✅ 已送出列印工作");
        }
    }
}
