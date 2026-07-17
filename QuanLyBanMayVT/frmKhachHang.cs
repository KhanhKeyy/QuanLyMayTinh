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

            // TODO: Tải danh sách sản phẩm từ DB vào DataGridView/ListView
            TaiDanhSachSanPham();
        }

        private void TaiDanhSachSanPham()
        {
            // TODO: Gọi SanPhamDAO.GetAll() và bind vào control hiển thị
            // Ví dụ:
            // var sanPhams = new SanPhamDAO().GetAll();
            // dgvSanPham.DataSource = sanPhams;
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
