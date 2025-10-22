# Label Printer Client

標籤列印客戶端應用程式 - 可從 Web API 取得資料並列印標籤 🚀

## ✨ 功能特色

- ✅ 從 Web API 取得標籤資料
- ✅ 支援本地標籤模板檔案
- ✅ 即時預覽標籤
- ✅ 列印到任何 Windows 印表機
- ✅ 匯出為高解析度 PNG 圖片
- ✅ 批次列印支援
- ✅ 支援動態欄位替換

## 🚀 快速開始

### 1. 建置專案

```bash
dotnet restore
dotnet build
```

### 2. 執行程式

```bash
dotnet run
```

### 3. 選擇功能

程式會顯示互動式選單:

```
請選擇範例:
  1. 載入本地標籤並預覽
  2. 從 API 取得資料並列印
  3. 批次列印多筆資料
  4. 匯出為圖片
  5. 預覽後列印
  6. 自訂印表機
  0. 結束
```

## 📖 使用範例

### 範例 1: 載入並預覽

```csharp
var template = LabelTemplate.LoadFromFile("Templates/TSMC.label");
var fields = new Dictionary<string, string>
{
    { "EmployeeID", "E12345" },
    { "Name", "張小明" },
    { "Department", "製造部" }
};
var resolver = new FieldResolver(fields);
var previewForm = new PreviewForm(template, resolver);
previewForm.ShowDialog();
```

### 範例 2: 從 API 取得資料

```csharp
var apiClient = new LabelApiClient("http://localhost:5000");
var labelData = await apiClient.GetLabelDataAsync("EMP-001");

var template = LabelTemplate.LoadFromFile("Templates/TSMC.label");
var resolver = new FieldResolver(labelData);

var printService = new PrintService();
printService.Print(template, resolver);
```

### 範例 3: 批次列印

```csharp
var apiClient = new LabelApiClient("http://localhost:5000");
var template = LabelTemplate.LoadFromFile("Templates/TSMC.label");
var printService = new PrintService();

var employeeIds = new[] { "EMP-001", "EMP-002", "EMP-003" };

foreach (var empId in employeeIds)
{
    var labelData = await apiClient.GetLabelDataAsync(empId);
    var resolver = new FieldResolver(labelData);
    printService.Print(template, resolver);
}
```

### 範例 4: 匯出為圖片

```csharp
var template = LabelTemplate.LoadFromFile("Templates/TSMC.label");
var resolver = new FieldResolver(fields);
var renderer = new LabelRenderer(template, resolver);

renderer.ExportToPng("output.png", dpi: 300);
```

## 📋 標籤模板格式

標籤模板使用 JSON 格式:

```json
{
  "Name": "TSMC Employee Badge",
  "Width": 400,
  "Height": 300,
  "Items": [
    {
      "Type": "Text",
      "Text": "{{FIELD:Name}}",
      "X": 50,
      "Y": 50,
      "Width": 300,
      "Height": 40,
      "FontFamily": "Microsoft JhengHei",
      "FontSize": 16,
      "Bold": true,
      "Color": "#000000",
      "Alignment": "Left"
    },
    {
      "Type": "Barcode",
      "Data": "{{FIELD:EmployeeID}}",
      "BarcodeType": "QRCode",
      "X": 50,
      "Y": 100,
      "Width": 100,
      "Height": 100,
      "ShowText": true
    }
  ]
}
```

## 🏷️ 動態欄位標記

| 標記 | 說明 | 範例輸出 |
|------|------|----------|
| `{{FIELD:欄位名}}` | 自訂欄位 | (依資料而定) |
| `{{DATE}}` | 當前日期 | 2025/10/22 |
| `{{TIME}}` | 當前時間 | 14:30 |
| `{{NOW:格式}}` | 自訂格式時間 | 2025/10/22 14:30 |
| `{{USERNAME}}` | 系統使用者名稱 | john.doe |

## 🔌 API 端點規格

### GET /api/label/{id}

取得單筆標籤資料

**回應範例:**
```json
{
  "Success": true,
  "Message": "成功",
  "Data": {
    "Fields": {
      "EmployeeID": "E12345",
      "Name": "張小明",
      "Department": "製造部",
      "Position": "工程師"
    }
  }
}
```

### GET /api/template/{name}

取得標籤模板

**回應範例:**
```json
{
  "Name": "TSMC Employee Badge",
  "Width": 400,
  "Height": 300,
  "Items": [...]
}
```

## ⚙️ 設定檔

編輯 `App.config`:

```xml
<appSettings>
  <add key="ApiBaseUrl" value="http://localhost:5000" />
  <add key="DefaultTemplate" value="Templates/TSMC.label" />
  <add key="DefaultPrinter" value="" />
  <add key="ExportDpi" value="300" />
</appSettings>
```

## 💻 命令列使用

```bash
# 從 API 取得資料並列印
LabelPrinterClient.exe --template TSMC.label --api-id EMP-001 --print

# 預覽標籤
LabelPrinterClient.exe --template TSMC.label --preview

# 匯出為圖片
LabelPrinterClient.exe --template TSMC.label --api-id EMP-001 --export output.png
```

## 📦 系統需求

- .NET 6.0 或更高版本
- Windows 7 / 8 / 10 / 11
- 支援的印表機驅動程式

## 📁 專案結構

```
LabelPrinterClient/
├── Models/              # 資料模型
├── Services/            # 服務層
├── Utils/               # 工具類
├── Forms/               # WinForms 視窗
├── Examples/            # 使用範例
└── Templates/           # 標籤模板
```

## 📄 授權

MIT License
