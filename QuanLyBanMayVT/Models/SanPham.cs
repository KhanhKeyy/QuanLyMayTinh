namespace QuanLyBanMayVT.Models
{
    public class SanPham
    {
        public int MaSanPham { get; set; }
        public int MaDanhMuc { get; set; }
        public string TenSanPham { get; set; } = "";
        public string? CauHinh { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuongTon { get; set; }
        public int MucTonToiThieu { get; set; }
        public string? HinhAnh { get; set; }
        public string TrangThai { get; set; } = "Con hang";

        // Navigation (join)
        public string TenDanhMuc { get; set; } = "";

        public bool ConHang => TrangThai == "Con hang" && SoLuongTon > 0;
        public bool DuoiMucToiThieu => SoLuongTon < MucTonToiThieu;
        public string GiaBanFormatted => GiaBan.ToString("N0") + " đ";
        public string TrangThaiDisplay => TrangThai switch
        {
            "Con hang"  => "Còn hàng",
            "Het hang"  => "Hết hàng",
            "Ngung KD"  => "Ngừng kinh doanh",
            _           => TrangThai
        };
    }
}
