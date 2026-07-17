using QuanLyBanMayVT.Common;

namespace QuanLyBanMayVT
{
    /// <summary>
    /// Form dành riêng cho Khách hàng: xem sản phẩm, đặt hàng, chọn phương thức thanh toán.
    /// </summary>
    public partial class frmKhachHang : Form
    {
        public frmKhachHang()
        {
            InitializeComponent();
        }

        private void frmKhachHang_Load(object sender, EventArgs e)
        {
            this.Text = $"Cửa hàng máy vi tính  |  {UserSession.DisplayName}";
            lblChaoMung.Text = $"Chào mừng, {UserSession.DisplayName}! 👋";

            // Căn nút Đăng xuất sát góc phải (không dùng Anchor vì form mở Maximized)
            panelTop.Resize += PanelTop_Resize;
            PanelTop_Resize(panelTop, EventArgs.Empty); // gọi ngay lần đầu

            // Căn giữa nội dung khi resize
            panelMain.Resize += PanelMain_Resize;
            PanelMain_Resize(panelMain, EventArgs.Empty);

            TaiDanhSachSanPham();
        }

        // ═══════════════════════════════════════════════════════════════
        // NÚT ĐĂNG XUẤT LUÔN BÁM GÓC PHẢI
        // ═══════════════════════════════════════════════════════════════
        private void PanelTop_Resize(object? sender, EventArgs e)
        {
            // Giữ khoảng cách 20px từ cạnh phải của panelTop
            btnDangXuat.Left = panelTop.ClientSize.Width - btnDangXuat.Width - 20;
        }

        private void TaiDanhSachSanPham()
        {
            // TODO: Gọi SanPhamDAO.GetAll() và bind vào control hiển thị
            // Ví dụ:
            // var sanPhams = new SanPhamDAO().GetAll();
            // dgvSanPham.DataSource = sanPhams;
        }

        // ═══════════════════════════════════════════════════════════════
        // CĂN GIỮA ĐỘNG — gọi mỗi khi panelMain thay đổi kích thước
        // ═══════════════════════════════════════════════════════════════
        private void PanelMain_Resize(object? sender, EventArgs e)
        {
            int w = panelMain.ClientSize.Width;
            int h = panelMain.ClientSize.Height;

            // Căn giữa theo chiều ngang và dọc, lệch lên một chút để nhường chỗ nút
            lblPlaceholder.Left = (w - lblPlaceholder.Width)  / 2;
            lblPlaceholder.Top  = (h - lblPlaceholder.Height) / 2 - 50;

            // Nút nằm ngay dưới label, cũng căn giữa ngang
            btnDatHang.Left = (w - btnDatHang.Width) / 2;
            btnDatHang.Top  = lblPlaceholder.Bottom + 20;
        }

        private void btnDatHang_Click(object sender, EventArgs e)
        {
            // TODO: Mở form xác nhận đơn hàng + chọn phương thức thanh toán
            MessageBox.Show("Chức năng đặt hàng đang được phát triển.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc muốn đăng xuất?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                this.Close();
        }
    }
}
