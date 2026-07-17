namespace QuanLyBanMayVT
{
    /// <summary>Form quản lý đơn hàng</summary>
    public class frmDonHang : Form
    {
        private readonly bool _cheBoDuyet;
        public frmDonHang(bool cheBoDuyet = false)
        {
            _cheBoDuyet = cheBoDuyet;
            this.Text = cheBoDuyet ? "Xác nhận đơn hàng" : "Danh sách đơn hàng";
            this.BackColor = Color.FromArgb(15, 23, 42);
            var lbl = new Label
            {
                Text = "📋 " + this.Text + "\n\n[Sẽ hiển thị danh sách đơn hàng từ CSDL]",
                Font = new Font("Segoe UI", 13F),
                ForeColor = Color.FromArgb(148, 163, 184),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lbl);
        }
    }

    /// <summary>Form quản lý hóa đơn</summary>
    public class frmHoaDon : Form
    {
        public frmHoaDon()
        {
            this.Text = "Hóa đơn bán hàng";
            this.BackColor = Color.FromArgb(15, 23, 42);
            var lbl = new Label
            {
                Text = "🧾 Lập hóa đơn\n\n[Sẽ hiển thị form lập hóa đơn bán hàng]",
                Font = new Font("Segoe UI", 13F),
                ForeColor = Color.FromArgb(148, 163, 184),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lbl);
        }
    }

    /// <summary>Form báo cáo thống kê</summary>
    public class frmBaoCao : Form
    {
        public frmBaoCao(string loaiBaoCao)
        {
            string tieuDe = loaiBaoCao switch
            {
                "DoanhThu" => "📈 Báo cáo doanh thu",
                "TonKho"   => "📊 Báo cáo hàng tồn kho",
                "BanChay"  => "🔥 Sản phẩm bán chạy",
                _          => "📋 Báo cáo"
            };
            this.Text = tieuDe;
            this.BackColor = Color.FromArgb(15, 23, 42);
            var lbl = new Label
            {
                Text = tieuDe + "\n\n[Sẽ hiển thị biểu đồ và thống kê từ CSDL]",
                Font = new Font("Segoe UI", 13F),
                ForeColor = Color.FromArgb(148, 163, 184),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lbl);
        }
    }
}
