using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    public partial class frmDangNhap : Form
    {
        private readonly TaiKhoanDAO _taiKhoanDAO = new();
        private readonly NhanVienDAO _nhanVienDAO = new();
        private readonly KhachHangDAO _khachHangDAO = new();

        public frmDangNhap()
        {
            InitializeComponent();
        }

        // ═══════════════════════════════════════════════════════════════
        // SỰ KIỆN FORM LOAD
        // ═══════════════════════════════════════════════════════════════
        private void frmDangNhap_Load(object sender, EventArgs e)
        {
            txtTenDangNhap.Focus();

            // Cho phép nhấn Enter để đăng nhập
            this.AcceptButton = btnDangNhap;
        }

        // ═══════════════════════════════════════════════════════════════
        // SỰ KIỆN NÚT ĐĂNG NHẬP - LOGIC PHÂN QUYỀN CHÍNH
        // ═══════════════════════════════════════════════════════════════
        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra nhập liệu
            if (string.IsNullOrWhiteSpace(txtTenDangNhap.Text))
            {
                ShowError("Vui lòng nhập tên đăng nhập!");
                txtTenDangNhap.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                ShowError("Vui lòng nhập mật khẩu!");
                txtMatKhau.Focus();
                return;
            }

            // 2. Hiện trạng thái đang xử lý
            btnDangNhap.Enabled = false;
            btnDangNhap.Text = "Đang xử lý...";

            try
            {
                // 3. Xác thực tài khoản từ CSDL
                var taiKhoan = _taiKhoanDAO.XacThucDangNhap(
                    txtTenDangNhap.Text.Trim(),
                    txtMatKhau.Text   // Nếu DB lưu hash, hãy hash ở đây trước
                );

                if (taiKhoan == null)
                {
                    ShowError("Tên đăng nhập hoặc mật khẩu không đúng!\nVui lòng thử lại.");
                    txtMatKhau.Clear();
                    txtMatKhau.Focus();
                    return;
                }

                // 4. Lưu thông tin tài khoản vào Session
                UserSession.CurrentAccount = taiKhoan;

                // 5. Tải thêm thông tin theo vai trò
                switch (taiKhoan.VaiTro)
                {
                    case VaiTro.NhanVienBanHang:
                    case VaiTro.KeToan:
                    case VaiTro.QuanLy:
                        UserSession.CurrentNhanVien =
                            _nhanVienDAO.GetByMaTaiKhoan(taiKhoan.MaTaiKhoan);
                        break;

                    case VaiTro.KhachHang:
                        UserSession.CurrentKhachHang =
                            _khachHangDAO.GetByMaTaiKhoan(taiKhoan.MaTaiKhoan);
                        break;
                }

                // 6. Chuyển hướng đến Form phù hợp theo quyền
                ChuyenHuongTheoQuyen(taiKhoan.VaiTro);
            }
            finally
            {
                // Khôi phục nút dù thành công hay thất bại
                btnDangNhap.Enabled = true;
                btnDangNhap.Text = "Đăng nhập";
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // LOGIC CHUYỂN HƯỚNG THEO VAI TRÒ
        // ═══════════════════════════════════════════════════════════════
        private void ChuyenHuongTheoQuyen(VaiTro vaiTro)
        {
            Form formChinh;

            switch (vaiTro)
            {
                case VaiTro.KhachHang:
                    // Khách hàng → Form xem sản phẩm & đặt hàng
                    formChinh = new frmKhachHang();
                    break;

                case VaiTro.NhanVienBanHang:
                case VaiTro.KeToan:
                case VaiTro.QuanLy:
                    // Mọi nhân viên → Form chính, menu hiện theo vai trò
                    formChinh = new frmMain();
                    break;

                default:
                    ShowError("Vai trò không xác định. Liên hệ quản trị viên.");
                    UserSession.Clear();
                    return;
            }

            // Ẩn form đăng nhập và mở form mới
            this.Hide();
            formChinh.FormClosed += (s, args) =>
            {
                // Khi đóng form chính → đăng xuất và hiện lại đăng nhập
                UserSession.Clear();
                txtMatKhau.Clear();
                txtTenDangNhap.Clear();
                txtTenDangNhap.Focus();
                this.Show();
            };
            formChinh.Show();
        }

        // ═══════════════════════════════════════════════════════════════
        // NÚT THOÁT
        // ═══════════════════════════════════════════════════════════════
        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // ═══════════════════════════════════════════════════════════════
        // NÚT HIỆN/ẨN MẬT KHẨU
        // ═══════════════════════════════════════════════════════════════
        private void btnTogglePassword_Click(object sender, EventArgs e)
        {
            if (txtMatKhau.PasswordChar == '*')
            {
                txtMatKhau.PasswordChar = '\0';
                btnTogglePassword.Text = "🙈";
            }
            else
            {
                txtMatKhau.PasswordChar = '*';
                btnTogglePassword.Text = "👁";
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // HÀM TIỆN ÍCH
        // ═══════════════════════════════════════════════════════════════
        private static void ShowError(string message)
        {
            MessageBox.Show(message, "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // ═══════════════════════════════════════════════════════════════
        // HIỆU ỨNG HOVER NÚT ĐĂNG NHẬP
        // ═══════════════════════════════════════════════════════════════
        private void btnDangNhap_MouseEnter(object sender, EventArgs e)
        {
            btnDangNhap.BackColor = Color.FromArgb(31, 41, 55);
        }

        private void btnDangNhap_MouseLeave(object sender, EventArgs e)
        {
            btnDangNhap.BackColor = Color.FromArgb(17, 24, 39);
        }
    }
}
