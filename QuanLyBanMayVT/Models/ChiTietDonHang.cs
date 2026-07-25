namespace QuanLyBanMayVT.Models
{
    public class ChiTietDonHang
    {
        public int MaChiTiet { get; set; }
        public int MaDonHang { get; set; }
        public int MaSanPham { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;

        // Navigation
        public string TenSanPham { get; set; } = "";
        public string DonGiaFormatted => DonGia.ToString("N0") + " đ";
        public string ThanhTienFormatted => ThanhTien.ToString("N0") + " đ";
    }
}
