namespace QuanLyBanMayVT.Models
{
    public class DanhMucSanPham
    {
        public int MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; } = "";
        public string? MoTa { get; set; }

        public override string ToString() => TenDanhMuc;
    }
}
