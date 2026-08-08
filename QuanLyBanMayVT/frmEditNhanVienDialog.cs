using System.Text.RegularExpressions;
using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    /// <summary>
    /// Dialog Pop-up Thêm / Sửa nhân viên
    /// </summary>
    public class frmEditNhanVienDialog : Form
    {
        private readonly NhanVien? _nvTarget;

        private TextBox txtHoTen = null!;
        private TextBox txtEmail = null!;
        private TextBox txtSDT = null!;
        private ComboBox cboChucVu = null!;
        private DateTimePicker dtpNgayVaoLam = null!;
        private TextBox txtTenDangNhap = null!;
        private Label lblTenDangNhap = null!;

        private Button btnLuu = null!;
        private Button btnHuy = null!;

        public frmEditNhanVienDialog(NhanVien? nvTarget = null)
        {
            _nvTarget = nvTarget;
            InitUI();
            LoadDataTarget();
        }

        private void InitUI()
        {
            bool isEdit = _nvTarget != null;
            this.Text = isEdit ? "✏️ Chỉnh Sửa Nhân Viên" : "➕ Thêm Nhân Viên Mới";
            this.Size = new Size(540, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = UIStyleHelper.BgMain;
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10F);

            // ── Header Panel ─────────────────────────────────────────────
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = UIStyleHelper.BgCard,
                Padding = new Padding(15, 0, 0, 0)
            };
            var lblTitle = new Label
            {
                Text = isEdit ? "✏️ CHỈNH SỬA THÔNG TIN NHÂN VIÊN" : "➕ THÊM NHÂN VIÊN MỚI",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(99, 179, 237),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlHeader.Controls.Add(lblTitle);

            // ── Body Panel ───────────────────────────────────────────────
            var pnlBody = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(25),
                BackColor = UIStyleHelper.BgMain
            };

            int lblX = 20, ctrlX = 150, ctrlW = 320;
            int yStep = 45;
            int curY = 15;

            // Họ tên
            pnlBody.Controls.Add(new Label { Text = "Họ và tên *:", Location = new Point(lblX, curY + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            txtHoTen = new TextBox { Location = new Point(ctrlX, curY), Width = ctrlW };
            UIStyleHelper.StyleTextBox(txtHoTen);
            pnlBody.Controls.Add(txtHoTen);
            curY += yStep;

            // Chức vụ
            pnlBody.Controls.Add(new Label { Text = "Chức vụ *:", Location = new Point(lblX, curY + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            cboChucVu = new ComboBox { Location = new Point(ctrlX, curY), Width = ctrlW, DropDownStyle = ComboBoxStyle.DropDownList };
            cboChucVu.Items.AddRange(new object[] { "NhanVienBanHang", "KeToan", "QuanLy" });
            cboChucVu.SelectedIndex = 0;
            UIStyleHelper.StyleComboBox(cboChucVu);
            pnlBody.Controls.Add(cboChucVu);
            curY += yStep;

            // Số ĐT
            pnlBody.Controls.Add(new Label { Text = "Số điện thoại:", Location = new Point(lblX, curY + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            txtSDT = new TextBox { Location = new Point(ctrlX, curY), Width = ctrlW };
            UIStyleHelper.StyleTextBox(txtSDT);
            pnlBody.Controls.Add(txtSDT);
            curY += yStep;

            // Email
            pnlBody.Controls.Add(new Label { Text = "Email:", Location = new Point(lblX, curY + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            txtEmail = new TextBox { Location = new Point(ctrlX, curY), Width = ctrlW };
            UIStyleHelper.StyleTextBox(txtEmail);
            pnlBody.Controls.Add(txtEmail);
            curY += yStep;

            // Ngày vào làm
            pnlBody.Controls.Add(new Label { Text = "Ngày vào làm:", Location = new Point(lblX, curY + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            dtpNgayVaoLam = new DateTimePicker { Location = new Point(ctrlX, curY), Width = ctrlW, Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 10F) };
            pnlBody.Controls.Add(dtpNgayVaoLam);
            curY += yStep;

            // Tên đăng nhập (chỉ hiện khi tạo mới nhân viên)
            if (!isEdit)
            {
                lblTenDangNhap = new Label { Text = "Tên đăng nhập *:", Location = new Point(lblX, curY + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted };
                txtTenDangNhap = new TextBox { Location = new Point(ctrlX, curY), Width = ctrlW };
                UIStyleHelper.StyleTextBox(txtTenDangNhap);
                pnlBody.Controls.Add(lblTenDangNhap);
                pnlBody.Controls.Add(txtTenDangNhap);
                curY += 55;
            }

            // Buttons
            btnLuu = new Button
            {
                Text = isEdit ? "💾 Cập nhật" : "➕ Thêm mới",
                Location = new Point(ctrlX, curY),
                Size = new Size(130, 36),
                BackColor = isEdit ? UIStyleHelper.SuccessGreen : UIStyleHelper.PrimaryBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLuu.FlatAppearance.BorderSize = 0;
            btnLuu.Click += BtnLuu_Click;

            btnHuy = new Button
            {
                Text = "Hủy bỏ",
                Location = new Point(ctrlX + 145, curY),
                Size = new Size(110, 36),
                BackColor = Color.FromArgb(100, 116, 139),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F),
                Cursor = Cursors.Hand
            };
            btnHuy.FlatAppearance.BorderSize = 0;
            btnHuy.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            pnlBody.Controls.Add(btnLuu);
            pnlBody.Controls.Add(btnHuy);

            this.Controls.Add(pnlBody);
            this.Controls.Add(pnlHeader);
        }

        private void LoadDataTarget()
        {
            if (_nvTarget == null) return;

            txtHoTen.Text = _nvTarget.HoTen;
            cboChucVu.SelectedItem = _nvTarget.ChucVu;
            txtSDT.Text = _nvTarget.SoDienThoai ?? "";
            txtEmail.Text = _nvTarget.Email ?? "";
            if (_nvTarget.NgayVaoLam.HasValue) dtpNgayVaoLam.Value = _nvTarget.NgayVaoLam.Value;
        }

        private void BtnLuu_Click(object? sender, EventArgs e)
        {
            // 1. Validate Họ và tên
            string hoTen = txtHoTen.Text.Trim();
            if (string.IsNullOrWhiteSpace(hoTen))
            {
                MessageBox.Show("Vui lòng nhập họ và tên nhân viên.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHoTen.Focus();
                return;
            }
            if (hoTen.Length < 2 || hoTen.Length > 100)
            {
                MessageBox.Show("Họ và tên phải có độ dài từ 2 đến 100 ký tự.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHoTen.Focus();
                txtHoTen.SelectAll();
                return;
            }
            if (Regex.IsMatch(hoTen, @"[0-9]"))
            {
                MessageBox.Show("Họ và tên không được chứa chữ số.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHoTen.Focus();
                txtHoTen.SelectAll();
                return;
            }

            // 2. Validate Tên đăng nhập (khi thêm mới)
            string tenDN = "";
            if (_nvTarget == null)
            {
                tenDN = txtTenDangNhap?.Text.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(tenDN))
                {
                    MessageBox.Show("Vui lòng nhập tên đăng nhập cho nhân viên.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTenDangNhap?.Focus();
                    return;
                }
                if (tenDN.Length < 4 || tenDN.Length > 30)
                {
                    MessageBox.Show("Tên đăng nhập phải từ 4 đến 30 ký tự.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTenDangNhap?.Focus();
                    txtTenDangNhap?.SelectAll();
                    return;
                }
                if (!Regex.IsMatch(tenDN, @"^[a-zA-Z0-9_]+$"))
                {
                    MessageBox.Show("Tên đăng nhập chỉ được chứa chữ cái không dấu, chữ số và dấu gạch dưới (_), không được chứa khoảng trắng.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTenDangNhap?.Focus();
                    txtTenDangNhap?.SelectAll();
                    return;
                }
                if (new TaiKhoanDAO().TonTaiTenDangNhap(tenDN))
                {
                    MessageBox.Show($"Tên đăng nhập '{tenDN}' đã được sử dụng.\nVui lòng chọn tên khác.", "Trùng tên đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTenDangNhap?.Focus();
                    txtTenDangNhap?.SelectAll();
                    return;
                }
            }

            // 3. Validate Số điện thoại (nếu có nhập)
            string sdt = txtSDT.Text.Trim();
            var allNV = new NhanVienDAO().GetAll();
            int currentId = _nvTarget?.MaNhanVien ?? 0;

            if (!string.IsNullOrEmpty(sdt))
            {
                if (!Regex.IsMatch(sdt, @"^0\d{9}$"))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ!\nVui lòng nhập đúng 10 chữ số và bắt đầu bằng số 0 (Ví dụ: 0987654321).", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSDT.Focus();
                    txtSDT.SelectAll();
                    return;
                }
                if (allNV.Any(n => n.MaNhanVien != currentId && n.SoDienThoai != null && n.SoDienThoai.Trim() == sdt))
                {
                    MessageBox.Show($"Số điện thoại '{sdt}' đã được đăng ký cho một nhân viên khác.", "Trùng số điện thoại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSDT.Focus();
                    txtSDT.SelectAll();
                    return;
                }
            }

            // 4. Validate Email (nếu có nhập)
            string email = txtEmail.Text.Trim();
            if (!string.IsNullOrEmpty(email))
            {
                if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageBox.Show("Email không hợp lệ!\nVui lòng nhập đúng định dạng email (Ví dụ: user@domain.com).", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    txtEmail.SelectAll();
                    return;
                }
                if (allNV.Any(n => n.MaNhanVien != currentId && n.Email != null && n.Email.Trim().Equals(email, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show($"Email '{email}' đã được đăng ký cho một nhân viên khác.", "Trùng Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    txtEmail.SelectAll();
                    return;
                }
            }

            // 5. Validate Ngày vào làm
            DateTime nvl = dtpNgayVaoLam.Value;
            if (nvl.Date > DateTime.Today.AddDays(30))
            {
                MessageBox.Show("Ngày vào làm không được vượt quá 30 ngày so với hiện tại.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpNgayVaoLam.Focus();
                return;
            }
            if (nvl.Year < 2000)
            {
                MessageBox.Show("Ngày vào làm không hợp lệ (phải từ năm 2000 trở lại đây).", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpNgayVaoLam.Focus();
                return;
            }

            // Khởi tạo model và Lưu
            var nv = new NhanVien
            {
                MaNhanVien = currentId,
                HoTen = hoTen,
                ChucVu = cboChucVu.SelectedItem?.ToString() ?? "NhanVienBanHang",
                SoDienThoai = sdt,
                Email = email,
                NgayVaoLam = nvl,
                MaTaiKhoan = _nvTarget?.MaTaiKhoan ?? 0
            };

            bool success;
            if (_nvTarget == null)
            {
                string passHash = PasswordHasher.Hash("123456");
                success = new NhanVienDAO().Insert(nv, tenDN, passHash);
            }
            else
            {
                success = new NhanVienDAO().Update(nv);
            }

            if (success)
            {
                MessageBox.Show(_nvTarget == null ? "Thêm nhân viên thành công! (Mật khẩu mặc định: 123456)" : "Cập nhật thông tin nhân viên thành công!",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}

