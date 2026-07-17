namespace QuanLyBanMayVT.Models
{
    /// <summary>
    /// Model ánh xạ bảng KhachHang trong CSDL
    /// </summary>
    public class KhachHang
    {
        public int MaKhachHang { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public int MaTaiKhoan { get; set; }
    }
}
