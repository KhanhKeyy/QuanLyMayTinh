using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    public class frmSanPham : Form
    {
        private readonly bool _cheBoDuyet;

        private DataGridView dgvSanPham = null!;
        private TextBox txtTimKiem = null!;
        private ComboBox cboFilterDanhMuc = null!;

        // Controls CRUD (chỉ hiện khi _cheBoDuyet = true)
        private Panel pnlInput = null!;
        private TextBox txtTenSP = null!;
        private ComboBox cboDanhMuc = null!;
        private TextBox txtCauHinh = null!;
        private NumericUpDown numGiaBan = null!;
        private NumericUpDown numSoLuongTon = null!;
        private NumericUpDown numMucToiThieu = null!;
        private ComboBox cboTrangThai = null!;

        private Button btnThem = null!;
        private Button btnSua = null!;
        private Button btnXoa = null!;
        private Button btnLamMoi = null!;

        private int _selectedId = 0;

        public frmSanPham(bool cheBoDuyet = false)
        {
            _cheBoDuyet = cheBoDuyet;
            InitUI();
            LoadDanhMuc();
            LoadData();
        }

        private void InitUI()
        {
            this.Text = _cheBoDuyet ? "Quản lý sản phẩm" : "Danh sách sản phẩm";
            this.BackColor = UIStyleHelper.BgMain;
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F);

            // ── TOP PANEL ──────────────────────────────────────────────
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12), BackColor = UIStyleHelper.BgCard };

            var lblTK = new Label { Text = "🔍 Tìm kiếm:", Location = new Point(12, 16), AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            txtTimKiem = new TextBox { Location = new Point(110, 12), Width = 220 };
            UIStyleHelper.StyleTextBox(txtTimKiem);
            txtTimKiem.TextChanged += (s, e) => LoadData();

            var lblDM = new Label { Text = "Danh mục:", Location = new Point(350, 16), AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            cboFilterDanhMuc = new ComboBox { Location = new Point(440, 12), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            UIStyleHelper.StyleComboBox(cboFilterDanhMuc);
            cboFilterDanhMuc.SelectedIndexChanged += (s, e) => LoadData();

            pnlTop.Controls.Add(lblTK);
            pnlTop.Controls.Add(txtTimKiem);
            pnlTop.Controls.Add(lblDM);
            pnlTop.Controls.Add(cboFilterDanhMuc);

            // ── DATAGRIDVIEW ───────────────────────────────────────────
            dgvSanPham = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvSanPham);
            dgvSanPham.SelectionChanged += DgvSanPham_SelectionChanged;

            // ── INPUT PANEL FOR CRUD ───────────────────────────────────
            if (_cheBoDuyet)
            {
                pnlInput = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 240,
                    Padding = new Padding(15),
                    BackColor = UIStyleHelper.BgCard
                };

                int labelX1 = 15, controlX1 = 110, controlWidth1 = 240;
                int labelX2 = 370, controlX2 = 485, controlWidth2 = 170;
                int btnX = 675;

                int y1 = 15, y2 = 58, y3 = 101, y4 = 180;

                // Cột 1
                pnlInput.Controls.Add(new Label { Text = "Tên SP:", Location = new Point(labelX1, y1 + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
                txtTenSP = new TextBox { Location = new Point(controlX1, y1), Width = controlWidth1 };
                UIStyleHelper.StyleTextBox(txtTenSP);
                pnlInput.Controls.Add(txtTenSP);

                pnlInput.Controls.Add(new Label { Text = "Danh mục:", Location = new Point(labelX1, y2 + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
                cboDanhMuc = new ComboBox { Location = new Point(controlX1, y2), Width = controlWidth1, DropDownStyle = ComboBoxStyle.DropDownList };
                UIStyleHelper.StyleComboBox(cboDanhMuc);
                pnlInput.Controls.Add(cboDanhMuc);

                pnlInput.Controls.Add(new Label { Text = "Cấu hình:", Location = new Point(labelX1, y3 + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
                txtCauHinh = new TextBox { Location = new Point(controlX1, y3), Width = controlWidth1, Multiline = true, Height = 110 };
                UIStyleHelper.StyleTextBox(txtCauHinh);
                pnlInput.Controls.Add(txtCauHinh);

                // Cột 2
                pnlInput.Controls.Add(new Label { Text = "Giá bán:", Location = new Point(labelX2, y1 + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
                numGiaBan = new NumericUpDown { Location = new Point(controlX2, y1), Width = controlWidth2, Maximum = 1000000000, Increment = 100000 };
                UIStyleHelper.StyleNumeric(numGiaBan);
                pnlInput.Controls.Add(numGiaBan);

                pnlInput.Controls.Add(new Label { Text = "Tồn kho:", Location = new Point(labelX2, y2 + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
                numSoLuongTon = new NumericUpDown { Location = new Point(controlX2, y2), Width = controlWidth2, Maximum = 10000 };
                UIStyleHelper.StyleNumeric(numSoLuongTon);
                pnlInput.Controls.Add(numSoLuongTon);

                pnlInput.Controls.Add(new Label { Text = "Mức tối thiểu:", Location = new Point(labelX2, y3 + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
                numMucToiThieu = new NumericUpDown { Location = new Point(controlX2, y3), Width = controlWidth2, Maximum = 10000 };
                UIStyleHelper.StyleNumeric(numMucToiThieu);
                pnlInput.Controls.Add(numMucToiThieu);

                pnlInput.Controls.Add(new Label { Text = "Trạng thái:", Location = new Point(labelX2, y4 + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
                cboTrangThai = new ComboBox { Location = new Point(controlX2, y4), Width = controlWidth2, DropDownStyle = ComboBoxStyle.DropDownList };
                cboTrangThai.Items.AddRange(new object[] { "Con hang", "Het hang", "Ngung KD" });
                cboTrangThai.SelectedIndex = 0;
                UIStyleHelper.StyleComboBox(cboTrangThai);
                pnlInput.Controls.Add(cboTrangThai);

                // Nút thao tác
                btnThem = CreateBtn("Thêm mới", UIStyleHelper.PrimaryBlue, btnX, y1);
                btnThem.Click += BtnThem_Click;

                btnSua = CreateBtn("Cập nhật", UIStyleHelper.SuccessGreen, btnX, y2);
                btnSua.Click += BtnSua_Click;

                btnXoa = CreateBtn("Xóa", UIStyleHelper.DangerRed, btnX, y3);
                btnXoa.Click += BtnXoa_Click;

                btnLamMoi = CreateBtn("Làm mới", Color.FromArgb(100, 116, 139), btnX, y4);
                btnLamMoi.Click += (s, e) => ClearForm();

                pnlInput.Controls.Add(btnThem);
                pnlInput.Controls.Add(btnSua);
                pnlInput.Controls.Add(btnXoa);
                pnlInput.Controls.Add(btnLamMoi);

                this.Controls.Add(dgvSanPham);
                this.Controls.Add(pnlInput);
            }
            else
            {
                this.Controls.Add(dgvSanPham);
            }

            this.Controls.Add(pnlTop);
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

        private void LoadDanhMuc()
        {
            var listDM = new DanhMucSanPhamDAO().GetAll();

            var listFilter = new List<DanhMucSanPham>(listDM);
            listFilter.Insert(0, new DanhMucSanPham { MaDanhMuc = 0, TenDanhMuc = "-- Tất cả danh mục --" });
            cboFilterDanhMuc.DataSource = listFilter;
            cboFilterDanhMuc.DisplayMember = "TenDanhMuc";
            cboFilterDanhMuc.ValueMember = "MaDanhMuc";

            if (_cheBoDuyet)
            {
                cboDanhMuc.DataSource = listDM;
                cboDanhMuc.DisplayMember = "TenDanhMuc";
                cboDanhMuc.ValueMember = "MaDanhMuc";
            }
        }

        private void LoadData()
        {
            string kw = txtTimKiem.Text.Trim();
            int maDM = cboFilterDanhMuc.SelectedValue is int id ? id : 0;
            var list = new SanPhamDAO().GetAll(kw, maDM);

            dgvSanPham.DataSource = list.Select(p => new
            {
                p.MaSanPham,
                p.TenSanPham,
                p.TenDanhMuc,
                p.CauHinh,
                GiaBan = p.GiaBanFormatted,
                p.SoLuongTon,
                p.MucTonToiThieu,
                TrangThai = p.TrangThaiDisplay
            }).ToList();

            if (dgvSanPham.Columns["MaSanPham"] != null) dgvSanPham.Columns["MaSanPham"].HeaderText = "Mã SP";
            if (dgvSanPham.Columns["TenSanPham"] != null) dgvSanPham.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";
            if (dgvSanPham.Columns["TenDanhMuc"] != null) dgvSanPham.Columns["TenDanhMuc"].HeaderText = "Danh Mục";
            if (dgvSanPham.Columns["CauHinh"] != null) dgvSanPham.Columns["CauHinh"].HeaderText = "Cấu Hình";
            if (dgvSanPham.Columns["GiaBan"] != null) dgvSanPham.Columns["GiaBan"].HeaderText = "Giá Bán";
            if (dgvSanPham.Columns["SoLuongTon"] != null) dgvSanPham.Columns["SoLuongTon"].HeaderText = "Tồn Kho";
            if (dgvSanPham.Columns["MucTonToiThieu"] != null) dgvSanPham.Columns["MucTonToiThieu"].HeaderText = "Mức Tối Thiểu";
            if (dgvSanPham.Columns["TrangThai"] != null) dgvSanPham.Columns["TrangThai"].HeaderText = "Trạng Thái";
        }

        private void DgvSanPham_SelectionChanged(object? sender, EventArgs e)
        {
            if (!_cheBoDuyet || dgvSanPham.CurrentRow == null) return;
            _selectedId = (int)dgvSanPham.CurrentRow.Cells["MaSanPham"].Value;
            var sp = new SanPhamDAO().GetById(_selectedId);
            if (sp == null) return;

            txtTenSP.Text = sp.TenSanPham;
            cboDanhMuc.SelectedValue = sp.MaDanhMuc;
            txtCauHinh.Text = sp.CauHinh ?? "";
            numGiaBan.Value = sp.GiaBan;
            numSoLuongTon.Value = sp.SoLuongTon;
            numMucToiThieu.Value = sp.MucTonToiThieu;
            cboTrangThai.SelectedItem = sp.TrangThai;
        }

        private void ClearForm()
        {
            _selectedId = 0;
            txtTenSP.Clear();
            txtCauHinh.Clear();
            numGiaBan.Value = 0;
            numSoLuongTon.Value = 0;
            numMucToiThieu.Value = 0;
            cboTrangThai.SelectedIndex = 0;
        }

        private void BtnThem_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenSP.Text))
            {
                MessageBox.Show("Tên sản phẩm không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var sp = new SanPham
            {
                TenSanPham = txtTenSP.Text.Trim(),
                MaDanhMuc = (int)(cboDanhMuc.SelectedValue ?? 0),
                CauHinh = txtCauHinh.Text.Trim(),
                GiaBan = numGiaBan.Value,
                SoLuongTon = (int)numSoLuongTon.Value,
                MucTonToiThieu = (int)numMucToiThieu.Value,
                TrangThai = cboTrangThai.SelectedItem?.ToString() ?? "Con hang"
            };

            int newId = new SanPhamDAO().Insert(sp);
            if (newId > 0)
            {
                MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadData();
            }
        }

        private void BtnSua_Click(object? sender, EventArgs e)
        {
            if (_selectedId <= 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var sp = new SanPham
            {
                MaSanPham = _selectedId,
                TenSanPham = txtTenSP.Text.Trim(),
                MaDanhMuc = (int)(cboDanhMuc.SelectedValue ?? 0),
                CauHinh = txtCauHinh.Text.Trim(),
                GiaBan = numGiaBan.Value,
                SoLuongTon = (int)numSoLuongTon.Value,
                MucTonToiThieu = (int)numMucToiThieu.Value,
                TrangThai = cboTrangThai.SelectedItem?.ToString() ?? "Con hang"
            };

            if (new SanPhamDAO().Update(sp))
            {
                MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
        }

        private void BtnXoa_Click(object? sender, EventArgs e)
        {
            if (_selectedId <= 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var res = MessageBox.Show("Bạn có chắc muốn xóa sản phẩm này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes && new SanPhamDAO().Delete(_selectedId))
            {
                MessageBox.Show("Đã xóa sản phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadData();
            }
        }
    }
}
