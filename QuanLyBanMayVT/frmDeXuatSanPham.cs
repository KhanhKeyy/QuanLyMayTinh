using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    public class frmDeXuatSanPham : Form
    {
        private TextBox txtTenSP = null!;
        private ComboBox cboDanhMuc = null!;
        private TextBox txtCauHinh = null!;
        private NumericUpDown numGiaBan = null!;
        private NumericUpDown numSoLuongTon = null!;
        private NumericUpDown numMucToiThieu = null!;
        private TextBox txtLyDo = null!;

        private Button btnGui = null!;
        private Button btnHuy = null!;

        public frmDeXuatSanPham()
        {
            InitUI();
            LoadDanhMuc();
        }

        private void InitUI()
        {
            this.Text = "💡 Đề xuất sản phẩm mới (Dành cho Nhân viên)";
            this.Size = new Size(520, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = UIStyleHelper.BgMain;
            this.Font = new Font("Segoe UI", 9.5F);

            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24),
                ColumnCount = 2,
                RowCount = 9,
                AutoSize = true
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            int r = 0;

            // Header Title
            var lblTitle = new Label
            {
                Text = "💡 ĐỀ XUẤT THÊM SẢN PHẨM MỚI",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 15)
            };
            tbl.Controls.Add(lblTitle, 0, r);
            tbl.SetColumnSpan(lblTitle, 2);

            // 1. Tên Sản Phẩm
            r++;
            tbl.Controls.Add(CreateLabel("Tên sản phẩm (*):"), 0, r);
            txtTenSP = new TextBox { Dock = DockStyle.Fill };
            UIStyleHelper.StyleTextBox(txtTenSP);
            tbl.Controls.Add(txtTenSP, 1, r);

            // 2. Danh Mục
            r++;
            tbl.Controls.Add(CreateLabel("Danh mục (*):"), 0, r);
            cboDanhMuc = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            UIStyleHelper.StyleComboBox(cboDanhMuc);
            tbl.Controls.Add(cboDanhMuc, 1, r);

            // 3. Cấu hình / Thông số
            r++;
            tbl.Controls.Add(CreateLabel("Cấu hình/Mô tả:"), 0, r);
            txtCauHinh = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 65 };
            UIStyleHelper.StyleTextBox(txtCauHinh);
            tbl.Controls.Add(txtCauHinh, 1, r);

            // 4. Giá bán đề xuất
            r++;
            tbl.Controls.Add(CreateLabel("Giá bán đề xuất (*):"), 0, r);
            numGiaBan = new NumericUpDown
            {
                Dock = DockStyle.Fill,
                Maximum = 1000000000,
                Increment = 100000,
                ThousandsSeparator = true
            };
            UIStyleHelper.StyleNumeric(numGiaBan);
            tbl.Controls.Add(numGiaBan, 1, r);

            // 5. Số lượng nhập dự kiến
            r++;
            tbl.Controls.Add(CreateLabel("Số lượng dự kiến:"), 0, r);
            numSoLuongTon = new NumericUpDown
            {
                Dock = DockStyle.Fill,
                Maximum = 10000,
                Value = 10
            };
            UIStyleHelper.StyleNumeric(numSoLuongTon);
            tbl.Controls.Add(numSoLuongTon, 1, r);

            // 6. Mức tồn tối thiểu
            r++;
            tbl.Controls.Add(CreateLabel("Mức tồn tối thiểu:"), 0, r);
            numMucToiThieu = new NumericUpDown
            {
                Dock = DockStyle.Fill,
                Maximum = 1000,
                Value = 2
            };
            UIStyleHelper.StyleNumeric(numMucToiThieu);
            tbl.Controls.Add(numMucToiThieu, 1, r);

            // 7. Lý do đề xuất
            r++;
            tbl.Controls.Add(CreateLabel("Lý do đề xuất:"), 0, r);
            txtLyDo = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 55 };
            UIStyleHelper.StyleTextBox(txtLyDo);
            tbl.Controls.Add(txtLyDo, 1, r);

            // 8. Buttons
            r++;
            var pnlBtns = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Margin = new Padding(0, 15, 0, 0)
            };

            btnHuy = new Button
            {
                Text = "Hủy",
                Size = new Size(90, 36),
                BackColor = Color.FromArgb(226, 232, 240),
                ForeColor = Color.FromArgb(51, 65, 85),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnHuy.FlatAppearance.BorderSize = 0;
            btnHuy.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            btnGui = new Button
            {
                Text = "🚀 Gửi đề xuất cho Quản lý",
                AutoSize = true,
                Padding = new Padding(12, 0, 12, 0),
                Height = 36,
                Margin = new Padding(0, 0, 10, 0),
                BackColor = UIStyleHelper.PrimaryBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGui.FlatAppearance.BorderSize = 0;
            btnGui.Click += BtnGui_Click;

            pnlBtns.Controls.Add(btnHuy);
            pnlBtns.Controls.Add(btnGui);

            tbl.Controls.Add(pnlBtns, 0, r);
            tbl.SetColumnSpan(pnlBtns, 2);

            this.Controls.Add(tbl);
        }

        private Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85)
            };
        }

        private void LoadDanhMuc()
        {
            cboDanhMuc.DataSource = new DanhMucSanPhamDAO().GetAll();
            cboDanhMuc.DisplayMember = "TenDanhMuc";
            cboDanhMuc.ValueMember = "MaDanhMuc";
        }

        private void BtnGui_Click(object? sender, EventArgs e)
        {
            string tenSP = txtTenSP.Text.Trim();
            if (string.IsNullOrEmpty(tenSP))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenSP.Focus();
                return;
            }
            if (cboDanhMuc.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn danh mục sản phẩm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (numGiaBan.Value <= 0)
            {
                MessageBox.Show("Giá bán đề xuất phải lớn hơn 0 đ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numGiaBan.Focus();
                return;
            }

            var currentNV = UserSession.CurrentNhanVien;
            int maNV = currentNV != null ? currentNV.MaNhanVien : 1;

            var yc = new YeuCauThemSanPham
            {
                MaNhanVienDeXuat = maNV,
                MaDanhMuc = (int)cboDanhMuc.SelectedValue,
                TenSanPham = tenSP,
                CauHinh = txtCauHinh.Text.Trim(),
                GiaBan = numGiaBan.Value,
                SoLuongTon = (int)numSoLuongTon.Value,
                MucTonToiThieu = (int)numMucToiThieu.Value,
                LyDoDeXuat = txtLyDo.Text.Trim()
            };

            int maYC = new YeuCauThemSanPhamDAO().InsertDeXuat(yc);
            if (maYC > 0)
            {
                MessageBox.Show($"🎉 Đã gửi đề xuất sản phẩm #{maYC} thành công!\nQuản lý sẽ xem xét và kiểm duyệt đề xuất của bạn.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
