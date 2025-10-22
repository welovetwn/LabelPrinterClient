using System.Drawing;
using System.Drawing.Printing;
using LabelPrinterClient.Models;

namespace LabelPrinterClient.Services
{
    public class PrintService
    {
        private LabelTemplate? _template;
        private FieldResolver? _resolver;
        
        public void Print(LabelTemplate template, FieldResolver? resolver = null, string? printerName = null)
        {
            _template = template;
            _resolver = resolver;
            
            var printDoc = new PrintDocument();
            
            if (!string.IsNullOrEmpty(printerName))
            {
                printDoc.PrinterSettings.PrinterName = printerName;
            }
            
            var paperSize = new PaperSize("Label", template.Width, template.Height);
            printDoc.DefaultPageSettings.PaperSize = paperSize;
            
            printDoc.PrintPage += PrintDoc_PrintPage;
            
            try
            {
                printDoc.Print();
            }
            catch (Exception ex)
            {
                throw new Exception($"列印失敗: {ex.Message}", ex);
            }
        }
        
        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (_template == null || e.Graphics == null)
                return;
            
            var renderer = new LabelRenderer(_template, _resolver);
            renderer.Render(e.Graphics);
            
            e.HasMorePages = false;
        }
        
        public static List<string> GetAvailablePrinters()
        {
            var printers = new List<string>();
            
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                printers.Add(printer);
            }
            
            return printers;
        }
        
        public static string? GetDefaultPrinter()
        {
            var printDoc = new PrintDocument();
            return printDoc.PrinterSettings.PrinterName;
        }
    }
}
