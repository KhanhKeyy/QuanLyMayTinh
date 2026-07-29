using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    /// <summary>
    /// Form Danh sách sản phẩm với Phân trang (Pagination) & Pop-up Thêm/Sửa
    /// </summary>
    public class frmSanPham : Form
    {
        private DataGridView dgvSanPham = null!;
        private TextBox txtTimKiem = null!;
        private ComboBox cboFilterDanhMuc = null!;

        // Header controls
        private Button btnThem = null!;
        private Button btnSua = null!;
        private Button btnXoa = null!;

        // Pagination controls
        private Panel pnlPagination = null!;
        private Label lblPageInfo = null!;
        private Button btnPrev = null!;
        private Button btnNext = null!;
        private FlowLayoutPanel pnlPageNumbers = null!;

        private List<SanPham> _fullList = new();
        private int _currentPage = 1;
        private const int PageSize = 10;

        public frmSanPham(bool cheBoDuyet = false)
        {
            InitUI();
            LoadDanhMuc();
            LoadData();

            // Nếu gọi trực tiếp mode thêm sản phẩm (từ sidebar), lập tức mở popup thêm mới
            if (cheBoDuyet)
            {
                this.Load += (s, e) => MoPopupThemMoi();
            }
        }

        private void InitUI()
        {
            this.Text = "Danh sách sản phẩm";
            this.BackColor = UIStyleHelper.BgMain;
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F);

            // ── TOP PANEL (Search, Filter & Add Button) ────────────────
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

            var lblTK = new Label
            {
                Text = "🔍 Tìm kiếm:",
                AutoSize = true,
                ForeColor = Color.FromArgb(17, 24, 39),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Margin = new Padding(0, 6, 8, 0)
            };
            txtTimKiem = new TextBox { Width = 220, Margin = new Padding(0, 2, 25, 0) };
            UIStyleHelper.StyleTextBox(txtTimKiem);
            txtTimKiem.TextChanged += (s, e) => { _currentPage = 1; LoadData(); };

            var lblDM = new Label
            {
                Text = "Danh mục:",
                AutoSize = true,
                ForeColor = Color.FromArgb(17, 24, 39),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Margin = new Padding(0, 6, 8, 0)
            };
            cboFilterDanhMuc = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(0, 2, 25, 0) };
            UIStyleHelper.StyleComboBox(cboFilterDanhMuc);
            cboFilterDanhMuc.SelectedIndexChanged += (s, e) => { _currentPage = 1; LoadData(); };

            // Nút Thêm Mới Sản Phẩm (Nổi bật màu xanh lá)
            btnThem = new Button
            {
                Text = "➕ Thêm sản phẩm",
                Size = new Size(160, 34),
                Margin = new Padding(0, 0, 10, 0),
                BackColor = UIStyleHelper.SuccessGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnThem.FlatAppearance.BorderSize = 0;
            btnThem.Click += (s, e) => MoPopupThemMoi();

            // Nút Sửa
            btnSua = new Button
            {
                Text = "✏️ Sửa",
                Size = new Size(90, 34),
                Margin = new Padding(0, 0, 10, 0),
                BackColor = UIStyleHelper.PrimaryBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSua.FlatAppearance.BorderSize = 0;
            btnSua.Click += (s, e) => MoPopupSuaSelected();

            // Nút Xóa
            btnXoa = new Button
            {
                Text = "🗑️ Xóa",
                Size = new Size(90, 34),
                Margin = new Padding(0, 0, 0, 0),
                BackColor = UIStyleHelper.DangerRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnXoa.FlatAppearance.BorderSize = 0;
            btnXoa.Click += (s, e) => XoaSelected();

            flowTop.Controls.Add(lblTK);
            flowTop.Controls.Add(txtTimKiem);
            flowTop.Controls.Add(lblDM);
            flowTop.Controls.Add(cboFilterDanhMuc);
            flowTop.Controls.Add(btnThem);
            flowTop.Controls.Add(btnSua);
            flowTop.Controls.Add(btnXoa);

            pnlTop.Controls.Add(flowTop);

            // ── DATAGRIDVIEW ───────────────────────────────────────────
            dgvSanPham = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders
            };
            UIStyleHelper.StyleDataGridView(dgvSanPham);
            dgvSanPham.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvSanPham.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) MoPopupSuaSelected();
            };

            // ── PAGINATION BAR (Bottom) ───────────────────────────────
            pnlPagination = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 55,
                Padding = new Padding(15, 10, 15, 10),
                BackColor = UIStyleHelper.BgCard
            };

            lblPageInfo = new Label
            {
                Text = "Hiển thị 0 - 0 / 0 sản phẩm",
                AutoSize = true,
                Location = new Point(15, 18),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = UIStyleHelper.TextMuted
            };

            pnlPageNumbers = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            btnPrev = new Button
            {
                Text = "◀ Trước",
                Size = new Size(85, 32),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(30, 41, 59),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPrev.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnPrev.FlatAppearance.BorderSize = 1;
            btnPrev.Click += (s, e) =>
            {
                if (_currentPage > 1) { _currentPage--; RenderPage(); }
            };

            btnNext = new Button
            {
                Text = "Sau ▶",
                Size = new Size(85, 32),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(30, 41, 59),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnNext.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnNext.FlatAppearance.BorderSize = 1;
            btnNext.Click += (s, e) =>
            {
                int totalPages = (int)Math.Ceiling((double)_fullList.Count / PageSize);
                if (_currentPage < totalPages) { _currentPage++; RenderPage(); }
            };

            pnlPagination.Controls.Add(lblPageInfo);
            pnlPagination.Controls.Add(btnPrev);
            pnlPagination.Controls.Add(pnlPageNumbers);
            pnlPagination.Controls.Add(btnNext);

            pnlPagination.Resize += (s, e) => RepositionPaginationControls();

            this.Controls.Add(dgvSanPham);
            this.Controls.Add(pnlPagination);
            this.Controls.Add(pnlTop);
        }

        private void RepositionPaginationControls()
        {
            int rightX = pnlPagination.ClientSize.Width - 15;
            btnNext.Left = rightX - btnNext.Width;
            btnNext.Top = 11;

            pnlPageNumbers.Left = btnNext.Left - pnlPageNumbers.Width - 8;
            pnlPageNumbers.Top = 11;

            btnPrev.Left = pnlPageNumbers.Left - btnPrev.Width - 8;
            btnPrev.Top = 11;
        }

        private void LoadDanhMuc()
        {
            var listDM = new DanhMucSanPhamDAO().GetAll();
            var listFilter = new List<DanhMucSanPham>(listDM);
            listFilter.Insert(0, new DanhMucSanPham { MaDanhMuc = 0, TenDanhMuc = "-- Tất cả danh mục --" });
            cboFilterDanhMuc.DataSource = listFilter;
            cboFilterDanhMuc.DisplayMember = "TenDanhMuc";
            cboFilterDanhMuc.ValueMember = "MaDanhMuc";
        }

        private void LoadData()
        {
            string kw = txtTimKiem.Text.Trim();
            int maDM = cboFilterDanhMuc.SelectedValue is int id ? id : 0;
            _fullList = new SanPhamDAO().GetAll(kw, maDM);

            RenderPage();
        }

        private void RenderPage()
        {
            int totalCount = _fullList.Count;
            int totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling((double)totalCount / PageSize);

            if (_currentPage > totalPages) _currentPage = totalPages;
            if (_currentPage < 1) _currentPage = 1;

            var pageItems = _fullList
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            dgvSanPham.DataSource = pageItems.Select(p => new
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

            // Cập nhật thông tin trang
            int startIdx = totalCount == 0 ? 0 : (_currentPage - 1) * PageSize + 1;
            int endIdx = Math.Min(_currentPage * PageSize, totalCount);
            lblPageInfo.Text = $"Hiển thị {startIdx} - {endIdx} / Tổng {totalCount} sản phẩm (Trang {_currentPage}/{totalPages})";

            btnPrev.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < totalPages;

            btnPrev.BackColor = btnPrev.Enabled ? Color.White : Color.FromArgb(241, 245, 249);
            btnPrev.ForeColor = btnPrev.Enabled ? Color.FromArgb(30, 41, 59) : Color.FromArgb(148, 163, 184);
            btnPrev.FlatAppearance.BorderColor = btnPrev.Enabled ? Color.FromArgb(203, 213, 225) : Color.FromArgb(226, 232, 240);

            btnNext.BackColor = btnNext.Enabled ? Color.White : Color.FromArgb(241, 245, 249);
            btnNext.ForeColor = btnNext.Enabled ? Color.FromArgb(30, 41, 59) : Color.FromArgb(148, 163, 184);
            btnNext.FlatAppearance.BorderColor = btnNext.Enabled ? Color.FromArgb(203, 213, 225) : Color.FromArgb(226, 232, 240);

            // Render các nút số trang (1, 2, 3...)
            pnlPageNumbers.Controls.Clear();
            for (int i = 1; i <= totalPages; i++)
            {
                int pageNum = i;
                bool isActive = pageNum == _currentPage;
                var btnNum = new Button
                {
                    Text = pageNum.ToString(),
                    Size = new Size(36, 32),
                    Margin = new Padding(2, 0, 2, 0),
                    BackColor = isActive ? UIStyleHelper.PrimaryBlue : Color.White,
                    ForeColor = isActive ? Color.White : Color.FromArgb(51, 65, 85),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, isActive ? FontStyle.Bold : FontStyle.Regular),
                    Cursor = Cursors.Hand
                };
                btnNum.FlatAppearance.BorderColor = isActive ? UIStyleHelper.PrimaryBlue : Color.FromArgb(203, 213, 225);
                btnNum.FlatAppearance.BorderSize = 1;
                btnNum.Click += (s, e) =>
                {
                    _currentPage = pageNum;
                    RenderPage();
                };
                pnlPageNumbers.Controls.Add(btnNum);
            }

            RepositionPaginationControls();
        }

        private void MoPopupThemMoi()
        {
            using var dialog = new frmEditSanPham(null);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void MoPopupSuaSelected()
        {
            if (dgvSanPham.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = (int)dgvSanPham.CurrentRow.Cells["MaSanPham"].Value;
            var sp = new SanPhamDAO().GetById(id);
            if (sp == null) return;

            using var dialog = new frmEditSanPham(sp);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void XoaSelected()
        {
            if (dgvSanPham.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = (int)dgvSanPham.CurrentRow.Cells["MaSanPham"].Value;
            string name = dgvSanPham.CurrentRow.Cells["TenSanPham"].Value?.ToString() ?? "";

            var res = MessageBox.Show($"Bạn có chắc muốn xóa sản phẩm '{name}' (Mã #{id})?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                if (new SanPhamDAO().Delete(id))
                {
                    MessageBox.Show("Đã xóa sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
            }
        }
    }
}
