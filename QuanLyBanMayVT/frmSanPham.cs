namespace QuanLyBanMayVT
{
    /// <summary>
    /// Form quản lý sản phẩm - dùng chung cho xem và chỉnh sửa.
    /// Tham số cheBoDuyet = true → bật chức năng thêm/sửa/xóa (cho Quản lý).
    /// </summary>
    public class frmSanPham : Form
    {
        private readonly bool _cheBoDuyet;

        public frmSanPham(bool cheBoDuyet = false)
        {
            _cheBoDuyet = cheBoDuyet;
            InitUI();
        }

        private void InitUI()
        {
            this.Text = _cheBoDuyet ? "Quản lý sản phẩm" : "Danh sách sản phẩm";
            this.BackColor = Color.FromArgb(15, 23, 42);

            var lbl = new Label
            {
                Text = "📦 " + this.Text + "\n\n[Sẽ hiển thị danh sách sản phẩm từ CSDL]",
                Font = new Font("Segoe UI", 13F),
                ForeColor = Color.FromArgb(148, 163, 184),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lbl);
        }
    }
}
