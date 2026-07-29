namespace QuanLyBanMayVT
{
    partial class frmMain
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
            this.panelSidebar     = new Panel();
            this.panelSidebarUser = new Panel();
            this.panelTop         = new Panel();
            this.panelContent     = new Panel();
            this.statusStrip      = new StatusStrip();
            this.lblStatus        = new ToolStripStatusLabel();
            this.lblTenNguoiDung  = new Label();
            this.lblVaiTro        = new Label();
            this.btnDangXuat      = new Button();
            this.btnDashboard     = new Button();
            this.btnHangHoa       = new Button();
            this.btnNhapHang      = new Button();
            this.btnDonHang       = new Button();
            this.btnHoaDon        = new Button();
            this.btnTonKho        = new Button();
            this.btnBaoCao        = new Button();
            this.btnNhanVien      = new Button();
            this.btnDanhMuc       = new Button();
            this.menuStrip               = new MenuStrip();
            this.menuStrip.Visible       = false;
            this.menuHangHoa             = new ToolStripMenuItem();
            this.menuHangHoa_XemDanhSach = new ToolStripMenuItem();
            this.menuHangHoa_Them        = new ToolStripMenuItem();
            this.menuHangHoa_Sua         = new ToolStripMenuItem();
            this.menuHangHoa_Xoa         = new ToolStripMenuItem();
            this.menuNhapHang            = new ToolStripMenuItem();
            this.menuNhapHang_LapPhieu   = new ToolStripMenuItem();
            this.menuNhapHang_DanhSach   = new ToolStripMenuItem();
            this.menuDonHang             = new ToolStripMenuItem();
            this.menuDonHang_XemDanhSach = new ToolStripMenuItem();
            this.menuDonHang_XacNhan     = new ToolStripMenuItem();
            this.menuHoaDon              = new ToolStripMenuItem();
            this.menuHoaDon_LapMoi       = new ToolStripMenuItem();
            this.menuHoaDon_DanhSach     = new ToolStripMenuItem();
            this.menuTonKho              = new ToolStripMenuItem();
            this.menuTonKho_XemTonKho    = new ToolStripMenuItem();
            this.menuTonKho_CapNhat      = new ToolStripMenuItem();
            this.menuBaoCao              = new ToolStripMenuItem();
            this.menuBaoCao_DoanhThu     = new ToolStripMenuItem();
            this.menuBaoCao_TonKho       = new ToolStripMenuItem();
            this.menuBaoCao_BanChay      = new ToolStripMenuItem();
            this.menuNhanVien            = new ToolStripMenuItem();
            this.menuNhanVien_DanhSach   = new ToolStripMenuItem();
            this.menuCaiDat              = new ToolStripMenuItem();
            this.menuDangXuat            = new ToolStripMenuItem();

            menuHangHoa_XemDanhSach.Click += menuHangHoa_XemDanhSach_Click;
            menuHangHoa_Them.Click        += menuHangHoa_Them_Click;
            menuNhapHang_LapPhieu.Click   += menuNhapHang_Click;
            menuNhapHang_DanhSach.Click   += menuNhapHang_Click;
            menuDonHang_XemDanhSach.Click += menuDonHang_XemDanhSach_Click;
            menuDonHang_XacNhan.Click     += menuDonHang_XacNhan_Click;
            menuHoaDon_LapMoi.Click       += menuHoaDon_LapMoi_Click;
            menuHoaDon_DanhSach.Click     += menuHoaDon_Click;
            menuTonKho_XemTonKho.Click    += menuTonKho_Click;
            menuTonKho_CapNhat.Click      += menuTonKho_Click;
            menuBaoCao_DoanhThu.Click     += menuBaoCao_DoanhThu_Click;
            menuBaoCao_TonKho.Click       += menuBaoCao_TonKho_Click;
            menuBaoCao_BanChay.Click      += menuBaoCao_BanChay_Click;
            menuNhanVien_DanhSach.Click   += menuNhanVien_Click;
            menuCaiDat.Click              += menuCaiDat_Click;
            menuDangXuat.Click            += menuDangXuat_Click;

            this.SuspendLayout();

            // ================================================================
            // SIDEBAR 260px wide
            // ================================================================
            this.panelSidebar.Dock       = DockStyle.Left;
            this.panelSidebar.Width      = 260;
            this.panelSidebar.BackColor  = Color.White;
            this.panelSidebar.Name       = "panelSidebar";
            this.panelSidebar.AutoScroll = true;
            this.panelSidebar.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(229, 231, 235), 1),
                    panelSidebar.Width - 1, 0, panelSidebar.Width - 1, panelSidebar.Height);

            // User Info Panel (Top of sidebar - 48px height, matching top bar)
            this.panelSidebarUser.Location  = new Point(0, 0);
            this.panelSidebarUser.Size      = new Size(260, 48);
            this.panelSidebarUser.BackColor = Color.FromArgb(248, 250, 252);
            this.panelSidebarUser.Name      = "panelSidebarUser";
            this.panelSidebarUser.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(229, 231, 235), 1),
                    0, panelSidebarUser.Height - 1, panelSidebarUser.Width, panelSidebarUser.Height - 1);

            // User Name Label (Font 11pt Bold)
            this.lblTenNguoiDung.AutoSize  = true;
            this.lblTenNguoiDung.Location  = new Point(14, 13);
            this.lblTenNguoiDung.Font      = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            this.lblTenNguoiDung.ForeColor = Color.FromArgb(17, 24, 39);
            this.lblTenNguoiDung.TextAlign = ContentAlignment.MiddleLeft;
            this.lblTenNguoiDung.Name      = "lblTenNguoiDung";

            // User Role Badge Label (Font 9pt Bold)
            this.lblVaiTro.AutoSize  = true;
            this.lblVaiTro.Padding   = new Padding(8, 3, 8, 3);
            this.lblVaiTro.Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            this.lblVaiTro.ForeColor = Color.White;
            this.lblVaiTro.TextAlign = ContentAlignment.MiddleCenter;
            this.lblVaiTro.BackColor = Color.FromArgb(124, 58, 237);
            this.lblVaiTro.Name      = "lblVaiTro";

            this.panelSidebarUser.Controls.Add(this.lblTenNguoiDung);
            this.panelSidebarUser.Controls.Add(this.lblVaiTro);

            // Nav buttons - start at y=52 (below 48px user panel + 4px gap)
            int btnY = 52;
            SetupSidebarBtn(this.btnDashboard, "Dashboard", ref btnY, true);
            SetupSidebarBtn(this.btnHangHoa,   "H\u00e0ng ho\u00e1", ref btnY, false);
            SetupSidebarBtn(this.btnNhapHang,  "Nh\u1eadp h\u00e0ng", ref btnY, false);
            SetupSidebarBtn(this.btnDonHang,   "\u0110\u01a1n h\u00e0ng", ref btnY, false);
            SetupSidebarBtn(this.btnHoaDon,    "H\u00f3a \u0111\u01a1n", ref btnY, false);
            SetupSidebarBtn(this.btnTonKho,    "T\u1ed3n kho", ref btnY, false);
            SetupSidebarBtn(this.btnBaoCao,    "B\u00e1o c\u00e1o", ref btnY, false);
            SetupSidebarBtn(this.btnNhanVien,  "Nh\u00e2n vi\u00ean", ref btnY, false);
            SetupSidebarBtn(this.btnDanhMuc,   "Danh m\u1ee5c SP", ref btnY, false);

            this.panelSidebar.Controls.Add(this.panelSidebarUser);
            this.panelSidebar.Controls.AddRange(new Control[]
            {
                this.btnDanhMuc,  this.btnNhanVien, this.btnBaoCao,
                this.btnTonKho,   this.btnHoaDon,   this.btnDonHang,
                this.btnNhapHang, this.btnHangHoa,  this.btnDashboard
            });

            // ================================================================
            // TOP BAR
            // ================================================================
            this.panelTop.Dock      = DockStyle.Top;
            this.panelTop.Height    = 48;
            this.panelTop.BackColor = Color.White;
            this.panelTop.Name      = "panelTop";
            this.panelTop.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(229, 231, 235), 1),
                    0, panelTop.Height - 1, panelTop.Width, panelTop.Height - 1);

            this.btnDangXuat.Text      = "\u0110\u0103ng xu\u1ea5t";
            this.btnDangXuat.Size      = new Size(110, 32);
            this.btnDangXuat.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnDangXuat.ForeColor = Color.FromArgb(220, 38, 38);
            this.btnDangXuat.BackColor = Color.FromArgb(254, 242, 242);
            this.btnDangXuat.FlatStyle = FlatStyle.Flat;
            this.btnDangXuat.FlatAppearance.BorderColor = Color.FromArgb(252, 165, 165);
            this.btnDangXuat.FlatAppearance.BorderSize  = 1;
            this.btnDangXuat.FlatAppearance.MouseOverBackColor = Color.FromArgb(254, 226, 226);
            this.btnDangXuat.Cursor    = Cursors.Hand;
            this.btnDangXuat.Name      = "btnDangXuat";
            this.btnDangXuat.UseCompatibleTextRendering = true;
            this.btnDangXuat.Click    += menuDangXuat_Click;
            this.panelTop.Controls.Add(this.btnDangXuat);

            // ================================================================
            // CONTENT AREA
            // ================================================================
            this.panelContent.Dock       = DockStyle.Fill;
            this.panelContent.BackColor  = Color.FromArgb(248, 250, 252);
            this.panelContent.Padding    = new Padding(0);
            this.panelContent.Name       = "panelContent";
            this.panelContent.AutoScroll = true;

            // ================================================================
            // STATUS STRIP
            // ================================================================
            this.statusStrip.BackColor  = Color.White;
            this.statusStrip.ForeColor  = Color.FromArgb(107, 114, 128);
            this.statusStrip.SizingGrip = false;
            this.lblStatus = new ToolStripStatusLabel("\u00a9 2026 Qu\u1ea3n L\u00fd B\u00e1n M\u00e1y Vi T\u00ednh");
            this.statusStrip.Items.Add(lblStatus);

            // ================================================================
            // FORM
            // ================================================================
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode       = AutoScaleMode.Dpi;
            this.ClientSize          = new Size(1200, 700);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelSidebar);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.statusStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name          = "frmMain";
            this.Text          = "Qu\u1ea3n L\u00fd B\u00e1n M\u00e1y Vi T\u00ednh";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor     = Color.FromArgb(248, 250, 252);
            this.WindowState   = FormWindowState.Maximized;
            this.Load         += frmMain_Load;
            this.Resize       += frmMain_Resize;

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void SetupSidebarBtn(Button btn, string text, ref int y, bool active)
        {
            btn.Text      = text;
            btn.Location  = new Point(0, y);
            btn.Size      = new Size(260, 44);
            btn.Font      = new Font("Segoe UI", 9.5F, active ? FontStyle.Bold : FontStyle.Regular);
            btn.ForeColor = active ? Color.FromArgb(37, 99, 235) : Color.FromArgb(55, 65, 81);
            btn.BackColor = active ? Color.FromArgb(239, 246, 255) : Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize           = 0;
            btn.FlatAppearance.MouseOverBackColor   = Color.FromArgb(239, 246, 255);
            btn.FlatAppearance.MouseDownBackColor   = Color.FromArgb(219, 234, 254);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding   = new Padding(16, 2, 4, 2);
            btn.Cursor    = Cursors.Hand;
            btn.UseCompatibleTextRendering = true;
            btn.AutoEllipsis = false;
            y += 44;
        }

        #endregion

        // Controls
        private MenuStrip menuStrip         = null!;
        private Panel     panelSidebar      = null!;
        private Panel     panelSidebarUser  = null!;
        private Panel     panelTop          = null!;
        private Label     lblTenNguoiDung   = null!;
        private Label     lblVaiTro         = null!;
        internal Panel    panelContent      = null!;
        private StatusStrip          statusStrip = null!;
        private ToolStripStatusLabel lblStatus   = null!;

        private Button btnDashboard = null!;
        private Button btnHangHoa   = null!;
        private Button btnNhapHang  = null!;
        private Button btnDonHang   = null!;
        private Button btnHoaDon    = null!;
        private Button btnTonKho    = null!;
        private Button btnBaoCao    = null!;
        private Button btnNhanVien  = null!;
        private Button btnDanhMuc   = null!;
        private Button btnDangXuat  = null!;

        private ToolStripMenuItem menuHangHoa          = null!;
        private ToolStripMenuItem menuHangHoa_XemDanhSach = null!;
        internal ToolStripMenuItem menuHangHoa_Them    = null!;
        internal ToolStripMenuItem menuHangHoa_Sua     = null!;
        internal ToolStripMenuItem menuHangHoa_Xoa     = null!;
        private ToolStripMenuItem menuNhapHang           = null!;
        private ToolStripMenuItem menuNhapHang_LapPhieu  = null!;
        private ToolStripMenuItem menuNhapHang_DanhSach  = null!;
        private ToolStripMenuItem menuDonHang              = null!;
        private ToolStripMenuItem menuDonHang_XemDanhSach  = null!;
        private ToolStripMenuItem menuDonHang_XacNhan      = null!;
        private ToolStripMenuItem menuHoaDon           = null!;
        private ToolStripMenuItem menuHoaDon_LapMoi    = null!;
        private ToolStripMenuItem menuHoaDon_DanhSach  = null!;
        private ToolStripMenuItem menuTonKho            = null!;
        private ToolStripMenuItem menuTonKho_XemTonKho  = null!;
        private ToolStripMenuItem menuTonKho_CapNhat    = null!;
        private ToolStripMenuItem menuBaoCao            = null!;
        private ToolStripMenuItem menuBaoCao_DoanhThu   = null!;
        private ToolStripMenuItem menuBaoCao_TonKho     = null!;
        private ToolStripMenuItem menuBaoCao_BanChay    = null!;
        private ToolStripMenuItem menuNhanVien          = null!;
        private ToolStripMenuItem menuNhanVien_DanhSach = null!;
        private ToolStripMenuItem menuCaiDat   = null!;
        private ToolStripMenuItem menuDangXuat = null!;
    }
}
