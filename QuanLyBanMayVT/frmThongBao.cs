using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    public partial class frmThongBao : Form
    {
        private DataGridView dgvThongBao = null!;
        private Label lblThongKe = null!;
        private Button btnDanhDauDaDoc = null!;
        private int _maTaiKhoan;

        public frmThongBao()
        {
            InitializeComponent();
            _maTaiKhoan = UserSession.CurrentAccount?.MaTaiKhoan ?? 0;
        }

        private void InitializeComponent()
        {
            this.Text = "🔔 Danh Sách Thông Báo";
            this.Size = new Size(720, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = UIStyleHelper.BgMain;
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F);

            // ── Top Panel ─────────────────────────────────────────────
            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = UIStyleHelper.BgCard,
                Padding = new Padding(16, 10, 16, 10)
            };

            lblThongKe = new Label
            {
                Text = "🔔 Đang tải thông báo...",
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(16, 16)
            };

            btnDanhDauDaDoc = new Button
            {
                Text = "✅ Đánh dấu tất cả đã đọc",
                AutoSize = true,
                Padding = new Padding(14, 6, 14, 6),
                BackColor = UIStyleHelper.PrimaryBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnDanhDauDaDoc.FlatAppearance.BorderSize = 0;
            btnDanhDauDaDoc.Click += BtnDanhDauDaDoc_Click;

            pnlTop.Controls.Add(lblThongKe);
            pnlTop.Controls.Add(btnDanhDauDaDoc);
            pnlTop.Resize += (s, e) =>
            {
                btnDanhDauDaDoc.Left = pnlTop.ClientSize.Width - btnDanhDauDaDoc.Width - 16;
            };

            // ── Grid Thông Báo ────────────────────────────────────────
            dgvThongBao = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvThongBao);
            dgvThongBao.CellClick += DgvThongBao_CellClick;

            this.Controls.Add(dgvThongBao);
            this.Controls.Add(pnlTop);

            this.Load += (s, e) => TaiDanhSachThongBao();
        }

        public void TaiDanhSachThongBao()
        {
            if (_maTaiKhoan == 0) return;

            var list = new ThongBaoDAO().GetByMaTaiKhoan(_maTaiKhoan);
            int chuaDoc = list.Count(t => !t.DaDoc);

            lblThongKe.Text = chuaDoc > 0
                ? $"🔔 Có {chuaDoc} thông báo chưa đọc"
                : "🔔 Không có thông báo mới";

            dgvThongBao.DataSource = list.Select(t => new
            {
                t.MaThongBao,
                Loai = t.LoaiDisplay,
                NoiDung = t.NoiDung,
                NgayTao = t.NgayTao.ToString("dd/MM/yyyy HH:mm"),
                TrangThai = t.DaDoc ? "✅ Đã đọc" : "🔴 Chưa đọc"
            }).ToList();

            if (dgvThongBao.Columns["MaThongBao"] != null) dgvThongBao.Columns["MaThongBao"].HeaderText = "Mã TB";
            if (dgvThongBao.Columns["Loai"]        != null) dgvThongBao.Columns["Loai"].HeaderText        = "Loại Thông Báo";
            if (dgvThongBao.Columns["NoiDung"]     != null) dgvThongBao.Columns["NoiDung"].HeaderText     = "Nội Dung";
            if (dgvThongBao.Columns["NgayTao"]     != null) dgvThongBao.Columns["NgayTao"].HeaderText     = "Thời Gian";
            if (dgvThongBao.Columns["TrangThai"]   != null) dgvThongBao.Columns["TrangThai"].HeaderText   = "Trạng Thái";
        }

        private void DgvThongBao_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvThongBao.CurrentRow == null) return;
            int maTB = (int)dgvThongBao.CurrentRow.Cells["MaThongBao"].Value;
            new ThongBaoDAO().DanhDauDaDoc(maTB);
            TaiDanhSachThongBao();
        }

        private void BtnDanhDauDaDoc_Click(object? sender, EventArgs e)
        {
            if (_maTaiKhoan == 0) return;
            var list = new ThongBaoDAO().GetByMaTaiKhoan(_maTaiKhoan, chiChuaDoc: true);
            var dao = new ThongBaoDAO();
            foreach (var tb in list)
            {
                dao.DanhDauDaDoc(tb.MaThongBao);
            }
            TaiDanhSachThongBao();
        }
    }
}
