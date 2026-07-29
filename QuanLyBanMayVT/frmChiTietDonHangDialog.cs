using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    /// <summary>
    /// Dialog Pop-up xem chi tiết danh sách sản phẩm trong đơn hàng
    /// </summary>
    public class frmChiTietDonHangDialog : Form
    {
        private readonly int _maDonHang;

        private Label lblThongTinDon = null!;
        private DataGridView dgvChiTiet = null!;
        private Label lblTongTien = null!;
        private Button btnDong = null!;

        public frmChiTietDonHangDialog(int maDonHang)
        {
            _maDonHang = maDonHang;
            InitUI();
            LoadChiTiet();
        }

        private void InitUI()
        {
            this.Text = $"🔍 Chi Tiết Đơn Hàng #{_maDonHang}";
            this.Size = new Size(680, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = UIStyleHelper.BgMain;
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F);

            // ── Header Panel ─────────────────────────────────────────────
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = UIStyleHelper.BgCard,
                Padding = new Padding(15, 10, 15, 10)
            };

            lblThongTinDon = new Label
            {
                Text = $"📦 ĐƠN HÀNG #{_maDonHang}",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlHeader.Controls.Add(lblThongTinDon);

            // ── Grid Chi Tiết ───────────────────────────────────────────
            dgvChiTiet = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvChiTiet);

            // ── Bottom Panel ────────────────────────────────────────────
            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 55,
                BackColor = UIStyleHelper.BgCard,
                Padding = new Padding(15, 10, 15, 10)
            };

            lblTongTien = new Label
            {
                Text = "Tổng tiền: 0 đ",
                AutoSize = true,
                Location = new Point(15, 16),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 211, 153)
            };

            btnDong = new Button
            {
                Text = "Đóng cửa sổ",
                Size = new Size(110, 34),
                Location = new Point(540, 10),
                BackColor = Color.FromArgb(100, 116, 139),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnDong.FlatAppearance.BorderSize = 0;
            btnDong.Click += (s, e) => this.Close();

            pnlBottom.Controls.Add(lblTongTien);
            pnlBottom.Controls.Add(btnDong);
            pnlBottom.Resize += (s, e) => { btnDong.Left = pnlBottom.ClientSize.Width - btnDong.Width - 15; };

            this.Controls.Add(dgvChiTiet);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlBottom);
        }

        private void LoadChiTiet()
        {
            var list = new DonHangDAO().GetChiTiet(_maDonHang);

            dgvChiTiet.DataSource = list.Select(c => new
            {
                c.TenSanPham,
                c.SoLuong,
                DonGia = c.DonGiaFormatted,
                ThanhTien = c.ThanhTienFormatted
            }).ToList();

            if (dgvChiTiet.Columns["TenSanPham"] != null) dgvChiTiet.Columns["TenSanPham"].HeaderText = "Sản Phẩm";
            if (dgvChiTiet.Columns["SoLuong"] != null) dgvChiTiet.Columns["SoLuong"].HeaderText = "Số Lượng";
            if (dgvChiTiet.Columns["DonGia"] != null) dgvChiTiet.Columns["DonGia"].HeaderText = "Đơn Giá";
            if (dgvChiTiet.Columns["ThanhTien"] != null) dgvChiTiet.Columns["ThanhTien"].HeaderText = "Thành Tiền";

            decimal total = list.Sum(x => x.ThanhTien);
            lblTongTien.Text = $"Tổng cộng: {total:N0} đ";
        }
    }
}
