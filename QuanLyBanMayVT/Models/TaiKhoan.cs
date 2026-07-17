namespace QuanLyBanMayVT.Models
{
    /// <summary>
    /// Enum vai trò — khớp với giá trị chuỗi trong cột VaiTro của bảng TaiKhoan
    /// </summary>
    public enum VaiTro
    {
        KhachHang       = 0,
        NhanVienBanHang = 1,
        KeToan          = 2,
        QuanLy          = 3
    }

    /// <summary>
    /// Model ánh xạ bảng TaiKhoan
    /// </summary>
    public class TaiKhoan
    {
        public int    MaTaiKhoan  { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau     { get; set; } = string.Empty;
        public VaiTro VaiTro      { get; set; }
        public bool   TrangThai   { get; set; } = true;

        // ─── Chuyển chuỗi DB → enum ─────────────────────────────────────
        /// <summary>
        /// Parse chuỗi từ DB ('QuanLy', 'KeToan', ...) thành enum VaiTro
        /// </summary>
        public static VaiTro ParseVaiTro(string vaiTroStr) =>
            vaiTroStr.Trim() switch
            {
                "QuanLy"          => VaiTro.QuanLy,
                "KeToan"          => VaiTro.KeToan,
                "NhanVienBanHang" => VaiTro.NhanVienBanHang,
                "KhachHang"       => VaiTro.KhachHang,
                _                 => VaiTro.KhachHang   // mặc định an toàn
            };

        // ─── Chuyển enum → chuỗi để ghi vào DB ─────────────────────────
        /// <summary>
        /// Chuyển enum VaiTro thành chuỗi để INSERT/UPDATE vào DB
        /// </summary>
        public static string ToDbString(VaiTro vaiTro) =>
            vaiTro switch
            {
                VaiTro.QuanLy          => "QuanLy",
                VaiTro.KeToan          => "KeToan",
                VaiTro.NhanVienBanHang => "NhanVienBanHang",
                VaiTro.KhachHang       => "KhachHang",
                _                      => "KhachHang"
            };

        /// <summary>Tên hiển thị tiếng Việt của vai trò</summary>
        public string VaiTroDisplayName => VaiTro switch
        {
            VaiTro.QuanLy          => "Quản lý",
            VaiTro.KeToan          => "Kế toán",
            VaiTro.NhanVienBanHang => "Nhân viên bán hàng",
            VaiTro.KhachHang       => "Khách hàng",
            _                      => "Không xác định"
        };
    }
}
