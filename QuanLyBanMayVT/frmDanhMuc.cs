using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    public class frmDanhMuc : Form
    {
        private DataGridView dgvDanhMuc = null!;
        private TextBox txtTenDanhMuc = null!;
        private TextBox txtMoTa = null!;
        private Button btnThem = null!;
        private Button btnSua = null!;
        private Button btnXoa = null!;
        private Button btnLamMoi = null!;

        private int _selectedId = 0;

        public frmDanhMuc()
        {
            InitUI();
            LoadData();
        }

        private void InitUI()
        {
            this.Text = "Quản lý danh mục sản phẩm";
            this.BackColor = UIStyleHelper.BgMain;
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F);

            dgvDanhMuc = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvDanhMuc);
            dgvDanhMuc.SelectionChanged += DgvDanhMuc_SelectionChanged;

            var pnlInput = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 150,
                Padding = new Padding(15),
                BackColor = UIStyleHelper.BgCard
            };

            pnlInput.Controls.Add(new Label { Text = "Tên danh mục:", Location = new Point(15, 20), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            txtTenDanhMuc = new TextBox { Location = new Point(130, 17), Width = 300 };
            UIStyleHelper.StyleTextBox(txtTenDanhMuc);
            pnlInput.Controls.Add(txtTenDanhMuc);

            pnlInput.Controls.Add(new Label { Text = "Mô tả:", Location = new Point(15, 65), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            txtMoTa = new TextBox { Location = new Point(130, 62), Width = 300, Multiline = true, Height = 65 };
            UIStyleHelper.StyleTextBox(txtMoTa);
            pnlInput.Controls.Add(txtMoTa);

            btnThem = CreateBtn("Thêm mới", UIStyleHelper.PrimaryBlue, 460, 17);
            btnThem.Click += BtnThem_Click;

            btnSua = CreateBtn("Cập nhật", UIStyleHelper.SuccessGreen, 460, 62);
            btnSua.Click += BtnSua_Click;

            btnXoa = CreateBtn("Xóa", UIStyleHelper.DangerRed, 595, 17);
            btnXoa.Click += BtnXoa_Click;

            btnLamMoi = CreateBtn("Làm mới", Color.FromArgb(100, 116, 139), 595, 62);
            btnLamMoi.Click += (s, e) => ClearForm();

            pnlInput.Controls.Add(btnThem);
            pnlInput.Controls.Add(btnSua);
            pnlInput.Controls.Add(btnXoa);
            pnlInput.Controls.Add(btnLamMoi);

            this.Controls.Add(dgvDanhMuc);
            this.Controls.Add(pnlInput);
        }

        private Button CreateBtn(string text, Color bg, int x, int y)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(115, 34),
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void LoadData()
        {
            var list = new DanhMucSanPhamDAO().GetAll();
            dgvDanhMuc.DataSource = list;

            if (dgvDanhMuc.Columns["MaDanhMuc"] != null) dgvDanhMuc.Columns["MaDanhMuc"].HeaderText = "Mã DM";
            if (dgvDanhMuc.Columns["TenDanhMuc"] != null) dgvDanhMuc.Columns["TenDanhMuc"].HeaderText = "Tên Danh Mục";
            if (dgvDanhMuc.Columns["MoTa"] != null) dgvDanhMuc.Columns["MoTa"].HeaderText = "Mô Tả";
        }

        private void DgvDanhMuc_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvDanhMuc.CurrentRow == null) return;
            if (dgvDanhMuc.CurrentRow.DataBoundItem is DanhMucSanPham dm)
            {
                _selectedId = dm.MaDanhMuc;
                txtTenDanhMuc.Text = dm.TenDanhMuc;
                txtMoTa.Text = dm.MoTa ?? "";
            }
        }

        private void ClearForm()
        {
            _selectedId = 0;
            txtTenDanhMuc.Clear();
            txtMoTa.Clear();
        }

        private void BtnThem_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenDanhMuc.Text))
            {
                MessageBox.Show("Vui lòng nhập tên danh mục.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dm = new DanhMucSanPham { TenDanhMuc = txtTenDanhMuc.Text.Trim(), MoTa = txtMoTa.Text.Trim() };
            if (new DanhMucSanPhamDAO().Insert(dm))
            {
                MessageBox.Show("Thêm danh mục thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadData();
            }
        }

        private void BtnSua_Click(object? sender, EventArgs e)
        {
            if (_selectedId <= 0)
            {
                MessageBox.Show("Vui lòng chọn danh mục cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dm = new DanhMucSanPham { MaDanhMuc = _selectedId, TenDanhMuc = txtTenDanhMuc.Text.Trim(), MoTa = txtMoTa.Text.Trim() };
            if (new DanhMucSanPhamDAO().Update(dm))
            {
                MessageBox.Show("Cập nhật danh mục thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadData();
            }
        }

        private void BtnXoa_Click(object? sender, EventArgs e)
        {
            if (_selectedId <= 0)
            {
                MessageBox.Show("Vui lòng chọn danh mục cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var res = MessageBox.Show("Bạn có chắc muốn xóa danh mục này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes && new DanhMucSanPhamDAO().Delete(_selectedId))
            {
                MessageBox.Show("Đã xóa danh mục!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadData();
            }
        }
    }
}
