using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    /// <summary>
    /// Dialog Cửa sổ Pop-up Thêm / Sửa sản phẩm
    /// </summary>
    public class frmEditSanPham : Form
    {
        private readonly SanPham? _spTarget;

        private TextBox txtTenSP = null!;
        private ComboBox cboDanhMuc = null!;
        private TextBox txtCauHinh = null!;
        private NumericUpDown numGiaBan = null!;
        private NumericUpDown numSoLuongTon = null!;
        private NumericUpDown numMucToiThieu = null!;
        private ComboBox cboTrangThai = null!;

        private Button btnLuu = null!;
        private Button btnHuy = null!;

        private readonly bool _isReadOnly;

        public frmEditSanPham(SanPham? spTarget = null, bool isReadOnly = false)
        {
            _spTarget = spTarget;
            _isReadOnly = isReadOnly;
            InitUI();
            LoadDanhMuc();
            LoadDataTarget();

            if (_isReadOnly)
            {
                ApplyReadOnlyMode();
            }
        }

        private void ApplyReadOnlyMode()
        {
            txtTenSP.ReadOnly = true;
            cboDanhMuc.Enabled = false;
            numGiaBan.Enabled = false;
            numSoLuongTon.Enabled = false;
            numMucToiThieu.Enabled = false;
            cboTrangThai.Enabled = false;
            txtCauHinh.ReadOnly = true;

            txtTenSP.BackColor = Color.White;
            txtCauHinh.BackColor = Color.White;

            btnLuu.Visible = false;
            btnHuy.Text = "Đóng";
            btnHuy.Location = new Point(140, btnHuy.Location.Y);
            btnHuy.Width = 140;
            btnHuy.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnHuy.BackColor = UIStyleHelper.PrimaryBlue;
        }

        private void InitUI()
        {
            bool isEdit = _spTarget != null;
            this.Text = _isReadOnly ? "🔍 Chi Tiết Sản Phẩm" : (isEdit ? "✏️ Chỉnh Sửa Sản Phẩm" : "➕ Thêm Sản Phẩm Mới");
            this.Size = new Size(580, 560);
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
                Text = _isReadOnly ? "🔍 CHI TIẾT THÔNG TIN SẢN PHẨM" : (isEdit ? "✏️ CHỈNH SỬA THÔNG TIN SẢN PHẨM" : "➕ THÊM SẢN PHẨM MỚI VÀO HỆ THỐNG"),
                Font = new Font("Segoe UI", 12.5F, FontStyle.Bold),
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

            int lblX1 = 20, ctrlX1 = 140, ctrlW1 = 380;
            int yStep = 45;
            int curY = 15;

            // Tên SP
            pnlBody.Controls.Add(new Label { Text = "Tên sản phẩm *:", Location = new Point(lblX1, curY + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            txtTenSP = new TextBox { Location = new Point(ctrlX1, curY), Width = ctrlW1 };
            UIStyleHelper.StyleTextBox(txtTenSP);
            pnlBody.Controls.Add(txtTenSP);
            curY += yStep;

            // Danh mục
            pnlBody.Controls.Add(new Label { Text = "Danh mục *:", Location = new Point(lblX1, curY + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            cboDanhMuc = new ComboBox { Location = new Point(ctrlX1, curY), Width = ctrlW1, DropDownStyle = ComboBoxStyle.DropDownList };
            UIStyleHelper.StyleComboBox(cboDanhMuc);
            pnlBody.Controls.Add(cboDanhMuc);
            curY += yStep;

            // Giá bán
            pnlBody.Controls.Add(new Label { Text = "Giá bán (VNĐ) *:", Location = new Point(lblX1, curY + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            numGiaBan = new NumericUpDown { Location = new Point(ctrlX1, curY), Width = 220, Maximum = 1_000_000_000, Increment = 100_000 };
            UIStyleHelper.StyleNumeric(numGiaBan);
            pnlBody.Controls.Add(numGiaBan);
            curY += yStep;

            // Tồn kho & Mức tối thiểu
            pnlBody.Controls.Add(new Label { Text = "Số lượng tồn:", Location = new Point(lblX1, curY + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            numSoLuongTon = new NumericUpDown { Location = new Point(ctrlX1, curY), Width = 110, Maximum = 10000 };
            UIStyleHelper.StyleNumeric(numSoLuongTon);
            pnlBody.Controls.Add(numSoLuongTon);

            pnlBody.Controls.Add(new Label { Text = "Mức tối thiểu:", Location = new Point(ctrlX1 + 130, curY + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            numMucToiThieu = new NumericUpDown { Location = new Point(ctrlX1 + 240, curY), Width = 140, Maximum = 10000, Value = 3 };
            UIStyleHelper.StyleNumeric(numMucToiThieu);
            pnlBody.Controls.Add(numMucToiThieu);
            curY += yStep;

            // Trạng thái
            pnlBody.Controls.Add(new Label { Text = "Trạng thái:", Location = new Point(lblX1, curY + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            cboTrangThai = new ComboBox { Location = new Point(ctrlX1, curY), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            cboTrangThai.Items.AddRange(new object[] { "Con hang", "Het hang", "Ngung KD" });
            cboTrangThai.SelectedIndex = 0;
            UIStyleHelper.StyleComboBox(cboTrangThai);
            pnlBody.Controls.Add(cboTrangThai);
            curY += yStep;

            // Cấu hình
            pnlBody.Controls.Add(new Label { Text = "Mô tả / Cấu hình:", Location = new Point(lblX1, curY + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            txtCauHinh = new TextBox { Location = new Point(ctrlX1, curY), Width = ctrlW1, Height = 80, Multiline = true };
            UIStyleHelper.StyleTextBox(txtCauHinh);
            pnlBody.Controls.Add(txtCauHinh);
            curY += 95;

            // Buttons
            btnLuu = new Button
            {
                Text = isEdit ? "💾 Cập nhật" : "➕ Thêm mới",
                Location = new Point(ctrlX1, curY),
                Size = new Size(140, 38),
                BackColor = isEdit ? UIStyleHelper.SuccessGreen : UIStyleHelper.PrimaryBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLuu.FlatAppearance.BorderSize = 0;
            btnLuu.Click += BtnLuu_Click;

            btnHuy = new Button
            {
                Text = "Hủy bỏ",
                Location = new Point(ctrlX1 + 155, curY),
                Size = new Size(110, 38),
                BackColor = Color.FromArgb(100, 116, 139),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand
            };
            btnHuy.FlatAppearance.BorderSize = 0;
            btnHuy.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            pnlBody.Controls.Add(btnLuu);
            pnlBody.Controls.Add(btnHuy);

            this.Controls.Add(pnlBody);
            this.Controls.Add(pnlHeader);
        }

        private void LoadDanhMuc()
        {
            var listDM = new DanhMucSanPhamDAO().GetAll();
            cboDanhMuc.DataSource = listDM;
            cboDanhMuc.DisplayMember = "TenDanhMuc";
            cboDanhMuc.ValueMember = "MaDanhMuc";
        }

        private void LoadDataTarget()
        {
            if (_spTarget == null) return;

            txtTenSP.Text = _spTarget.TenSanPham;
            cboDanhMuc.SelectedValue = _spTarget.MaDanhMuc;
            txtCauHinh.Text = _spTarget.CauHinh ?? "";
            numGiaBan.Value = _spTarget.GiaBan;
            numSoLuongTon.Value = _spTarget.SoLuongTon;
            numMucToiThieu.Value = _spTarget.MucTonToiThieu;
            cboTrangThai.SelectedItem = _spTarget.TrangThai;
        }

        private void BtnLuu_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenSP.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var sp = new SanPham
            {
                MaSanPham = _spTarget?.MaSanPham ?? 0,
                TenSanPham = txtTenSP.Text.Trim(),
                MaDanhMuc = (int)(cboDanhMuc.SelectedValue ?? 0),
                CauHinh = txtCauHinh.Text.Trim(),
                GiaBan = numGiaBan.Value,
                SoLuongTon = (int)numSoLuongTon.Value,
                MucTonToiThieu = (int)numMucToiThieu.Value,
                TrangThai = cboTrangThai.SelectedItem?.ToString() ?? "Con hang"
            };

            bool success;
            if (_spTarget == null)
            {
                int newId = new SanPhamDAO().Insert(sp);
                success = newId > 0;
            }
            else
            {
                success = new SanPhamDAO().Update(sp);
            }

            if (success)
            {
                MessageBox.Show(_spTarget == null ? "Thêm sản phẩm thành công!" : "Cập nhật sản phẩm thành công!",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
