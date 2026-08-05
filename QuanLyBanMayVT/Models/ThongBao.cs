namespace QuanLyBanMayVT.Models
{
    public class ThongBao
    {
        public int MaThongBao { get; set; }
        public string LoaiThongBao { get; set; } = "";
        public string? NoiDung { get; set; }
        public DateTime NgayTao { get; set; }
        public int? MaTaiKhoanNhan { get; set; }
        public int? MaSanPham { get; set; }
        public int? MaDonHang { get; set; }
        public bool DaDoc { get; set; }

        public string LoaiDisplay => LoaiThongBao switch
        {
            "Ton kho thap"      => "⚠️ Tồn kho thấp",
            "Ket qua don hang"  => "📦 Kết quả đơn hàng",
            "De xuat sp"        => "💡 Đề xuất sản phẩm",
            _                   => LoaiThongBao
        };
    }
}
