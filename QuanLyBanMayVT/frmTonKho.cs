using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;

namespace QuanLyBanMayVT
{
    public class frmTonKho : Form
    {
        private DataGridView dgvTonKho = null!;
        private Button btnCapNhat = null!;
        private NumericUpDown numSoLuongTon = null!;
        private Label lblSelectedSP = null!;
        private int _selectedId = 0;

        public frmTonKho()
        {
            InitUI();
            LoadData();
        }

        private void InitUI()
        {
            this.Text = "Tình trạng tồn kho";
            this.BackColor = UIStyleHelper.BgMain;
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F);

            dgvTonKho = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvTonKho);
            dgvTonKho.SelectionChanged += DgvTonKho_SelectionChanged;

            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 65,
                Padding = new Padding(12),
                BackColor = UIStyleHelper.BgCard
            };

            lblSelectedSP = new Label
            {
                Text = "Sản phẩm chọn: (chưa chọn)",
                Location = new Point(12, 22),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(99, 179, 237)
            };

            var lblSL = new Label { Text = "Số lượng tồn mới:", Location = new Point(360, 22), AutoSize = true, ForeColor = UIStyleHelper.TextMuted };
            numSoLuongTon = new NumericUpDown { Location = new Point(485, 18), Width = 110, Maximum = 10000 };
            UIStyleHelper.StyleNumeric(numSoLuongTon);

            btnCapNhat = new Button
            {
                Text = "💾 Cập nhật tồn kho",
                Location = new Point(610, 15),
                Size = new Size(170, 35),
                BackColor = UIStyleHelper.SuccessGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCapNhat.FlatAppearance.BorderSize = 0;
            btnCapNhat.Click += BtnCapNhat_Click;

            pnlBottom.Controls.Add(lblSelectedSP);
            pnlBottom.Controls.Add(lblSL);
            pnlBottom.Controls.Add(numSoLuongTon);
            pnlBottom.Controls.Add(btnCapNhat);

            this.Controls.Add(dgvTonKho);
            this.Controls.Add(pnlBottom);
        }

        private void LoadData()
        {
            var list = new SanPhamDAO().GetAll();
            dgvTonKho.DataSource = list.Select(p => new
            {
                p.MaSanPham,
                p.TenSanPham,
                p.TenDanhMuc,
                p.SoLuongTon,
                p.MucTonToiThieu,
                CanhBao = p.DuoiMucToiThieu ? "⚠️ THẤP" : "OK",
                TrangThai = p.TrangThaiDisplay
            }).ToList();

            if (dgvTonKho.Columns["MaSanPham"] != null) dgvTonKho.Columns["MaSanPham"].HeaderText = "Mã SP";
            if (dgvTonKho.Columns["TenSanPham"] != null) dgvTonKho.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";
            if (dgvTonKho.Columns["TenDanhMuc"] != null) dgvTonKho.Columns["TenDanhMuc"].HeaderText = "Danh Mục";
            if (dgvTonKho.Columns["SoLuongTon"] != null) dgvTonKho.Columns["SoLuongTon"].HeaderText = "Số Lượng Tồn";
            if (dgvTonKho.Columns["MucTonToiThieu"] != null) dgvTonKho.Columns["MucTonToiThieu"].HeaderText = "Mức Tối Thiểu";
            if (dgvTonKho.Columns["CanhBao"] != null) dgvTonKho.Columns["CanhBao"].HeaderText = "Cảnh Báo";
            if (dgvTonKho.Columns["TrangThai"] != null) dgvTonKho.Columns["TrangThai"].HeaderText = "Trạng Thái";
        }

        private void DgvTonKho_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvTonKho.CurrentRow == null) return;
            _selectedId = (int)dgvTonKho.CurrentRow.Cells["MaSanPham"].Value;
            var sp = new SanPhamDAO().GetById(_selectedId);
            if (sp == null) return;

            lblSelectedSP.Text = $"Sản phẩm: {sp.TenSanPham}";
            numSoLuongTon.Value = sp.SoLuongTon;
        }

        private void BtnCapNhat_Click(object? sender, EventArgs e)
        {
            if (_selectedId <= 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần cập nhật tồn kho.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var sp = new SanPhamDAO().GetById(_selectedId);
            if (sp == null) return;

            int delta = (int)numSoLuongTon.Value - sp.SoLuongTon;
            if (new SanPhamDAO().CapNhatSoLuongTon(_selectedId, delta))
            {
                MessageBox.Show("Cập nhật số lượng tồn kho thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
        }
    }
}
