namespace QuanLyBanMayVT
{
    partial class frmDangNhap
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            panelBackground   = new Panel();
            panelCard         = new Panel();
            lblTitle          = new Label();
            lblSubtitle       = new Label();
            lblTenDangNhap    = new Label();
            panelTenDangNhap  = new Panel();
            txtTenDangNhap    = new TextBox();
            lblMatKhau        = new Label();
            panelPassword     = new Panel();
            txtMatKhau        = new TextBox();
            btnTogglePassword = new Button();
            btnDangNhap       = new Button();
            btnThoat          = new Button();
            lblFooter         = new Label();

            panelBackground.SuspendLayout();
            panelCard.SuspendLayout();
            panelTenDangNhap.SuspendLayout();
            panelPassword.SuspendLayout();
            SuspendLayout();

            // ── panelBackground — vẽ viền ngoài card ────────────────────
            panelBackground.BackColor = Color.FromArgb(243, 244, 246);
            panelBackground.Controls.Add(panelCard);
            panelBackground.Dock      = DockStyle.Fill;
            panelBackground.Location  = new Point(0, 0);
            panelBackground.Name      = "panelBackground";
            panelBackground.Size      = new Size(800, 520);
            panelBackground.Paint    += PanelBackground_PaintCardBorder;

            // ── panelCard — white card, centred ───────────────────────────
            panelCard.BackColor = Color.White;
            panelCard.Controls.Add(lblTitle);
            panelCard.Controls.Add(lblSubtitle);
            panelCard.Controls.Add(lblTenDangNhap);
            panelCard.Controls.Add(panelTenDangNhap);
            panelCard.Controls.Add(lblMatKhau);
            panelCard.Controls.Add(panelPassword);
            panelCard.Controls.Add(btnDangNhap);
            panelCard.Controls.Add(btnThoat);
            panelCard.Controls.Add(lblFooter);
            panelCard.Location = new Point(200, 10);
            panelCard.Name     = "panelCard";
            panelCard.Size     = new Size(400, 500);

            // ── lblTitle ─────────────────────────────────────────────────
            lblTitle.Font      = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(17, 24, 39);
            lblTitle.Location  = new Point(0, 35);
            lblTitle.Name      = "lblTitle";
            lblTitle.Size      = new Size(400, 45);
            lblTitle.Text      = "🖥️ Quản Lý Máy Vi Tính";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // ── lblSubtitle ───────────────────────────────────────────────
            lblSubtitle.Font      = new Font("Segoe UI", 9F);
            lblSubtitle.ForeColor = Color.FromArgb(107, 114, 128);
            lblSubtitle.Location  = new Point(0, 82);
            lblSubtitle.Name      = "lblSubtitle";
            lblSubtitle.Size      = new Size(400, 22);
            lblSubtitle.Text      = "Hệ thống quản lý cửa hàng máy tính";
            lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;

            // ── lblTenDangNhap ────────────────────────────────────────────
            lblTenDangNhap.Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblTenDangNhap.ForeColor = Color.FromArgb(55, 65, 81);
            lblTenDangNhap.Location  = new Point(40, 135);
            lblTenDangNhap.Name      = "lblTenDangNhap";
            lblTenDangNhap.Size      = new Size(140, 22);
            lblTenDangNhap.Text      = "Tên đăng nhập";

            // ── panelTenDangNhap — bọc TextBox để viền giống panelPassword ─
            panelTenDangNhap.BackColor = Color.FromArgb(249, 250, 251);
            panelTenDangNhap.Controls.Add(txtTenDangNhap);
            panelTenDangNhap.Location  = new Point(40, 162);
            panelTenDangNhap.Name      = "panelTenDangNhap";
            panelTenDangNhap.Size      = new Size(320, 36);
            panelTenDangNhap.Paint    += PanelInput_Paint;

            // ── txtTenDangNhap — không có viền riêng, viền do panel vẽ ──────
            txtTenDangNhap.BackColor       = Color.FromArgb(249, 250, 251);
            txtTenDangNhap.BorderStyle     = BorderStyle.None;
            txtTenDangNhap.Dock            = DockStyle.Fill;
            txtTenDangNhap.Font            = new Font("Segoe UI", 11F);
            txtTenDangNhap.ForeColor       = Color.FromArgb(17, 24, 39);
            txtTenDangNhap.Name            = "txtTenDangNhap";
            txtTenDangNhap.PlaceholderText = "Nhập tên đăng nhập...";

            // ── lblMatKhau ────────────────────────────────────────────────
            lblMatKhau.Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblMatKhau.ForeColor = Color.FromArgb(55, 65, 81);
            lblMatKhau.Location  = new Point(40, 215);
            lblMatKhau.Name      = "lblMatKhau";
            lblMatKhau.Size      = new Size(100, 22);
            lblMatKhau.Text      = "Mật khẩu";

            // ── panelPassword — viền giống panelTenDangNhap ──────────────
            panelPassword.BackColor  = Color.FromArgb(249, 250, 251);
            panelPassword.Controls.Add(txtMatKhau);
            panelPassword.Controls.Add(btnTogglePassword);
            panelPassword.Location   = new Point(40, 242);
            panelPassword.Name       = "panelPassword";
            panelPassword.Size       = new Size(320, 36);
            panelPassword.Paint     += PanelInput_Paint;

            // ── txtMatKhau ────────────────────────────────────────────────
            txtMatKhau.BackColor       = Color.FromArgb(249, 250, 251);
            txtMatKhau.BorderStyle     = BorderStyle.None;
            txtMatKhau.Dock            = DockStyle.Fill;
            txtMatKhau.Font            = new Font("Segoe UI", 11F);
            txtMatKhau.ForeColor       = Color.FromArgb(17, 24, 39);
            txtMatKhau.PasswordChar    = '*';
            txtMatKhau.PlaceholderText = "Nhập mật khẩu...";

            // ── btnTogglePassword ─────────────────────────────────────────
            btnTogglePassword.BackColor = Color.FromArgb(249, 250, 251);
            btnTogglePassword.Cursor    = Cursors.Hand;
            btnTogglePassword.Dock      = DockStyle.Right;
            btnTogglePassword.FlatAppearance.BorderSize = 0;
            btnTogglePassword.FlatStyle = FlatStyle.Flat;
            btnTogglePassword.Font      = new Font("Segoe UI", 14F);
            btnTogglePassword.ForeColor = Color.FromArgb(107, 114, 128);
            btnTogglePassword.Size      = new Size(36, 36);
            btnTogglePassword.Text      = "👁";
            btnTogglePassword.UseVisualStyleBackColor = false;
            btnTogglePassword.Click += btnTogglePassword_Click;

            // ── btnDangNhap ───────────────────────────────────────────────
            btnDangNhap.BackColor = Color.FromArgb(17, 24, 39);
            btnDangNhap.Cursor    = Cursors.Hand;
            btnDangNhap.FlatAppearance.BorderSize = 0;
            btnDangNhap.FlatAppearance.MouseOverBackColor = Color.FromArgb(31, 41, 55);
            btnDangNhap.FlatStyle = FlatStyle.Flat;
            btnDangNhap.Font      = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnDangNhap.ForeColor = Color.White;
            btnDangNhap.Location  = new Point(40, 310);
            btnDangNhap.Name      = "btnDangNhap";
            btnDangNhap.Size      = new Size(320, 46);
            btnDangNhap.Text      = "Đăng nhập";
            btnDangNhap.UseVisualStyleBackColor = false;
            btnDangNhap.Click      += btnDangNhap_Click;
            btnDangNhap.MouseEnter += btnDangNhap_MouseEnter;
            btnDangNhap.MouseLeave += btnDangNhap_MouseLeave;

            // ── btnThoat ─────────────────────────────────────────────────
            btnThoat.BackColor = Color.Transparent;
            btnThoat.Cursor    = Cursors.Hand;
            btnThoat.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            btnThoat.FlatAppearance.MouseOverBackColor = Color.FromArgb(243, 244, 246);
            btnThoat.FlatStyle = FlatStyle.Flat;
            btnThoat.Font      = new Font("Segoe UI", 10F);
            btnThoat.ForeColor = Color.FromArgb(107, 114, 128);
            btnThoat.Location  = new Point(40, 368);
            btnThoat.Name      = "btnThoat";
            btnThoat.Size      = new Size(320, 40);
            btnThoat.Text      = "Thoát";
            btnThoat.UseVisualStyleBackColor = false;
            btnThoat.Click += btnThoat_Click;

            // ── lblFooter ─────────────────────────────────────────────────
            lblFooter.Font      = new Font("Segoe UI", 8F);
            lblFooter.ForeColor = Color.FromArgb(156, 163, 175);
            lblFooter.Location  = new Point(0, 455);
            lblFooter.Name      = "lblFooter";
            lblFooter.Size      = new Size(400, 30);
            lblFooter.Text      = "© 2026 QuanLyBanMayVT - Hệ thống quản lý cửa hàng";
            lblFooter.TextAlign = ContentAlignment.MiddleCenter;

            // ── frmDangNhap ───────────────────────────────────────────────
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode       = AutoScaleMode.Dpi;
            BackColor           = Color.FromArgb(243, 244, 246);
            ClientSize          = new Size(800, 520);
            Controls.Add(panelBackground);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox     = false;
            Name            = "frmDangNhap";
            StartPosition   = FormStartPosition.CenterScreen;
            Text            = "Đăng nhập - Quản lý bán máy vi tính";
            Load           += frmDangNhap_Load;

            panelBackground.ResumeLayout(false);
            panelCard.ResumeLayout(false);
            panelCard.PerformLayout();
            panelTenDangNhap.ResumeLayout(false);
            panelTenDangNhap.PerformLayout();
            panelPassword.ResumeLayout(false);
            panelPassword.PerformLayout();
            ResumeLayout(false);
        }

        // Vẽ viền 1px xám nhạt đồng nhất cho cả 2 panel input
        private void PanelInput_Paint(object sender, PaintEventArgs e)
        {
            if (sender is Panel p)
            {
                using var pen = new Pen(Color.FromArgb(209, 213, 219), 1);
                e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
            }
        }

        // Vẽ viền ngoài cho panelCard từ panelBackground (không bị che bởi control con)
        private void PanelBackground_PaintCardBorder(object sender, PaintEventArgs e)
        {
            var card = panelCard;
            if (card == null) return;
            var r = new System.Drawing.Rectangle(card.Left - 1, card.Top - 1, card.Width + 1, card.Height + 1);
            using var pen = new Pen(Color.FromArgb(209, 213, 219), 1);
            e.Graphics.DrawRectangle(pen, r);
        }

        #endregion

        // Controls
        private Panel panelBackground;
        private Panel panelCard;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblTenDangNhap;
        private Panel panelTenDangNhap;
        private TextBox txtTenDangNhap;
        private Label lblMatKhau;
        private Panel panelPassword;
        private TextBox txtMatKhau;
        private Button btnTogglePassword;
        private Button btnDangNhap;
        private Button btnThoat;
        private Label lblFooter;
    }
}
