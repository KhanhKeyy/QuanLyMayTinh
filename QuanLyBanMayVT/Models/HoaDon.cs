namespace QuanLyBanMayVT.Models
{
    public class HoaDon
    {
        public int MaHoaDon { get; set; }
        public int MaDonHang { get; set; }
        public int MaKeToan { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThaiThanhToan { get; set; } = "Chua thanh toan";
        public DateTime? NgayThanhToan { get; set; }

        // Navigation
        public string TenKeToan { get; set; } = "";
        public string TenKhachHang { get; set; } = "";
        public int MaKhachHang { get; set; }

        public string TongTienFormatted => TongTien.ToString("N0") + " đ";
        public string TrangThaiDisplay => TrangThaiThanhToan switch
        {
            "Chua thanh toan" => "⏳ Chưa thanh toán",
            "Da thanh toan"   => "✅ Đã thanh toán",
            "That bai"        => "❌ Thất bại",
            _                 => TrangThaiThanhToan
        };
    }
}
