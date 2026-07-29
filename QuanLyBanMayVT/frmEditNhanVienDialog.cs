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
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên nhân viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nv = new NhanVien
            {
                MaNhanVien = _nvTarget?.MaNhanVien ?? 0,
                HoTen = txtHoTen.Text.Trim(),
                ChucVu = cboChucVu.SelectedItem?.ToString() ?? "NhanVienBanHang",
                SoDienThoai = txtSDT.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                NgayVaoLam = dtpNgayVaoLam.Value,
                MaTaiKhoan = _nvTarget?.MaTaiKhoan ?? 0
            };

            bool success;
            if (_nvTarget == null)
            {
                if (txtTenDangNhap != null && string.IsNullOrWhiteSpace(txtTenDangNhap.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên đăng nhập cho nhân viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string tenDN = txtTenDangNhap?.Text.Trim() ?? "user" + DateTime.Now.Ticks % 1000;
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
