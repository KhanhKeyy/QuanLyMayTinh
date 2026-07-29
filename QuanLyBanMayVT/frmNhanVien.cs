using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    /// <summary>
    /// Form Quản lý Nhân viên với Pop-up Dialog Thêm/Sửa
    /// </summary>
    public class frmNhanVien : Form
    {
        private DataGridView dgvNhanVien = null!;

        private Button btnThem = null!;
        private Button btnSua = null!;
        private Button btnXoa = null!;

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
                Text = "➕ Thêm nhân viên",
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
            dgvNhanVien.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) MoPopupSuaSelected();
            };

            this.Controls.Add(dgvNhanVien);
            this.Controls.Add(pnlTop);
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

        private void MoPopupThemMoi()
        {
            using var dialog = new frmEditNhanVienDialog(null);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void MoPopupSuaSelected()
        {
            if (dgvNhanVien.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = (int)dgvNhanVien.CurrentRow.Cells["MaNhanVien"].Value;
            var list = new NhanVienDAO().GetAll();
            var nv = list.FirstOrDefault(x => x.MaNhanVien == id);
            if (nv == null) return;

            using var dialog = new frmEditNhanVienDialog(nv);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void XoaSelected()
        {
            if (dgvNhanVien.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = (int)dgvNhanVien.CurrentRow.Cells["MaNhanVien"].Value;
            string name = dgvNhanVien.CurrentRow.Cells["HoTen"].Value?.ToString() ?? "";

            var res = MessageBox.Show($"Bạn có chắc muốn xóa nhân viên '{name}' (Mã #{id})?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                if (new NhanVienDAO().Delete(id))
                {
                    MessageBox.Show("Đã xóa nhân viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
            }
        }
    }
}
