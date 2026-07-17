using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT.Common
{
    /// <summary>
    /// Lưu trữ thông tin phiên làm việc của người dùng hiện tại.
    /// Static → truy cập toàn cục trong toàn bộ ứng dụng.
    /// </summary>
    public static class UserSession
    {
        // ─── Thông tin tài khoản ────────────────────────────────────────
        public static TaiKhoan?  CurrentAccount   { get; set; }
        public static NhanVien?  CurrentNhanVien  { get; set; }
        public static KhachHang? CurrentKhachHang { get; set; }

        // ─── Thuộc tính kiểm tra quyền (dùng VaiTro trực tiếp) ─────────

        /// <summary>Đã đăng nhập chưa</summary>
        public static bool IsLoggedIn => CurrentAccount != null;

        /// <summary>Là Khách hàng</summary>
        public static bool IsKhachHang =>
            CurrentAccount?.VaiTro == VaiTro.KhachHang;

        /// <summary>Là Nhân viên bán hàng</summary>
        public static bool IsNVBanHang =>
            CurrentAccount?.VaiTro == VaiTro.NhanVienBanHang;

        /// <summary>Là Kế toán</summary>
        public static bool IsKeToan =>
            CurrentAccount?.VaiTro == VaiTro.KeToan;

        /// <summary>Là Quản lý</summary>
        public static bool IsQuanLy =>
            CurrentAccount?.VaiTro == VaiTro.QuanLy;

        /// <summary>Là bất kỳ nhân viên nào (bán hàng / kế toán / quản lý)</summary>
        public static bool IsNhanVien =>
            CurrentAccount?.VaiTro == VaiTro.NhanVienBanHang
            || CurrentAccount?.VaiTro == VaiTro.KeToan
            || CurrentAccount?.VaiTro == VaiTro.QuanLy;

        // ─── Tên hiển thị ──────────────────────────────────────────────
        public static string DisplayName
        {
            get
            {
                if (CurrentNhanVien  != null) return CurrentNhanVien.HoTen;
                if (CurrentKhachHang != null) return CurrentKhachHang.HoTen;
                return CurrentAccount?.TenDangNhap ?? "Khách";
            }
        }

        /// <summary>Xóa phiên → đăng xuất</summary>
        public static void Clear()
        {
            CurrentAccount   = null;
            CurrentNhanVien  = null;
            CurrentKhachHang = null;
        }
    }
}
