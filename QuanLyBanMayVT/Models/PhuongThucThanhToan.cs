namespace QuanLyBanMayVT.Models
{
    public class PhuongThucThanhToan
    {
        public int MaPhuongThucTT { get; set; }
        public string TenPhuongThuc { get; set; } = "";

        public override string ToString() => TenPhuongThuc;
    }
}
