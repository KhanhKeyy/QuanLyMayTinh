using System;

namespace QuanLyBanMayVT.Models
{
    public class YeuCauThemSanPham
    {
        public int MaYeuCau { get; set; }
        public int MaNhanVienDeXuat { get; set; }
        public int MaDanhMuc { get; set; }
        public string TenSanPham { get; set; } = "";
        public string CauHinh { get; set; } = "";
        public decimal GiaBan { get; set; }
        public int SoLuongTon { get; set; }
        public int MucTonToiThieu { get; set; }
        public string LyDoDeXuat { get; set; } = "";
        public string TrangThai { get; set; } = "Cho duyet"; // Cho duyet, Da duyet, Tu choi
        public DateTime NgayDeXuat { get; set; } = DateTime.Now;
        public DateTime? NgayDuyet { get; set; }
        public int? MaNhanVienDuyet { get; set; }
        public string GhiChuDuyet { get; set; } = "";

        // Navigation Display Properties
        public string TenNhanVienDeXuat { get; set; } = "";
        public string TenNhanVienDuyet { get; set; } = "";
        public string TenDanhMuc { get; set; } = "";

        public string GiaBanFormatted => GiaBan.ToString("N0") + " đ";

        public string TrangThaiDisplay => TrangThai switch
        {
            "Cho duyet" => "⏳ Chờ duyệt",
            "Da duyet"  => "✅ Đã duyệt",
            "Tu choi"   => "❌ Từ chối",
            _           => TrangThai
        };
    }
}
