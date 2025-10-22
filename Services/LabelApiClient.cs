using System.Net.Http;
using Newtonsoft.Json;
using LabelPrinterClient.Models;

namespace LabelPrinterClient.Services
{
    public class LabelApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        
        public LabelApiClient(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        // 檔案路徑：LabelPrinterClient/Services/LabelApiClient.cs
        public async Task<Dictionary<string, string>?> GetLabelDataAsync(string labelId)
        {
            try
            {
                var url = $"{_baseUrl}/api/label/{labelId}";
                Console.WriteLine($"📌 呼叫 LabelData API: {url}");
                var response = await _httpClient.GetAsync(url);
                Console.WriteLine($"📌 回應狀態碼: {(int)response.StatusCode} {response.StatusCode}");

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"📌 回傳內容: {json}");

                response.EnsureSuccessStatusCode();

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<LabelDataResponse>>(json);
                if (apiResponse == null)
                {
                    Console.WriteLine("⚠️ Deserialize 回傳空");
                    return null;
                }
                if (!apiResponse.Success)
                {
                    Console.WriteLine($"⚠️ API 表示失敗: {apiResponse.Message}");
                    return null;
                }
                if (apiResponse.Data == null)
                {
                    Console.WriteLine("⚠️ API 回傳 Data 為 null");
                    return null;
                }

                return apiResponse.Data.Fields;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API 錯誤: {ex.Message}");
                return null;
            }
        }


        public async Task<LabelTemplate?> GetLabelTemplateAsync(string templateName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/template/{templateName}");
                response.EnsureSuccessStatusCode();
                
                var json = await response.Content.ReadAsStringAsync();
                return LabelTemplate.FromJson(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API 錯誤: {ex.Message}");
                return null;
            }
        }
        
        public async Task<List<Dictionary<string, string>>> GetBatchLabelDataAsync(List<string> labelIds)
        {
            var result = new List<Dictionary<string, string>>();
            
            foreach (var id in labelIds)
            {
                var data = await GetLabelDataAsync(id);
                if (data != null)
                {
                    result.Add(data);
                }
            }
            
            return result;
        }
    }
}
