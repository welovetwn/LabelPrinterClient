// 檔案路徑：LabelPrinterClient/Services/FieldResolver.cs
using System.Text.RegularExpressions;

namespace LabelPrinterClient.Services
{
    public class FieldResolver
    {
        private readonly Dictionary<string, string> _fields;

        public FieldResolver(Dictionary<string, string> fields)
        {
            _fields = fields ?? new Dictionary<string, string>();
        }

        public string Resolve(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            var result = input;

            // ✅ 替換 {{FIELD:欄位名稱}}
            result = Regex.Replace(result, @"\{\{FIELD:(.+?)\}\}", match =>
            {
                var key = match.Groups[1].Value;
                return _fields.TryGetValue(key, out var value)
                    ? value
                    : $"[未知欄位:{key}]";
            });

            // ✅ 替換 {{DATE}}
            result = result.Replace("{{DATE}}", DateTime.Now.ToShortDateString());

            // ✅ 替換 {{TIME}}
            result = result.Replace("{{TIME}}", DateTime.Now.ToShortTimeString());

            // ✅ 替換 {{USERNAME}}
            result = result.Replace("{{USERNAME}}", Environment.UserName);

            // ✅ 替換 {{NOW:yyyyMMdd}}
            result = Regex.Replace(result, @"\{\{NOW:(.+?)\}\}", match =>
            {
                var format = match.Groups[1].Value;
                try
                {
                    return DateTime.Now.ToString(format);
                }
                catch
                {
                    return DateTime.Now.ToString();
                }
            });

            return result;
        }
    }
}
