// 檔案路徑：LabelPrinterClient/Services/InternalWebApiService.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LabelPrinterClient.Models;
using Microsoft.AspNetCore.Http;

namespace LabelPrinterClient.Services
{
    public class InternalWebApiService : BackgroundService
    {
        private readonly ILogger<InternalWebApiService> _logger;
        private IHost? _webHost;

        public InternalWebApiService(ILogger<InternalWebApiService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _webHost = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://localhost:5000");
                    webBuilder.ConfigureServices(services =>
                    {
                        services.AddControllers().AddNewtonsoftJson();
                        services.AddEndpointsApiExplorer();    // ✅
                        services.AddSwaggerGen();              // ✅
                    });
                    webBuilder.Configure(app =>
                    {
                        app.UseSwagger();                      // ✅
                        app.UseSwaggerUI(c =>                  // ✅
                        {
                            c.SwaggerEndpoint("/swagger/v1/swagger.json", "LabelPrinter API V1");
                            c.RoutePrefix = "swagger";
                        });
                        //
                        var templates = GetSampleTemplates();
                        var labels = GetSampleLabelData();

                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapGet("/api/label/{labelId}", (string labelId) =>
                            {
                                labels.TryGetValue(labelId, out var data);
                                return Results.Ok(new ApiResponse<LabelDataResponse>
                                {
                                    Success = data != null,
                                    Message = data != null ? "成功" : "找不到標籤資料",
                                    Data = data
                                });
                            });

                            endpoints.MapGet("/api/template/{templateName}", (string templateName) =>
                            {
                                templates.TryGetValue(templateName, out var template);
                                return Results.Ok(new ApiResponse<LabelTemplate>
                                {
                                    Success = template != null,
                                    Message = template != null ? "成功" : "找不到模板",
                                    Data = template
                                });
                            });
                        });
                    });
                })
                .Build();

            _logger.LogInformation("✅ 內建 Web API 已啟動於 http://localhost:5000");
            await _webHost.RunAsync(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_webHost != null)
            {
                await _webHost.StopAsync(cancellationToken);
                _webHost.Dispose();
            }
        }

        private static Dictionary<string, LabelTemplate> GetSampleTemplates() =>
            new()
            {
                ["sample-template"] = new LabelTemplate
                {
                    Name = "sample-template",
                    Width = 400,
                    Height = 300,
                    Items = new List<LabelItem>
                    {
                        new TextItem
                        {
                            Type = "Text",
                            X = 10,
                            Y = 20,
                            Width = 100,
                            Height = 30,
                            Text = "{{ProductName}}",
                            FontSize = 12
                        },
                        new BarcodeItem
                        {
                            Type = "Barcode",
                            X = 10,
                            Y = 60,
                            Width = 200,
                            Height = 50,
                            BarcodeType = "Code128"
                        }
                    }
                }
            };

        private static Dictionary<string, LabelDataResponse> GetSampleLabelData() =>
            new()
            {
                ["EMP-001"] = new LabelDataResponse
                {
                    Fields = new Dictionary<string, string>
                    {
                        { "EmployeeID", "E54321" },
                        { "Name", "張明明" },
                        { "Department", "製造部" },
                        { "Position", "工程師" }
                    }
                },
                ["A001"] = new LabelDataResponse
                {
                    Fields = new Dictionary<string, string>
                    {
                        { "ProductName", "巧克力餅乾" },
                        { "Barcode", "4712345678901" }
                    }
                }
            };
    }
}
