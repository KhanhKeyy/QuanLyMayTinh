namespace QuanLyBanMayVT.Models
{
    public class PhieuNhapHang
    {
        public int MaPhieuNhap { get; set; }
        public int MaQuanLy { get; set; }
        public DateTime NgayNhap { get; set; }
        public string TrangThai { get; set; } = "Cho kiem tra";

        // Navigation
        public string TenQuanLy { get; set; } = "";
        public decimal TongGiaTri { get; set; }

        public string TrangThaiDisplay => TrangThai switch
        {
            "Cho kiem tra" => "⏳ Chờ kiểm tra",
            "Da nhap kho"  => "✅ Đã nhập kho",
            _              => TrangThai
        };
    }
}
