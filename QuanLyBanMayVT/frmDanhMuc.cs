using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    /// <summary>
    /// Form Quản lý Danh mục sản phẩm với Pop-up Dialog Thêm/Sửa
    /// </summary>
    public class frmDanhMuc : Form
    {
        private DataGridView dgvDanhMuc = null!;

        private Button btnThem = null!;
        private Button btnSua = null!;
        private Button btnXoa = null!;

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

            // ── TOP PANEL ──────────────────────────────────────────────
            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = UIStyleHelper.BgCard
            };

            var flowTop = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(15, 12, 15, 10),
                BackColor = UIStyleHelper.BgCard
            };

            btnThem = new Button
            {
                Text = "➕ Thêm danh mục",
                Size = new Size(165, 36),
                Margin = new Padding(0, 0, 10, 0),
                BackColor = UIStyleHelper.SuccessGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnThem.FlatAppearance.BorderSize = 0;
            btnThem.Click += (s, e) => MoPopupThemMoi();

            btnSua = new Button
            {
                Text = "✏️ Sửa",
                Size = new Size(90, 36),
                Margin = new Padding(0, 0, 10, 0),
                BackColor = UIStyleHelper.PrimaryBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSua.FlatAppearance.BorderSize = 0;
            btnSua.Click += (s, e) => MoPopupSuaSelected();

            btnXoa = new Button
            {
                Text = "🗑️ Xóa",
                Size = new Size(90, 36),
                Margin = new Padding(0, 0, 0, 0),
                BackColor = UIStyleHelper.DangerRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnXoa.FlatAppearance.BorderSize = 0;
            btnXoa.Click += (s, e) => XoaSelected();

            flowTop.Controls.Add(btnThem);
            flowTop.Controls.Add(btnSua);
            flowTop.Controls.Add(btnXoa);

            pnlTop.Controls.Add(flowTop);

            // ── DATAGRIDVIEW ───────────────────────────────────────────
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
            dgvDanhMuc.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) MoPopupSuaSelected();
            };

            this.Controls.Add(dgvDanhMuc);
            this.Controls.Add(pnlTop);
        }

        private void LoadData()
        {
            var list = new DanhMucSanPhamDAO().GetAll();
            dgvDanhMuc.DataSource = list;

            if (dgvDanhMuc.Columns["MaDanhMuc"] != null) dgvDanhMuc.Columns["MaDanhMuc"].HeaderText = "Mã DM";
            if (dgvDanhMuc.Columns["TenDanhMuc"] != null) dgvDanhMuc.Columns["TenDanhMuc"].HeaderText = "Tên Danh Mục";
            if (dgvDanhMuc.Columns["MoTa"] != null) dgvDanhMuc.Columns["MoTa"].HeaderText = "Mô Tả";
        }

        private void MoPopupThemMoi()
        {
            using var dialog = new frmEditDanhMucDialog(null);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void MoPopupSuaSelected()
        {
            if (dgvDanhMuc.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn danh mục cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvDanhMuc.CurrentRow.DataBoundItem is DanhMucSanPham dm)
            {
                using var dialog = new frmEditDanhMucDialog(dm);
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void XoaSelected()
        {
            if (dgvDanhMuc.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn danh mục cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvDanhMuc.CurrentRow.DataBoundItem is DanhMucSanPham dm)
            {
                var res = MessageBox.Show($"Bạn có chắc muốn xóa danh mục '{dm.TenDanhMuc}' (Mã #{dm.MaDanhMuc})?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    if (new DanhMucSanPhamDAO().Delete(dm.MaDanhMuc))
                    {
                        MessageBox.Show("Đã xóa danh mục thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                }
            }
        }
    }
}
