using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBanMayVT.Common
{
    /// <summary>
    /// Utility chuẩn hóa giao diện — Light Modern Theme
    /// </summary>
    public static class UIStyleHelper
    {
        // ── Background colours ───────────────────────────────────────────
        public static readonly Color BgMain   = Color.FromArgb(248, 250, 252);   // #F8FAFC
        public static readonly Color BgCard   = Color.White;
        public static readonly Color BgHeader = Color.FromArgb(243, 244, 246);   // #F3F4F6

        // ── Text colours ─────────────────────────────────────────────────
        public static readonly Color TextWhite   = Color.White;                   // for buttons with colored bg
        public static readonly Color TextDark    = Color.FromArgb(17, 24, 39);    // #111827
        public static readonly Color TextMuted   = Color.FromArgb(107, 114, 128); // #6B7280

        // ── Action colours ────────────────────────────────────────────────
        public static readonly Color PrimaryBlue  = Color.FromArgb(37,  99, 235);  // #2563EB
        public static readonly Color SuccessGreen = Color.FromArgb(22, 163, 74);   // #16A34A
        public static readonly Color DangerRed    = Color.FromArgb(220, 38,  38);  // #DC2626

        // ────────────────────────────────────────────────────────────────
        // DataGridView — light theme
        // ────────────────────────────────────────────────────────────────
        public static void StyleDataGridView(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.BackgroundColor           = Color.White;
            dgv.BorderStyle               = BorderStyle.None;
            dgv.CellBorderStyle           = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor                 = Color.FromArgb(229, 231, 235);   // #E5E7EB
            dgv.RowHeadersVisible         = false;
            dgv.AllowUserToResizeColumns  = false;
            dgv.AllowUserToResizeRows     = false;

            // Column headers
            dgv.ColumnHeadersDefaultCellStyle.BackColor   = Color.FromArgb(243, 244, 246);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor   = Color.FromArgb(55, 65, 81);
            dgv.ColumnHeadersDefaultCellStyle.Font        = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment   = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 244, 246);
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(55, 65, 81);
            dgv.ColumnHeadersHeight                       = 38;
            dgv.ColumnHeadersHeightSizeMode               = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Rows — fixed height 56px (3 lines tall), vertically middle, horizontally left
            dgv.DefaultCellStyle.BackColor          = Color.White;
            dgv.DefaultCellStyle.ForeColor          = Color.FromArgb(17, 24, 39);
            dgv.DefaultCellStyle.Font               = new Font("Segoe UI", 9.5F);
            dgv.DefaultCellStyle.Alignment          = DataGridViewContentAlignment.MiddleLeft;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);  // light blue
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(29, 78, 216);
            dgv.DefaultCellStyle.Padding            = new Padding(6, 0, 6, 0);

            // Alternating rows — very light gray
            dgv.AlternatingRowsDefaultCellStyle.BackColor          = Color.FromArgb(249, 250, 251);
            dgv.AlternatingRowsDefaultCellStyle.ForeColor          = Color.FromArgb(17, 24, 39);
            dgv.AlternatingRowsDefaultCellStyle.Alignment          = DataGridViewContentAlignment.MiddleLeft;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(29, 78, 216);

            dgv.RowTemplate.Height = 56;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }

        // ────────────────────────────────────────────────────────────────
        // TextBox — light theme
        // ────────────────────────────────────────────────────────────────
        public static void StyleTextBox(TextBox txt)
        {
            txt.BackColor   = Color.FromArgb(249, 250, 251);
            txt.ForeColor   = Color.FromArgb(17, 24, 39);
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Font        = new Font("Segoe UI", 10F);
        }

        // ────────────────────────────────────────────────────────────────
        // ComboBox — light theme
        // ────────────────────────────────────────────────────────────────
        public static void StyleComboBox(ComboBox cbo)
        {
            cbo.BackColor = Color.FromArgb(249, 250, 251);
            cbo.ForeColor = Color.FromArgb(17, 24, 39);
            cbo.FlatStyle = FlatStyle.Flat;
            cbo.Font      = new Font("Segoe UI", 10F);
        }

        // ────────────────────────────────────────────────────────────────
        // NumericUpDown — light theme
        // ────────────────────────────────────────────────────────────────
        public static void StyleNumeric(NumericUpDown num)
        {
            num.BackColor = Color.FromArgb(249, 250, 251);
            num.ForeColor = Color.FromArgb(17, 24, 39);
            num.Font      = new Font("Segoe UI", 10F);
        }
    }
}
