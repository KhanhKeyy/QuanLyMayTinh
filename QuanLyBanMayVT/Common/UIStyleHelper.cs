using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBanMayVT.Common
{
    /// <summary>
    /// Utility giúp chuẩn hóa giao diện (Dark Modern Theme) cho DataGridView, Button, Control
    /// </summary>
    public static class UIStyleHelper
    {
        public static readonly Color BgMain = Color.FromArgb(15, 23, 42);      // Slate 900
        public static readonly Color BgCard = Color.FromArgb(30, 41, 59);      // Slate 800
        public static readonly Color BgHeader = Color.FromArgb(51, 65, 85);    // Slate 700
        public static readonly Color TextWhite = Color.White;
        public static readonly Color TextMuted = Color.FromArgb(148, 163, 184); // Slate 400
        public static readonly Color PrimaryBlue = Color.FromArgb(59, 130, 246);
        public static readonly Color SuccessGreen = Color.FromArgb(16, 185, 129);
        public static readonly Color DangerRed = Color.FromArgb(239, 68, 68);

        public static void StyleDataGridView(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.BackgroundColor = BgCard;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(51, 65, 85);
            dgv.RowHeadersVisible = false;

            // Column Headers
            dgv.ColumnHeadersDefaultCellStyle.BackColor = BgHeader;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextWhite;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersHeight = 38;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Rows
            dgv.DefaultCellStyle.BackColor = BgCard;
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(241, 245, 249);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgv.DefaultCellStyle.SelectionBackColor = PrimaryBlue;
            dgv.DefaultCellStyle.SelectionForeColor = TextWhite;
            dgv.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            // Alternating Row
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 34, 53);
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(241, 245, 249);
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = PrimaryBlue;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = TextWhite;

            dgv.RowTemplate.Height = 35;
        }

        public static void StyleTextBox(TextBox txt)
        {
            txt.BackColor = Color.FromArgb(51, 65, 85);
            txt.ForeColor = TextWhite;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Font = new Font("Segoe UI", 10F);
        }

        public static void StyleComboBox(ComboBox cbo)
        {
            cbo.BackColor = Color.FromArgb(51, 65, 85);
            cbo.ForeColor = TextWhite;
            cbo.FlatStyle = FlatStyle.Flat;
            cbo.Font = new Font("Segoe UI", 10F);
        }

        public static void StyleNumeric(NumericUpDown num)
        {
            num.BackColor = Color.FromArgb(51, 65, 85);
            num.ForeColor = TextWhite;
            num.Font = new Font("Segoe UI", 10F);
        }
    }
}
