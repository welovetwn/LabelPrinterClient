using System.Drawing;
using System.Windows.Forms;
using LabelPrinterClient.Models;
using LabelPrinterClient.Services;

namespace LabelPrinterClient.Forms
{
    public partial class PreviewForm : Form
    {
        private readonly LabelTemplate _template;
        private readonly FieldResolver? _resolver;
        private readonly LabelRenderer _renderer;
        
        public PreviewForm(LabelTemplate template, FieldResolver? resolver = null)
        {
            _template = template;
            _resolver = resolver;
            _renderer = new LabelRenderer(template, resolver);
            
            InitializeComponent();
            InitializePreview();
        }
        
        private void InitializePreview()
        {
            this.Text = $"標籤預覽 - {_template.Name}";
            this.ClientSize = new Size(_template.Width + 40, _template.Height + 120);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            pictureBox1.Size = new Size(_template.Width, _template.Height);
            pictureBox1.Location = new Point(20, 20);
            
            var bitmap = _renderer.RenderToBitmap(96);
            pictureBox1.Image = bitmap;
        }
        
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                var printService = new PrintService();
                printService.Print(_template, _resolver);
                MessageBox.Show("列印完成!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"列印失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnExport_Click(object sender, EventArgs e)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "PNG 圖片|*.png";
                saveDialog.FileName = "label.png";
                
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    _renderer.ExportToPng(saveDialog.FileName, 300);
                    MessageBox.Show("匯出完成!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
