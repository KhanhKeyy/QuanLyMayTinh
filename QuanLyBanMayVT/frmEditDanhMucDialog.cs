using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    /// <summary>
    /// Dialog Pop-up Thêm / Sửa Danh mục sản phẩm
    /// </summary>
    public class frmEditDanhMucDialog : Form
    {
        private readonly DanhMucSanPham? _dmTarget;

        private TextBox txtTenDanhMuc = null!;
        private TextBox txtMoTa = null!;

        private Button btnLuu = null!;
        private Button btnHuy = null!;

        public frmEditDanhMucDialog(DanhMucSanPham? dmTarget = null)
        {
            _dmTarget = dmTarget;
            InitUI();
            LoadDataTarget();
        }

        private void InitUI()
        {
            bool isEdit = _dmTarget != null;
            this.Text = isEdit ? "✏️ Chỉnh Sửa Danh Mục" : "➕ Thêm Danh Mục Mới";
            this.Size = new Size(500, 360);
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
                Text = isEdit ? "✏️ CHỈNH SỬA DANH MỤC SẢN PHẨM" : "➕ THÊM DANH MỤC SẢN PHẨM MỚI",
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

            int lblX = 20, ctrlX = 140, ctrlW = 300;
            int curY = 20;

            // Tên danh mục
            pnlBody.Controls.Add(new Label { Text = "Tên danh mục *:", Location = new Point(lblX, curY + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            txtTenDanhMuc = new TextBox { Location = new Point(ctrlX, curY), Width = ctrlW };
            UIStyleHelper.StyleTextBox(txtTenDanhMuc);
            pnlBody.Controls.Add(txtTenDanhMuc);
            curY += 45;

            // Mô tả
            pnlBody.Controls.Add(new Label { Text = "Mô tả:", Location = new Point(lblX, curY + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            txtMoTa = new TextBox { Location = new Point(ctrlX, curY), Width = ctrlW, Height = 90, Multiline = true };
            UIStyleHelper.StyleTextBox(txtMoTa);
            pnlBody.Controls.Add(txtMoTa);
            curY += 110;

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
            if (_dmTarget == null) return;
            txtTenDanhMuc.Text = _dmTarget.TenDanhMuc;
            txtMoTa.Text = _dmTarget.MoTa ?? "";
        }

        private void BtnLuu_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenDanhMuc.Text))
            {
                MessageBox.Show("Vui lòng nhập tên danh mục.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dm = new DanhMucSanPham
            {
                MaDanhMuc = _dmTarget?.MaDanhMuc ?? 0,
                TenDanhMuc = txtTenDanhMuc.Text.Trim(),
                MoTa = txtMoTa.Text.Trim()
            };

            bool success;
            if (_dmTarget == null)
            {
                success = new DanhMucSanPhamDAO().Insert(dm);
            }
            else
            {
                success = new DanhMucSanPhamDAO().Update(dm);
            }

            if (success)
            {
                MessageBox.Show(_dmTarget == null ? "Thêm danh mục thành công!" : "Cập nhật danh mục thành công!",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
