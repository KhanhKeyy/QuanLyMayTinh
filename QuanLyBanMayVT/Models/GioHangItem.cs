namespace QuanLyBanMayVT.Models
{
    public class GioHangItem
    {
        public SanPham SanPham { get; set; } = null!;
        public int SoLuong { get; set; }

        public decimal ThanhTien => SanPham.GiaBan * SoLuong;
        public string ThanhTienFormatted => ThanhTien.ToString("N0") + " đ";
    }
}
