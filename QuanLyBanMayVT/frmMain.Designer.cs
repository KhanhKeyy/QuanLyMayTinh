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
            this.menuStrip        = new MenuStrip();
            this.panelTop         = new Panel();
            this.lblTenNguoiDung  = new Label();
            this.lblVaiTro        = new Label();
            this.panelSidebar     = new Panel();
            this.panelContent     = new Panel();
            this.statusStrip      = new StatusStrip();
            this.lblStatus        = new ToolStripStatusLabel();

            // ── Menu items ──────────────────────────────────────────
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

            this.SuspendLayout();

            // ── menuStrip ────────────────────────────────────────────
            this.menuStrip.BackColor = Color.FromArgb(30, 41, 59);
            this.menuStrip.ForeColor = Color.White;
            this.menuStrip.Font = new Font("Segoe UI", 10F);
            this.menuStrip.Padding = new Padding(8, 4, 0, 4);
            this.menuStrip.Items.AddRange(new ToolStripItem[]
            {
                menuHangHoa, menuNhapHang, menuDonHang,
                menuHoaDon, menuTonKho, menuBaoCao,
                menuNhanVien, menuCaiDat, menuDangXuat
            });

            // ── Hàng hoá ─────────────────────────────────────────────
            SetMenuItem(menuHangHoa, "📦 Hàng hoá");
            menuHangHoa_XemDanhSach.Text = "Xem danh sách sản phẩm";
            menuHangHoa_Them.Text        = "Thêm sản phẩm mới";
            menuHangHoa_Sua.Text         = "Sửa thông tin sản phẩm";
            menuHangHoa_Xoa.Text         = "Xóa sản phẩm";
            menuHangHoa.DropDownItems.AddRange(new ToolStripItem[]
            {
                menuHangHoa_XemDanhSach, new ToolStripSeparator(),
                menuHangHoa_Them, menuHangHoa_Sua, menuHangHoa_Xoa
            });
            menuHangHoa_XemDanhSach.Click += menuHangHoa_XemDanhSach_Click;
            menuHangHoa_Them.Click         += menuHangHoa_Them_Click;

            // ── Nhập hàng ────────────────────────────────────────────
            SetMenuItem(menuNhapHang, "🚚 Nhập hàng");
            menuNhapHang_LapPhieu.Text = "Lập phiếu nhập hàng";
            menuNhapHang_DanhSach.Text = "Danh sách phiếu nhập";
            menuNhapHang.DropDownItems.AddRange(new ToolStripItem[]
            { menuNhapHang_LapPhieu, menuNhapHang_DanhSach });
            menuNhapHang_LapPhieu.Click += menuNhapHang_Click;
            menuNhapHang_DanhSach.Click += menuNhapHang_Click;

            // ── Đơn hàng ─────────────────────────────────────────────
            SetMenuItem(menuDonHang, "📋 Đơn hàng");
            menuDonHang_XemDanhSach.Text = "Xem tất cả đơn hàng";
            menuDonHang_XacNhan.Text     = "Xác nhận đơn hàng";
            menuDonHang.DropDownItems.AddRange(new ToolStripItem[]
            { menuDonHang_XemDanhSach, menuDonHang_XacNhan });
            menuDonHang_XemDanhSach.Click += menuDonHang_XemDanhSach_Click;
            menuDonHang_XacNhan.Click     += menuDonHang_XacNhan_Click;

            // ── Hóa đơn ──────────────────────────────────────────────
            SetMenuItem(menuHoaDon, "🧾 Hóa đơn");
            menuHoaDon_LapMoi.Text     = "Lập hóa đơn mới";
            menuHoaDon_DanhSach.Text   = "Danh sách hóa đơn";
            menuHoaDon.DropDownItems.AddRange(new ToolStripItem[]
            { menuHoaDon_LapMoi, menuHoaDon_DanhSach });
            menuHoaDon_LapMoi.Click   += menuHoaDon_LapMoi_Click;
            menuHoaDon_DanhSach.Click += menuHoaDon_Click;

            // ── Tồn kho ──────────────────────────────────────────────
            SetMenuItem(menuTonKho, "📊 Tồn kho");
            menuTonKho_XemTonKho.Text = "Xem tình trạng tồn kho";
            menuTonKho_CapNhat.Text   = "Cập nhật tồn kho";
            menuTonKho.DropDownItems.AddRange(new ToolStripItem[]
            { menuTonKho_XemTonKho, menuTonKho_CapNhat });
            menuTonKho_XemTonKho.Click += menuTonKho_Click;
            menuTonKho_CapNhat.Click   += menuTonKho_Click;

            // ── Báo cáo ──────────────────────────────────────────────
            SetMenuItem(menuBaoCao, "📈 Báo cáo");
            menuBaoCao_DoanhThu.Text = "Doanh thu";
            menuBaoCao_TonKho.Text   = "Hàng tồn kho";
            menuBaoCao_BanChay.Text  = "Sản phẩm bán chạy";
            menuBaoCao.DropDownItems.AddRange(new ToolStripItem[]
            { menuBaoCao_DoanhThu, menuBaoCao_TonKho, menuBaoCao_BanChay });
            menuBaoCao_DoanhThu.Click += menuBaoCao_DoanhThu_Click;
            menuBaoCao_TonKho.Click   += menuBaoCao_TonKho_Click;
            menuBaoCao_BanChay.Click  += menuBaoCao_BanChay_Click;

            // ── Nhân viên ────────────────────────────────────────────
            SetMenuItem(menuNhanVien, "👥 Nhân viên");
            menuNhanVien_DanhSach.Text = "Danh sách nhân viên";
            menuNhanVien.DropDownItems.Add(menuNhanVien_DanhSach);
            menuNhanVien_DanhSach.Click += menuNhanVien_Click;

            // ── Cài đặt ──────────────────────────────────────────────
            SetMenuItem(menuCaiDat, "⚙️ Danh mục SP");
            menuCaiDat.Click += menuCaiDat_Click;

            // ── Đăng xuất (căn phải) ─────────────────────────────────
            SetMenuItem(menuDangXuat, "🚪 Đăng xuất");
            menuDangXuat.Alignment = ToolStripItemAlignment.Right;
            menuDangXuat.ForeColor = Color.FromArgb(248, 113, 113);        // Đỏ nhạt
            menuDangXuat.Click += menuDangXuat_Click;

            // ── panelTop ─────────────────────────────────────────────
            this.panelTop.Dock = DockStyle.Top;
            this.panelTop.Height = 50;
            this.panelTop.BackColor = Color.FromArgb(15, 23, 42);
            this.panelTop.Padding = new Padding(16, 0, 16, 0);
            this.panelTop.Controls.Add(this.lblVaiTro);
            this.panelTop.Controls.Add(this.lblTenNguoiDung);

            // ── lblTenNguoiDung ──────────────────────────────────────
            this.lblTenNguoiDung.AutoSize = false;
            this.lblTenNguoiDung.Size = new Size(300, 50);
            this.lblTenNguoiDung.Location = new Point(16, 0);
            this.lblTenNguoiDung.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblTenNguoiDung.ForeColor = Color.White;
            this.lblTenNguoiDung.TextAlign = ContentAlignment.MiddleLeft;
            this.lblTenNguoiDung.Text = "";
            this.lblTenNguoiDung.Name = "lblTenNguoiDung";

            // ── lblVaiTro (badge) ────────────────────────────────────
            this.lblVaiTro.AutoSize = false;
            this.lblVaiTro.Size = new Size(160, 26);
            this.lblVaiTro.Location = new Point(340, 12);
            this.lblVaiTro.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblVaiTro.ForeColor = Color.White;
            this.lblVaiTro.TextAlign = ContentAlignment.MiddleCenter;
            this.lblVaiTro.BackColor = Color.FromArgb(71, 85, 105);
            this.lblVaiTro.Text = "";
            this.lblVaiTro.Name = "lblVaiTro";

            // ── panelContent ─────────────────────────────────────────
            this.panelContent.Dock = DockStyle.Fill;
            this.panelContent.BackColor = Color.FromArgb(15, 23, 42);
            this.panelContent.Padding = new Padding(8);
            this.panelContent.Name = "panelContent";
            this.panelContent.AutoScroll = true;

            // ── statusStrip ──────────────────────────────────────────
            this.statusStrip.BackColor = Color.FromArgb(30, 41, 59);
            this.statusStrip.ForeColor = Color.FromArgb(148, 163, 184);
            this.lblStatus = new ToolStripStatusLabel("© 2025 Quản Lý Bán Máy Vi Tính");
            this.statusStrip.Items.Add(lblStatus);

            // ── frmMain ──────────────────────────────────────────────
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1200, 700);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.statusStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "frmMain";
            this.Text = "Quản Lý Bán Máy Vi Tính";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(15, 23, 42);
            this.WindowState = FormWindowState.Maximized;
            this.Load += frmMain_Load;

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void SetMenuItem(ToolStripMenuItem item, string text)
        {
            item.Text = text;
            item.ForeColor = Color.White;
            item.BackColor = Color.FromArgb(30, 41, 59);
            item.Font = new Font("Segoe UI", 10F);
        }

        #endregion

        // Controls
        private MenuStrip menuStrip = null!;
        private Panel panelTop = null!;
        private Label lblTenNguoiDung = null!;
        private Label lblVaiTro = null!;
        private Panel panelSidebar = null!;
        internal Panel panelContent = null!;
        private StatusStrip statusStrip = null!;
        private ToolStripStatusLabel lblStatus = null!;

        // Menu items
        private ToolStripMenuItem menuHangHoa = null!;
        private ToolStripMenuItem menuHangHoa_XemDanhSach = null!;
        internal ToolStripMenuItem menuHangHoa_Them = null!;
        internal ToolStripMenuItem menuHangHoa_Sua = null!;
        internal ToolStripMenuItem menuHangHoa_Xoa = null!;

        private ToolStripMenuItem menuNhapHang = null!;
        private ToolStripMenuItem menuNhapHang_LapPhieu = null!;
        private ToolStripMenuItem menuNhapHang_DanhSach = null!;

        private ToolStripMenuItem menuDonHang = null!;
        private ToolStripMenuItem menuDonHang_XemDanhSach = null!;
        private ToolStripMenuItem menuDonHang_XacNhan = null!;

        private ToolStripMenuItem menuHoaDon = null!;
        private ToolStripMenuItem menuHoaDon_LapMoi = null!;
        private ToolStripMenuItem menuHoaDon_DanhSach = null!;

        private ToolStripMenuItem menuTonKho = null!;
        private ToolStripMenuItem menuTonKho_XemTonKho = null!;
        private ToolStripMenuItem menuTonKho_CapNhat = null!;

        private ToolStripMenuItem menuBaoCao = null!;
        private ToolStripMenuItem menuBaoCao_DoanhThu = null!;
        private ToolStripMenuItem menuBaoCao_TonKho = null!;
        private ToolStripMenuItem menuBaoCao_BanChay = null!;

        private ToolStripMenuItem menuNhanVien = null!;
        private ToolStripMenuItem menuNhanVien_DanhSach = null!;

        private ToolStripMenuItem menuCaiDat = null!;
        private ToolStripMenuItem menuDangXuat = null!;
    }
}
