namespace QuanLyBanMayVT.Models
{
    public class DonHang
    {
        public int MaDonHang { get; set; }
        public int MaKhachHang { get; set; }
        public DateTime NgayDatHang { get; set; }
        public int MaPhuongThucTT { get; set; }
        public int? MaNhanVienXacNhan { get; set; }
        public string TrangThaiDonHang { get; set; } = "Cho xac nhan";
        public string? GhiChu { get; set; }

        // Navigation (join)
        public string TenKhachHang { get; set; } = "";
        public string TenPhuongThuc { get; set; } = "";
        public string TenNhanVienXacNhan { get; set; } = "";
        public decimal TongTien { get; set; }

        public string TrangThaiDisplay => TrangThaiDonHang switch
        {
            "Cho xac nhan" => "⏳ Chờ xác nhận",
            "Da xac nhan"  => "✅ Đã xác nhận",
            "Hoan tat"     => "🎉 Hoàn tất",
            "Da huy"       => "❌ Đã huỷ",
            _              => TrangThaiDonHang
        };
    }
}
