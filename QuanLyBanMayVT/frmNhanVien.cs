using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    public class frmNhanVien : Form
    {
        private DataGridView dgvNhanVien = null!;

        private TextBox txtHoTen = null!;
        private TextBox txtEmail = null!;
        private TextBox txtSDT = null!;
        private ComboBox cboChucVu = null!;
        private DateTimePicker dtpNgayVaoLam = null!;
        private TextBox txtTenDangNhap = null!;

        private Button btnThem = null!;
        private Button btnSua = null!;
        private Button btnXoa = null!;
        private Button btnLamMoi = null!;

        private int _selectedId = 0;

        public frmNhanVien()
        {
            InitUI();
            LoadData();
        }

        private void InitUI()
        {
            this.Text = "Quản lý nhân viên";
            this.BackColor = UIStyleHelper.BgMain;
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F);

            dgvNhanVien = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvNhanVien);
            dgvNhanVien.SelectionChanged += DgvNhanVien_SelectionChanged;

            var pnlInput = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 190,
                Padding = new Padding(15),
                BackColor = UIStyleHelper.BgCard
            };

            int labelX1 = 15, controlX1 = 120, controlWidth1 = 230;
            int labelX2 = 380, controlX2 = 495, controlWidth2 = 180;
            int btnX = 695;

            int y1 = 15, y2 = 58, y3 = 101, y4 = 144;

            // Cột 1
            pnlInput.Controls.Add(new Label { Text = "Họ tên:", Location = new Point(labelX1, y1 + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            txtHoTen = new TextBox { Location = new Point(controlX1, y1), Width = controlWidth1 };
            UIStyleHelper.StyleTextBox(txtHoTen);
            pnlInput.Controls.Add(txtHoTen);

            pnlInput.Controls.Add(new Label { Text = "Email:", Location = new Point(labelX1, y2 + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            txtEmail = new TextBox { Location = new Point(controlX1, y2), Width = controlWidth1 };
            UIStyleHelper.StyleTextBox(txtEmail);
            pnlInput.Controls.Add(txtEmail);

            pnlInput.Controls.Add(new Label { Text = "Số ĐT:", Location = new Point(labelX1, y3 + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            txtSDT = new TextBox { Location = new Point(controlX1, y3), Width = controlWidth1 };
            UIStyleHelper.StyleTextBox(txtSDT);
            pnlInput.Controls.Add(txtSDT);

            // Cột 2
            pnlInput.Controls.Add(new Label { Text = "Chức vụ:", Location = new Point(labelX2, y1 + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            cboChucVu = new ComboBox { Location = new Point(controlX2, y1), Width = controlWidth2, DropDownStyle = ComboBoxStyle.DropDownList };
            cboChucVu.Items.AddRange(new object[] { "NhanVienBanHang", "KeToan", "QuanLy" });
            cboChucVu.SelectedIndex = 0;
            UIStyleHelper.StyleComboBox(cboChucVu);
            pnlInput.Controls.Add(cboChucVu);

            pnlInput.Controls.Add(new Label { Text = "Ngày vào làm:", Location = new Point(labelX2, y2 + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            dtpNgayVaoLam = new DateTimePicker { Location = new Point(controlX2, y2), Width = controlWidth2, Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 10F) };
            pnlInput.Controls.Add(dtpNgayVaoLam);

            pnlInput.Controls.Add(new Label { Text = "Tên đăng nhập:", Location = new Point(labelX2, y3 + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            txtTenDangNhap = new TextBox { Location = new Point(controlX2, y3), Width = controlWidth2 };
            UIStyleHelper.StyleTextBox(txtTenDangNhap);
            pnlInput.Controls.Add(txtTenDangNhap);

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

            this.Controls.Add(dgvNhanVien);
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
            var list = new NhanVienDAO().GetAll();
            dgvNhanVien.DataSource = list.Select(n => new
            {
                n.MaNhanVien,
                n.HoTen,
                ChucVu = n.ChucVuDisplay,
                n.SoDienThoai,
                n.Email,
                NgayVaoLam = n.NgayVaoLam?.ToString("dd/MM/yyyy") ?? "",
                n.MaTaiKhoan
            }).ToList();

            if (dgvNhanVien.Columns["MaNhanVien"] != null) dgvNhanVien.Columns["MaNhanVien"].HeaderText = "Mã NV";
            if (dgvNhanVien.Columns["HoTen"] != null) dgvNhanVien.Columns["HoTen"].HeaderText = "Họ Và Tên";
            if (dgvNhanVien.Columns["ChucVu"] != null) dgvNhanVien.Columns["ChucVu"].HeaderText = "Chức Vụ";
            if (dgvNhanVien.Columns["SoDienThoai"] != null) dgvNhanVien.Columns["SoDienThoai"].HeaderText = "Số Điện Thoại";
            if (dgvNhanVien.Columns["Email"] != null) dgvNhanVien.Columns["Email"].HeaderText = "Email";
            if (dgvNhanVien.Columns["NgayVaoLam"] != null) dgvNhanVien.Columns["NgayVaoLam"].HeaderText = "Ngày Vào Làm";
            if (dgvNhanVien.Columns["MaTaiKhoan"] != null) dgvNhanVien.Columns["MaTaiKhoan"].HeaderText = "Mã TK";
        }

        private void DgvNhanVien_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvNhanVien.CurrentRow == null) return;
            _selectedId = (int)dgvNhanVien.CurrentRow.Cells["MaNhanVien"].Value;
            var list = new NhanVienDAO().GetAll();
            var nv = list.FirstOrDefault(x => x.MaNhanVien == _selectedId);
            if (nv == null) return;

            txtHoTen.Text = nv.HoTen;
            txtEmail.Text = nv.Email;
            txtSDT.Text = nv.SoDienThoai;
            cboChucVu.SelectedItem = nv.ChucVu;
            if (nv.NgayVaoLam.HasValue) dtpNgayVaoLam.Value = nv.NgayVaoLam.Value;
            txtTenDangNhap.Enabled = false;
        }

        private void ClearForm()
        {
            _selectedId = 0;
            txtHoTen.Clear();
            txtEmail.Clear();
            txtSDT.Clear();
            cboChucVu.SelectedIndex = 0;
            dtpNgayVaoLam.Value = DateTime.Now;
            txtTenDangNhap.Clear();
            txtTenDangNhap.Enabled = true;
        }

        private void BtnThem_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên nhân viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtTenDangNhap.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập cho tài khoản nhân viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string username = txtTenDangNhap.Text.Trim();
            if (new TaiKhoanDAO().TonTaiTenDangNhap(username))
            {
                MessageBox.Show("Tên đăng nhập này đã tồn tại trong hệ thống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nv = new NhanVien
            {
                HoTen = txtHoTen.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                SoDienThoai = txtSDT.Text.Trim(),
                ChucVu = cboChucVu.SelectedItem?.ToString() ?? "NhanVienBanHang",
                NgayVaoLam = dtpNgayVaoLam.Value
            };

            // Mật khẩu mặc định: 123456
            string pass = "123456";

            if (new NhanVienDAO().Insert(nv, username, pass))
            {
                MessageBox.Show($"Thêm nhân viên thành công!\nTài khoản: {username}\nMật khẩu mặc định: 123456", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadData();
            }
        }

        private void BtnSua_Click(object? sender, EventArgs e)
        {
            if (_selectedId <= 0)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var list = new NhanVienDAO().GetAll();
            var nvOld = list.FirstOrDefault(x => x.MaNhanVien == _selectedId);
            if (nvOld == null) return;

            var nv = new NhanVien
            {
                MaNhanVien = _selectedId,
                MaTaiKhoan = nvOld.MaTaiKhoan,
                HoTen = txtHoTen.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                SoDienThoai = txtSDT.Text.Trim(),
                ChucVu = cboChucVu.SelectedItem?.ToString() ?? "NhanVienBanHang",
                NgayVaoLam = dtpNgayVaoLam.Value
            };

            if (new NhanVienDAO().Update(nv))
            {
                MessageBox.Show("Cập nhật thông tin nhân viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
        }

        private void BtnXoa_Click(object? sender, EventArgs e)
        {
            if (_selectedId <= 0)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var res = MessageBox.Show("Xóa nhân viên sẽ xóa cả tài khoản liên quan. Bạn có chắc không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes && new NhanVienDAO().Delete(_selectedId))
            {
                MessageBox.Show("Đã xóa nhân viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadData();
            }
        }
    }
}
