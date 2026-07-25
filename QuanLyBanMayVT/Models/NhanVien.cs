namespace QuanLyBanMayVT.Models
{
    public class NhanVien
    {
        public int       MaNhanVien  { get; set; }
        public string    HoTen       { get; set; } = string.Empty;
        public string    Email       { get; set; } = string.Empty;
        public string    SoDienThoai { get; set; } = string.Empty;
        /// <summary>Chức vụ trong công ty (lưu từ cột ChucVu trong bảng NhanVien)</summary>
        public string    ChucVu      { get; set; } = string.Empty;
        public int       MaTaiKhoan  { get; set; }
        public DateTime? NgayVaoLam  { get; set; }

        public string ChucVuDisplay => ChucVu switch
        {
            "NhanVienBanHang" => "Nhân viên bán hàng",
            "KeToan"          => "Kế toán",
            "QuanLy"          => "Quản lý",
            _                 => ChucVu
        };
    }
}

