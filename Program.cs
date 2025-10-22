// 檔案路徑：LabelPrinterClient/Program.cs
using System;
using System.Windows.Forms;
using LabelPrinterClient.Examples;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LabelPrinterClient.Services;

namespace LabelPrinterClient
{
    internal static class Program
    {
        [STAThread]
        static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            // 啟動背景服務（Web API）
            await host.StartAsync();

            Console.WriteLine("╔═══════════════════════════════════════╗");
            Console.WriteLine("║   Label Printer Client - 標籤列印客戶端   ║");
            Console.WriteLine("╚═══════════════════════════════════════╝");
            Console.WriteLine();

            Application.EnableVisualStyles(); // ✅ 確保 WinForms 樣式一致性
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
            {
                await RunCommandLine(args);
            }
            else
            {
                await RunInteractive();
            }

            // 結束後關閉主機
            await host.StopAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    // 註冊背景 Web API 服務
                    services.AddHostedService<InternalWebApiService>();
                });

        static async Task RunInteractive()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("請選擇範例:");
                Console.WriteLine("  1. 載入本地標籤並預覽");
                Console.WriteLine("  2. 從 API 取得資料並列印");
                Console.WriteLine("  3. 批次列印多筆資料");
                Console.WriteLine("  4. 匯出為圖片");
                Console.WriteLine("  5. 預覽後列印");
                Console.WriteLine("  6. 自訂印表機");
                Console.WriteLine("  0. 結束");
                Console.Write("\n請輸入選項: ");

                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            UsageExamples.Example1_LoadAndPreview();
                            break;
                        case "2":
                            await UsageExamples.Example2_ApiAndPrint();
                            break;
                        case "3":
                            await UsageExamples.Example3_BatchPrint();
                            break;
                        case "4":
                            await UsageExamples.Example4_ExportToImage();
                            break;
                        case "5":
                            await UsageExamples.Example5_PreviewBeforePrint();
                            break;
                        case "6":
                            UsageExamples.Example6_CustomPrinter();
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("❌ 無效的選項");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n❌ 發生錯誤: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        static async Task RunCommandLine(string[] args)
        {
            var templateFile = "Templates/TSMC.label";
            var apiId = string.Empty;
            var shouldPrint = false;
            var shouldPreview = false;
            var exportPath = string.Empty;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "--template":
                    case "-t":
                        if (i + 1 < args.Length)
                            templateFile = args[++i];
                        break;
                    case "--api-id":
                    case "-a":
                        if (i + 1 < args.Length)
                            apiId = args[++i];
                        break;
                    case "--print":
                    case "-p":
                        shouldPrint = true;
                        break;
                    case "--preview":
                    case "-v":
                        shouldPreview = true;
                        break;
                    case "--export":
                    case "-e":
                        if (i + 1 < args.Length)
                            exportPath = args[++i];
                        break;
                }
            }

            Console.WriteLine($"模板檔案: {templateFile}");
            Console.WriteLine($"API ID: {apiId}");
            Console.WriteLine($"列印: {shouldPrint}");
            Console.WriteLine($"預覽: {shouldPreview}");
            Console.WriteLine($"匯出: {exportPath}");

            if (!string.IsNullOrEmpty(apiId))
            {
                await UsageExamples.Example2_ApiAndPrint();
            }
            else if (shouldPreview)
            {
                await UsageExamples.Example5_PreviewBeforePrint();
            }
        }
    }
}