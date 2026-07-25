namespace QuanLyBanMayVT.Models
{
    public class ChiTietPhieuNhap
    {
        public int MaChiTiet { get; set; }
        public int MaPhieuNhap { get; set; }
        public int MaSanPham { get; set; }
        public int SoLuongNhap { get; set; }
        public decimal DonGiaNhap { get; set; }
        public decimal ThanhTien => SoLuongNhap * DonGiaNhap;

        // Navigation
        public string TenSanPham { get; set; } = "";
        public string DonGiaNhapFormatted => DonGiaNhap.ToString("N0") + " đ";
        public string ThanhTienFormatted => ThanhTien.ToString("N0") + " đ";
    }
}
