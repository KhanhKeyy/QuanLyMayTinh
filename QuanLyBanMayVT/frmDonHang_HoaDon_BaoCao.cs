using System.Data;
using System.Data.SqlClient;
using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    /// <summary>Form quản lý & xác nhận đơn hàng</summary>
    public class frmDonHang : Form
    {
        private readonly bool _cheBoDuyet;

        private DataGridView dgvDonHang = null!;
        private ComboBox cboTrangThai = null!;
        private Button btnXemChiTiet = null!;
        private Button btnXacNhan = null!;
        private Button btnHuy = null!;

        // Phân trang
        private int _currentPage = 1;
        private const int PageSize = 10;
        private List<DonHang> _fullOrderList = new();

        private Panel pnlPagination = null!;
        private Label lblPageInfo = null!;
        private Button btnPrev = null!;
        private Button btnNext = null!;
        private FlowLayoutPanel pnlPageNumbers = null!;

        public frmDonHang(bool cheBoDuyet = false)
        {
            _cheBoDuyet = cheBoDuyet;
            InitUI();
            LoadData();
        }

        private void InitUI()
        {
            this.Text = _cheBoDuyet ? "Xác nhận đơn hàng" : "Danh sách đơn hàng";
            this.BackColor = UIStyleHelper.BgMain;
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F);

            // ── TOP PANEL (TableLayoutPanel → không bao giờ bị wrap) ─────
            var tblTop = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(12, 10, 12, 10),
                BackColor = UIStyleHelper.BgCard,
                ColumnCount = 10,   // nhiều cột → tự co giãn
                RowCount = 1,
            };
            tblTop.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            // Cho tất cả cột AutoSize
            for (int i = 0; i < 10; i++)
                tblTop.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var lblTT = new Label
            {
                Text = "Lọc trạng thái:",
                AutoSize = true,
                ForeColor = Color.FromArgb(17, 24, 39),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                Margin = new Padding(0, 6, 8, 0)
            };
            cboTrangThai = new ComboBox
            {
                Width = 170,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 3, 20, 0)
            };
            UIStyleHelper.StyleComboBox(cboTrangThai);
            cboTrangThai.Items.AddRange(new object[] { "-- Tất cả --", "Cho xac nhan", "Da xac nhan", "Hoan tat", "Da huy" });
            cboTrangThai.SelectedIndex = 0;
            cboTrangThai.SelectedIndexChanged += (s, e) => LoadData();

            btnXemChiTiet = new Button
            {
                Text = "🔍 Xem chi tiết đơn",
                AutoSize = true,
                Padding = new Padding(14, 6, 14, 6),
                Margin = new Padding(0, 0, 12, 0),
                BackColor = UIStyleHelper.PrimaryBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnXemChiTiet.FlatAppearance.BorderSize = 0;
            btnXemChiTiet.Click += (s, e) => MoPopupChiTietSelected();

            tblTop.Controls.Add(lblTT);
            tblTop.Controls.Add(cboTrangThai);
            tblTop.Controls.Add(btnXemChiTiet);

            if (_cheBoDuyet || UserSession.IsNVBanHang || UserSession.IsQuanLy)
            {
                btnXacNhan = new Button
                {
                    Text = "✅ Xác nhận đơn hàng",
                    AutoSize = true,
                    Padding = new Padding(14, 6, 14, 6),
                    Margin = new Padding(0, 0, 12, 0),
                    BackColor = UIStyleHelper.SuccessGreen,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnXacNhan.FlatAppearance.BorderSize = 0;
                btnXacNhan.Click += BtnXacNhan_Click;

                btnHuy = new Button
                {
                    Text = "❌ Huỷ đơn hàng",
                    AutoSize = true,
                    Padding = new Padding(14, 6, 14, 6),
                    Margin = new Padding(0, 0, 0, 0),
                    BackColor = UIStyleHelper.DangerRed,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnHuy.FlatAppearance.BorderSize = 0;
                btnHuy.Click += BtnHuy_Click;

                tblTop.Controls.Add(btnXacNhan);
                tblTop.Controls.Add(btnHuy);
            }

            dgvDonHang = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvDonHang);
            dgvDonHang.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) MoPopupChiTietSelected();
            };

            // ── PAGINATION BAR ──────────────────────────────────────────
            pnlPagination = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 52,
                BackColor = UIStyleHelper.BgCard,
                Padding = new Padding(16, 0, 16, 0)
            };
            pnlPagination.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240), 1), 0, 0, pnlPagination.Width, 0);

            lblPageInfo = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(16, 17)
            };

            btnPrev = new Button
            {
                Text = "◀",
                Size = new Size(36, 32),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPrev.Click += (s, e) => { if (_currentPage > 1) { _currentPage--; RenderPage(); } };

            btnNext = new Button
            {
                Text = "▶",
                Size = new Size(36, 32),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnNext.Click += (s, e) => {
                int totalPages = _fullOrderList.Count == 0 ? 1 : (int)Math.Ceiling((double)_fullOrderList.Count / PageSize);
                if (_currentPage < totalPages) { _currentPage++; RenderPage(); }
            };

            pnlPageNumbers = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent
            };

            pnlPagination.Controls.Add(lblPageInfo);
            pnlPagination.Controls.Add(btnPrev);
            pnlPagination.Controls.Add(pnlPageNumbers);
            pnlPagination.Controls.Add(btnNext);
            pnlPagination.Resize += (s, e) => RepositionPaginationControls();

            this.Controls.Add(dgvDonHang);
            this.Controls.Add(pnlPagination);
            this.Controls.Add(tblTop);
        }

        private void RepositionPaginationControls()
        {
            int rightX = pnlPagination.ClientSize.Width - 16;
            btnNext.Left = rightX - btnNext.Width;
            btnNext.Top = 10;

            pnlPageNumbers.Left = btnNext.Left - pnlPageNumbers.Width - 6;
            pnlPageNumbers.Top = 10;

            btnPrev.Left = pnlPageNumbers.Left - btnPrev.Width - 6;
            btnPrev.Top = 10;
        }

        private void LoadData()
        {
            string? filterTT = cboTrangThai.SelectedIndex > 0 ? cboTrangThai.SelectedItem?.ToString() : null;
            _fullOrderList = new DonHangDAO().GetAll(filterTT);
            _currentPage = 1;
            RenderPage();
        }

        private void RenderPage()
        {
            int totalCount = _fullOrderList.Count;
            int totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling((double)totalCount / PageSize);

            if (_currentPage > totalPages) _currentPage = totalPages;
            if (_currentPage < 1) _currentPage = 1;

            var pageItems = _fullOrderList
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            dgvDonHang.DataSource = pageItems.Select(d => new
            {
                d.MaDonHang,
                d.TenKhachHang,
                d.NgayDatHang,
                d.TenPhuongThuc,
                TongTien = d.TongTien.ToString("N0") + " đ",
                TrangThai = d.TrangThaiDisplay,
                NhanVienXacNhan = d.TenNhanVienXacNhan,
                d.GhiChu
            }).ToList();

            if (dgvDonHang.Columns["MaDonHang"] != null) dgvDonHang.Columns["MaDonHang"].HeaderText = "Mã Đơn";
            if (dgvDonHang.Columns["TenKhachHang"] != null) dgvDonHang.Columns["TenKhachHang"].HeaderText = "Khách Hàng";
            if (dgvDonHang.Columns["NgayDatHang"] != null) dgvDonHang.Columns["NgayDatHang"].HeaderText = "Ngày Đặt";
            if (dgvDonHang.Columns["TenPhuongThuc"] != null) dgvDonHang.Columns["TenPhuongThuc"].HeaderText = "Phương Thức TT";
            if (dgvDonHang.Columns["TongTien"] != null) dgvDonHang.Columns["TongTien"].HeaderText = "Tổng Tiền";
            if (dgvDonHang.Columns["TrangThai"] != null) dgvDonHang.Columns["TrangThai"].HeaderText = "Trạng Thái";
            if (dgvDonHang.Columns["NhanVienXacNhan"] != null) dgvDonHang.Columns["NhanVienXacNhan"].HeaderText = "NV Xác Nhận";
            if (dgvDonHang.Columns["GhiChu"] != null) dgvDonHang.Columns["GhiChu"].HeaderText = "Ghi Chú";

            int startIdx = totalCount == 0 ? 0 : (_currentPage - 1) * PageSize + 1;
            int endIdx = Math.Min(_currentPage * PageSize, totalCount);
            lblPageInfo.Text = $"Hiển thị {startIdx} - {endIdx} / Tổng {totalCount} đơn hàng (Trang {_currentPage}/{totalPages})";

            btnPrev.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < totalPages;

            btnPrev.BackColor = btnPrev.Enabled ? Color.White : Color.FromArgb(241, 245, 249);
            btnPrev.ForeColor = btnPrev.Enabled ? Color.FromArgb(30, 41, 59) : Color.FromArgb(148, 163, 184);
            btnPrev.FlatAppearance.BorderColor = btnPrev.Enabled ? Color.FromArgb(203, 213, 225) : Color.FromArgb(226, 232, 240);
            btnPrev.FlatAppearance.BorderSize = 1;

            btnNext.BackColor = btnNext.Enabled ? Color.White : Color.FromArgb(241, 245, 249);
            btnNext.ForeColor = btnNext.Enabled ? Color.FromArgb(30, 41, 59) : Color.FromArgb(148, 163, 184);
            btnNext.FlatAppearance.BorderColor = btnNext.Enabled ? Color.FromArgb(203, 213, 225) : Color.FromArgb(226, 232, 240);
            btnNext.FlatAppearance.BorderSize = 1;

            pnlPageNumbers.Controls.Clear();
            for (int i = 1; i <= totalPages; i++)
            {
                int pageNum = i;
                var btnNum = new Button
                {
                    Text = pageNum.ToString(),
                    Size = new Size(36, 32),
                    Margin = new Padding(3, 0, 3, 0),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };

                if (pageNum == _currentPage)
                {
                    btnNum.BackColor = UIStyleHelper.PrimaryBlue;
                    btnNum.ForeColor = Color.White;
                    btnNum.FlatAppearance.BorderColor = UIStyleHelper.PrimaryBlue;
                    btnNum.FlatAppearance.BorderSize = 1;
                }
                else
                {
                    btnNum.BackColor = Color.White;
                    btnNum.ForeColor = Color.FromArgb(30, 41, 59);
                    btnNum.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
                    btnNum.FlatAppearance.BorderSize = 1;
                }

                btnNum.Click += (s, e) =>
                {
                    _currentPage = pageNum;
                    RenderPage();
                };
                pnlPageNumbers.Controls.Add(btnNum);
            }

            RepositionPaginationControls();
        }

        private void MoPopupChiTietSelected()
        {
            if (dgvDonHang.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần xem chi tiết.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int maDH = (int)dgvDonHang.CurrentRow.Cells["MaDonHang"].Value;
            using var dialog = new frmChiTietDonHangDialog(maDH);
            dialog.ShowDialog(this);
        }

        private void BtnXacNhan_Click(object? sender, EventArgs e)
        {
            if (dgvDonHang.CurrentRow == null) return;
            int maDH = (int)dgvDonHang.CurrentRow.Cells["MaDonHang"].Value;

            var currentNV = UserSession.CurrentNhanVien;
            int maNV = currentNV != null ? currentNV.MaNhanVien : 1;

            if (new DonHangDAO().XacNhanDonHang(maDH, maNV))
            {
                MessageBox.Show($"Đã xác nhận đơn hàng #{maDH}!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            else
            {
                MessageBox.Show("Không thể xác nhận (đơn hàng phải ở trạng thái 'Chờ xác nhận').", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnHuy_Click(object? sender, EventArgs e)
        {
            if (dgvDonHang.CurrentRow == null) return;
            int maDH = (int)dgvDonHang.CurrentRow.Cells["MaDonHang"].Value;

            var res = MessageBox.Show($"Bạn có chắc muốn huỷ đơn hàng #{maDH}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                if (new DonHangDAO().HuyDonHang(maDH))
                {
                    MessageBox.Show($"Đã huỷ đơn hàng #{maDH}.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
            }
        }
    }

    /// <summary>Form quản lý hóa đơn (Kế toán & Quản lý)</summary>
    public class frmHoaDon : Form
    {
        private readonly int _defaultTabIndex;
        private TabControl tabControl = null!;
        private TabPage tabDanhSach = null!;
        private TabPage tabLapMoi = null!;

        // Tab Danh sach
        private DataGridView dgvHoaDon = null!;
        private Button btnThanhToan = null!;

        // Tab Lap moi
        private DataGridView dgvDonHangDaXacNhan = null!;
        private Button btnLapHoaDon = null!;

        public frmHoaDon(int defaultTabIndex = 0)
        {
            _defaultTabIndex = defaultTabIndex;
            InitUI();
            if (_defaultTabIndex == 1)
            {
                tabControl.SelectedTab = tabLapMoi;
                LoadDonHangDaXacNhan();
            }
            else
            {
                LoadDataHoaDon();
            }
        }

        private void InitUI()
        {
            this.Text = "Hóa đơn bán hàng";
            this.BackColor = UIStyleHelper.BgMain;
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F);

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Appearance = TabAppearance.FlatButtons,
                ItemSize = new Size(0, 1),
                SizeMode = TabSizeMode.Fixed
            };

            // ── Tab 1: Danh sách hóa đơn ─────────────────────────────
            tabDanhSach = new TabPage("🧾 Danh sách hóa đơn");
            tabDanhSach.BackColor = UIStyleHelper.BgMain;

            var tblTopDS = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(12, 10, 12, 10),
                BackColor = UIStyleHelper.BgCard,
                ColumnCount = 5,
                RowCount = 1,
            };
            tblTopDS.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            for (int i = 0; i < 5; i++)
                tblTopDS.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            btnThanhToan = new Button
            {
                Text = "💳 Xác nhận thanh toán (Hoàn tất)",
                AutoSize = true,
                Padding = new Padding(14, 6, 14, 6),
                Margin = new Padding(0, 0, 0, 0),
                BackColor = UIStyleHelper.SuccessGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnThanhToan.FlatAppearance.BorderSize = 0;
            btnThanhToan.Click += BtnThanhToan_Click;
            tblTopDS.Controls.Add(btnThanhToan);

            dgvHoaDon = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvHoaDon);

            tabDanhSach.Controls.Add(dgvHoaDon);
            tabDanhSach.Controls.Add(tblTopDS);

            // ── Tab 2: Lập hóa đơn mới ───────────────────────────────
            tabLapMoi = new TabPage("➕ Lập hóa đơn mới");
            tabLapMoi.BackColor = UIStyleHelper.BgMain;

            var tblTopLM = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(12, 10, 12, 10),
                BackColor = UIStyleHelper.BgCard,
                ColumnCount = 5,
                RowCount = 1,
            };
            tblTopLM.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            for (int i = 0; i < 5; i++)
                tblTopLM.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            btnLapHoaDon = new Button
            {
                Text = "🧾 Lập hóa đơn cho đơn đã chọn",
                AutoSize = true,
                Padding = new Padding(14, 6, 14, 6),
                Margin = new Padding(0, 0, 0, 0),
                BackColor = UIStyleHelper.PrimaryBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLapHoaDon.FlatAppearance.BorderSize = 0;
            btnLapHoaDon.Click += BtnLapHoaDon_Click;
            tblTopLM.Controls.Add(btnLapHoaDon);

            dgvDonHangDaXacNhan = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvDonHangDaXacNhan);

            tabLapMoi.Controls.Add(dgvDonHangDaXacNhan);
            tabLapMoi.Controls.Add(tblTopLM);

            tabControl.TabPages.Add(tabDanhSach);
            tabControl.TabPages.Add(tabLapMoi);
            tabControl.SelectedIndexChanged += (s, e) =>
            {
                if (tabControl.SelectedTab == tabLapMoi) LoadDonHangDaXacNhan();
                else LoadDataHoaDon();
            };

            this.Controls.Add(tabControl);
        }

        private void LoadDataHoaDon()
        {
            var list = new HoaDonDAO().GetAll();
            dgvHoaDon.DataSource = list.Select(h => new
            {
                h.MaHoaDon,
                h.MaDonHang,
                h.TenKhachHang,
                h.TenKeToan,
                h.NgayLapHoaDon,
                TongTien = h.TongTienFormatted,
                TrangThai = h.TrangThaiDisplay,
                NgayThanhToan = h.NgayThanhToan?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa"
            }).ToList();

            if (dgvHoaDon.Columns["MaHoaDon"] != null) dgvHoaDon.Columns["MaHoaDon"].HeaderText = "Mã HĐ";
            if (dgvHoaDon.Columns["MaDonHang"] != null) dgvHoaDon.Columns["MaDonHang"].HeaderText = "Mã Đơn";
            if (dgvHoaDon.Columns["TenKhachHang"] != null) dgvHoaDon.Columns["TenKhachHang"].HeaderText = "Khách Hàng";
            if (dgvHoaDon.Columns["TenKeToan"] != null) dgvHoaDon.Columns["TenKeToan"].HeaderText = "Kế Toán Lập";
            if (dgvHoaDon.Columns["NgayLapHoaDon"] != null) dgvHoaDon.Columns["NgayLapHoaDon"].HeaderText = "Ngày Lập";
            if (dgvHoaDon.Columns["TongTien"] != null) dgvHoaDon.Columns["TongTien"].HeaderText = "Tổng Tiền";
            if (dgvHoaDon.Columns["TrangThai"] != null) dgvHoaDon.Columns["TrangThai"].HeaderText = "Trạng Thái TT";
            if (dgvHoaDon.Columns["NgayThanhToan"] != null) dgvHoaDon.Columns["NgayThanhToan"].HeaderText = "Ngày Thanh Toán";
        }

        private void LoadDonHangDaXacNhan()
        {
            var list = new DonHangDAO().GetAll("Da xac nhan");
            dgvDonHangDaXacNhan.DataSource = list.Select(d => new
            {
                d.MaDonHang,
                d.TenKhachHang,
                d.NgayDatHang,
                d.TenPhuongThuc,
                TongTien = d.TongTien.ToString("N0") + " đ",
                TrangThai = d.TrangThaiDisplay
            }).ToList();

            if (dgvDonHangDaXacNhan.Columns["MaDonHang"] != null) dgvDonHangDaXacNhan.Columns["MaDonHang"].HeaderText = "Mã Đơn";
            if (dgvDonHangDaXacNhan.Columns["TenKhachHang"] != null) dgvDonHangDaXacNhan.Columns["TenKhachHang"].HeaderText = "Khách Hàng";
            if (dgvDonHangDaXacNhan.Columns["NgayDatHang"] != null) dgvDonHangDaXacNhan.Columns["NgayDatHang"].HeaderText = "Ngày Đặt";
            if (dgvDonHangDaXacNhan.Columns["TenPhuongThuc"] != null) dgvDonHangDaXacNhan.Columns["TenPhuongThuc"].HeaderText = "Phương Thức TT";
            if (dgvDonHangDaXacNhan.Columns["TongTien"] != null) dgvDonHangDaXacNhan.Columns["TongTien"].HeaderText = "Tổng Tiền";
            if (dgvDonHangDaXacNhan.Columns["TrangThai"] != null) dgvDonHangDaXacNhan.Columns["TrangThai"].HeaderText = "Trạng Thái";
        }

        private void BtnLapHoaDon_Click(object? sender, EventArgs e)
        {
            if (dgvDonHangDaXacNhan.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn đơn hàng đã xác nhận.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int maDH = (int)dgvDonHangDaXacNhan.CurrentRow.Cells["MaDonHang"].Value;
            var dh = new DonHangDAO().GetById(maDH);
            if (dh == null) return;

            var currentNV = UserSession.CurrentNhanVien;
            int maKeToan = currentNV != null ? currentNV.MaNhanVien : 1;

            var hd = new HoaDon
            {
                MaDonHang = maDH,
                MaKeToan = maKeToan,
                TongTien = dh.TongTien
            };

            int maHD = new HoaDonDAO().Insert(hd);
            if (maHD > 0)
            {
                MessageBox.Show($"Đã lập hóa đơn #{maHD} thành công cho đơn hàng #{maDH}!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tabControl.SelectedTab = tabDanhSach;
                LoadDataHoaDon();
            }
        }

        private void BtnThanhToan_Click(object? sender, EventArgs e)
        {
            if (dgvHoaDon.CurrentRow == null) return;
            int maHD = (int)dgvHoaDon.CurrentRow.Cells["MaHoaDon"].Value;

            var res = MessageBox.Show($"Xác nhận khách hàng đã thanh toán hóa đơn #{maHD}?\n(Hệ thống sẽ cập nhật đơn hàng thành Hoàn Tất và giảm tồn kho)",
                "Xác nhận thanh toán", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (res == DialogResult.Yes)
            {
                if (new HoaDonDAO().CapNhatTrangThaiThanhToan(maHD, "Da thanh toan"))
                {
                    MessageBox.Show($"Thanh toán hóa đơn #{maHD} thành công!\nĐã cập nhật tồn kho & thông báo cho khách hàng.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataHoaDon();
                }
            }
        }
    }

    /// <summary>Form báo cáo thống kê (Doanh thu, Tồn kho, Sản phẩm bán chạy)</summary>
    public class ThongKeRow
    {
        public int STT { get; set; }
        public string Ngay { get; set; } = "";
        public decimal DoanhThu { get; set; }
        public decimal ChiPhi { get; set; }
        public decimal LoiNhuan { get; set; }
        public int SoHoaDon { get; set; }
    }

    public class TonKhoRow
    {
        public int STT { get; set; }
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; } = "";
        public string TenDanhMuc { get; set; } = "";
        public int SoLuongTon { get; set; }
        public int MucTonToiThieu { get; set; }
        public int CanNhapThem { get; set; }
        public string TrangThai { get; set; } = "";
    }

    public class BanChayRow
    {
        public int STT { get; set; }
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; } = "";
        public string TenDanhMuc { get; set; } = "";
        public int TongDaBan { get; set; }
        public decimal TongDoanhThu { get; set; }
    }

    public class frmBaoCao : Form
    {
        private string _loaiBaoCaoMode;

        // Metric Card Labels & Titles
        private Label lblCard1Title = null!, lblCardDoanhThu = null!;
        private Label lblCard2Title = null!, lblCardChiPhi = null!;
        private Label lblCard3Title = null!, lblCardLoiNhuan = null!;
        private Label lblCard4Title = null!, lblCardSoHoaDon = null!;

        // Title
        private Label lblTitle = null!;

        // Filter Controls
        private Label lblTG = null!, lblNam = null!, lblThang = null!;
        private ComboBox cboLoaiThoiGian = null!;
        private ComboBox cboNam = null!;
        private ComboBox cboThang = null!;
        private Button btnXem = null!;
        private Button btnXuat = null!;

        // Data Grid
        private DataGridView dgvThongKe = null!;

        private Panel card1 = null!, card2 = null!, card3 = null!, card4 = null!;
        private FlowLayoutPanel flowCards = null!;

        // Phân trang
        private int _currentPage = 1;
        private const int PageSize = 10;
        private List<ThongKeRow> _fullDataList = new();
        private List<TonKhoRow> _fullTonKhoList = new();
        private List<BanChayRow> _fullBanChayList = new();

        private Panel pnlPagination = null!;
        private Label lblPageInfo = null!;
        private Button btnPrev = null!;
        private Button btnNext = null!;
        private FlowLayoutPanel pnlPageNumbers = null!;

        public frmBaoCao(string loaiBaoCao = "DoanhThu")
        {
            _loaiBaoCaoMode = string.IsNullOrEmpty(loaiBaoCao) ? "DoanhThu" : loaiBaoCao;
            InitUI();
            ChonLoaiBaoCao(_loaiBaoCaoMode);
        }

        private void InitUI()
        {
            this.Text = "Báo cáo thống kê";
            this.BackColor = UIStyleHelper.BgMain;
            this.ForeColor = Color.FromArgb(17, 24, 39);
            this.Font = new Font("Segoe UI", 9.5F);

            // ── 1. TOP TITLE ─────────────────────────────────────────────
            var pnlTitle = new Panel
            {
                Dock = DockStyle.Top,
                Height = 45,
                Padding = new Padding(18, 10, 18, 0),
                BackColor = UIStyleHelper.BgMain
            };

            lblTitle = new Label
            {
                Text = "Thống kê",
                Font = new Font("Segoe UI", 13.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Dock = DockStyle.Left,
                AutoSize = true
            };

            pnlTitle.Controls.Add(lblTitle);

            // ── 2. METRIC KPI CARDS (4 CARDS) ───────────────────────────
            var pnlCardsContainer = new Panel
            {
                Dock = DockStyle.Top,
                Height = 92,
                Padding = new Padding(18, 5, 18, 5),
                BackColor = UIStyleHelper.BgMain
            };

            flowCards = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = UIStyleHelper.BgMain
            };

            card1 = CreateMetricCard("Doanh thu", out lblCard1Title, out lblCardDoanhThu, Color.FromArgb(37, 99, 235), Color.FromArgb(37, 99, 235));
            card2 = CreateMetricCard("Chi phí",   out lblCard2Title, out lblCardChiPhi,   Color.FromArgb(225, 29, 72), Color.FromArgb(225, 29, 72));
            card3 = CreateMetricCard("Lợi nhuận", out lblCard3Title, out lblCardLoiNhuan, Color.FromArgb(5, 150, 105), Color.FromArgb(5, 150, 105));
            card4 = CreateMetricCard("Số hóa đơn",out lblCard4Title, out lblCardSoHoaDon, Color.FromArgb(124, 58, 237), Color.FromArgb(124, 58, 237));

            flowCards.Controls.Add(card1);
            flowCards.Controls.Add(card2);
            flowCards.Controls.Add(card3);
            flowCards.Controls.Add(card4);

            pnlCardsContainer.Controls.Add(flowCards);
            pnlCardsContainer.Resize += (s, e) => RepositionCards();

            // ── 3. FILTER BAR (Thời gian, Năm, Tháng) ─────────────────
            var pnlFilter = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                Padding = new Padding(18, 10, 18, 10),
                BackColor = UIStyleHelper.BgMain
            };

            var flowFilter = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = UIStyleHelper.BgMain
            };

            lblTG = new Label
            {
                Text = "Thời gian:",
                AutoSize = true,
                Margin = new Padding(0, 6, 8, 0),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(71, 85, 105)
            };
            cboLoaiThoiGian = new ComboBox { Width = 110, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(0, 2, 20, 0) };
            UIStyleHelper.StyleComboBox(cboLoaiThoiGian);
            cboLoaiThoiGian.Items.AddRange(new object[] { "Tháng", "Năm", "Tất cả" });
            cboLoaiThoiGian.SelectedIndex = 0;

            lblNam = new Label
            {
                Text = "Năm:",
                AutoSize = true,
                Margin = new Padding(0, 6, 8, 0),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(71, 85, 105)
            };
            cboNam = new ComboBox { Width = 100, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(0, 2, 20, 0) };
            UIStyleHelper.StyleComboBox(cboNam);
            int curYear = DateTime.Now.Year;
            for (int y = curYear; y >= curYear - 5; y--) cboNam.Items.Add(y);
            cboNam.SelectedIndex = 0;

            lblThang = new Label
            {
                Text = "Tháng:",
                AutoSize = true,
                Margin = new Padding(0, 6, 8, 0),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(71, 85, 105)
            };
            cboThang = new ComboBox { Width = 100, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(0, 2, 30, 0) };
            UIStyleHelper.StyleComboBox(cboThang);
            cboThang.Items.Add("Tất cả");
            for (int m = 1; m <= 12; m++) cboThang.Items.Add(m.ToString());
            cboThang.SelectedIndex = DateTime.Now.Month;

            btnXem = new Button
            {
                Text = "Xem",
                Size = new Size(95, 34),
                Margin = new Padding(0, 0, 15, 0),
                BackColor = Color.FromArgb(2, 132, 199),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnXem.FlatAppearance.BorderSize = 0;
            btnXem.Click += (s, e) => LoadDataCurrentMode();

            btnXuat = new Button
            {
                Text = "⬇ Xuất báo cáo",
                Size = new Size(135, 34),
                Margin = new Padding(0, 0, 0, 0),
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnXuat.FlatAppearance.BorderSize = 0;
            btnXuat.Click += (s, e) => XuatBaoCaoCsv();

            flowFilter.Controls.Add(lblTG);
            flowFilter.Controls.Add(cboLoaiThoiGian);
            flowFilter.Controls.Add(lblNam);
            flowFilter.Controls.Add(cboNam);
            flowFilter.Controls.Add(lblThang);
            flowFilter.Controls.Add(cboThang);
            flowFilter.Controls.Add(btnXem);
            flowFilter.Controls.Add(btnXuat);

            pnlFilter.Controls.Add(flowFilter);

            // ── 4. DATA GRID VIEW ───────────────────────────────────────
            var pnlGridContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(18, 5, 18, 18),
                BackColor = UIStyleHelper.BgMain
            };

            dgvThongKe = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvThongKe);
            dgvThongKe.CellFormatting += DgvThongKe_CellFormatting;

            pnlGridContainer.Controls.Add(dgvThongKe);

            // ── 5. PAGINATION BAR ─────────────────────────────────────────
            pnlPagination = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 52,
                BackColor = UIStyleHelper.BgCard,
                Padding = new Padding(16, 0, 16, 0)
            };
            pnlPagination.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240), 1), 0, 0, pnlPagination.Width, 0);

            lblPageInfo = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(16, 17)
            };

            btnPrev = new Button
            {
                Text = "◀",
                Size = new Size(36, 32),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPrev.Click += (s, e) => { if (_currentPage > 1) { _currentPage--; RenderPage(); } };

            btnNext = new Button
            {
                Text = "▶",
                Size = new Size(36, 32),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnNext.Click += (s, e) => {
                int totalItems = GetCurrentTotalCount();
                int totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling((double)totalItems / PageSize);
                if (_currentPage < totalPages) { _currentPage++; RenderPage(); }
            };

            pnlPageNumbers = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent
            };

            pnlPagination.Controls.Add(lblPageInfo);
            pnlPagination.Controls.Add(btnPrev);
            pnlPagination.Controls.Add(pnlPageNumbers);
            pnlPagination.Controls.Add(btnNext);
            pnlPagination.Resize += (s, e) => RepositionPaginationControls();

            this.Controls.Add(pnlGridContainer);
            this.Controls.Add(pnlPagination);
            this.Controls.Add(pnlFilter);
            this.Controls.Add(pnlCardsContainer);
            this.Controls.Add(pnlTitle);
        }

        private void ChonLoaiBaoCao(string mode)
        {
            _loaiBaoCaoMode = mode;

            if (mode == "DoanhThu")
            {
                lblTitle.Text = "📈 Thống kê Doanh Thu & Lợi Nhuận";
                lblTG.Visible = cboLoaiThoiGian.Visible = lblNam.Visible = cboNam.Visible = lblThang.Visible = cboThang.Visible = btnXem.Visible = true;
                btnXuat.Text = "⬇ Xuất báo cáo DT";
            }
            else if (mode == "TonKho")
            {
                lblTitle.Text = "📦 Báo cáo Hàng Tồn Kho & Cảnh Báo Cần Nhập";
                lblTG.Visible = cboLoaiThoiGian.Visible = lblNam.Visible = cboNam.Visible = lblThang.Visible = cboThang.Visible = false;
                btnXem.Visible = false;
                btnXuat.Text = "⬇ Xuất báo cáo Tồn kho";
            }
            else if (mode == "BanChay")
            {
                lblTitle.Text = "🔥 Báo cáo Sản Phẩm Bán Chạy Nhất";
                lblTG.Visible = cboLoaiThoiGian.Visible = lblNam.Visible = cboNam.Visible = lblThang.Visible = cboThang.Visible = false;
                btnXem.Visible = false;
                btnXuat.Text = "⬇ Xuất SP Bán Chạy";
            }

            LoadDataCurrentMode();
        }

        private Panel CreateMetricCard(string defaultTitle, out Label lblTitle, out Label lblValue, Color accentColor, Color textColor)
        {
            var card = new Panel
            {
                Height = 80,
                Margin = new Padding(0, 0, 16, 0),
                BackColor = Color.White,
                Padding = new Padding(16, 10, 16, 10)
            };
            card.Paint += (s, e) =>
            {
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(226, 232, 240), 1), 0, 0, card.Width - 1, card.Height - 1);
                using var b = new SolidBrush(accentColor);
                e.Graphics.FillRectangle(b, 0, 0, 5, card.Height);
            };

            lblTitle = new Label
            {
                Text = defaultTitle,
                Dock = DockStyle.Top,
                Height = 22,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblValue = new Label
            {
                Text = "0",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12.5F, FontStyle.Bold),
                ForeColor = textColor,
                TextAlign = ContentAlignment.MiddleLeft
            };

            card.Controls.Add(lblValue);
            card.Controls.Add(lblTitle);
            return card;
        }

        private void RepositionCards()
        {
            if (flowCards == null) return;
            int totalW = flowCards.ClientSize.Width;
            int cardW = (totalW - 3 * 16) / 4;
            if (cardW < 140) cardW = 140;

            card1.Width = cardW;
            card2.Width = cardW;
            card3.Width = cardW;
            card4.Width = cardW;
        }

        private void RepositionPaginationControls()
        {
            if (pnlPagination == null) return;
            int rightX = pnlPagination.ClientSize.Width - 16;
            btnNext.Left = rightX - btnNext.Width;
            btnNext.Top = 10;

            pnlPageNumbers.Left = btnNext.Left - pnlPageNumbers.Width - 6;
            pnlPageNumbers.Top = 10;

            btnPrev.Left = pnlPageNumbers.Left - btnPrev.Width - 6;
            btnPrev.Top = 10;
        }

        private void LoadDataCurrentMode()
        {
            if (_loaiBaoCaoMode == "TonKho") LoadTonKhoData();
            else if (_loaiBaoCaoMode == "BanChay") LoadBanChayData();
            else LoadThongKeData();
        }

        private void LoadThongKeData()
        {
            try
            {
                int nam = cboNam.SelectedItem is int n ? n : DateTime.Now.Year;
                int thang = cboThang.SelectedIndex; // 0 = Tất cả
                string loaiTG = cboLoaiThoiGian.SelectedItem?.ToString() ?? "Tháng";

                using var conn = DatabaseHelper.GetConnection();

                // 1. Doanh thu theo Ngày từ HoaDon
                var dictDoanhThu = new Dictionary<DateTime, (decimal Revenue, int Invoices)>();
                const string sqlHD = @"
                    SELECT CAST(NgayThanhToan AS DATE) AS Ngay,
                           SUM(TongTien) AS DoanhThu,
                           COUNT(1) AS SoHoaDon
                    FROM HoaDon
                    WHERE TrangThaiThanhToan = 'Da thanh toan'
                      AND YEAR(NgayThanhToan) = @Nam
                      AND (@Thang = 0 OR MONTH(NgayThanhToan) = @Thang)
                    GROUP BY CAST(NgayThanhToan AS DATE)";

                using (var cmd = new SqlCommand(sqlHD, conn))
                {
                    cmd.Parameters.AddWithValue("@Nam", nam);
                    cmd.Parameters.AddWithValue("@Thang", thang);
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        var dt = (DateTime)r["Ngay"];
                        decimal rev = r["DoanhThu"] != DBNull.Value ? Convert.ToDecimal(r["DoanhThu"]) : 0;
                        int inv = Convert.ToInt32(r["SoHoaDon"]);
                        dictDoanhThu[dt] = (rev, inv);
                    }
                }

                // 2. Chi phí vốn hàng bán — tính theo ngày bán hàng (70% giá bán × số lượng)
                //    Cách này đảm bảo Chi phí luôn xuất hiện cùng ngày với Doanh thu
                var dictChiPhi = new Dictionary<DateTime, decimal>();
                const string sqlPN = @"
                    SELECT CAST(hd.NgayThanhToan AS DATE) AS Ngay,
                           SUM(ct.SoLuong * sp.GiaBan * 0.7) AS ChiPhi
                    FROM HoaDon hd
                    INNER JOIN DonHang   dh ON hd.MaDonHang  = dh.MaDonHang
                    INNER JOIN ChiTietDonHang ct ON dh.MaDonHang = ct.MaDonHang
                    INNER JOIN SanPham   sp ON ct.MaSanPham  = sp.MaSanPham
                    WHERE hd.TrangThaiThanhToan = 'Da thanh toan'
                      AND YEAR(hd.NgayThanhToan)  = @Nam
                      AND (@Thang = 0 OR MONTH(hd.NgayThanhToan) = @Thang)
                    GROUP BY CAST(hd.NgayThanhToan AS DATE)";

                using (var cmd = new SqlCommand(sqlPN, conn))
                {
                    cmd.Parameters.AddWithValue("@Nam", nam);
                    cmd.Parameters.AddWithValue("@Thang", thang);
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        var dt = (DateTime)r["Ngay"];
                        decimal cost = r["ChiPhi"] != DBNull.Value ? Convert.ToDecimal(r["ChiPhi"]) : 0;
                        dictChiPhi[dt] = cost;
                    }
                }

                // 3. Tổng hợp số liệu theo dòng
                var dataList = new List<ThongKeRow>();

                if (loaiTG == "Tháng" && thang > 0)
                {
                    int daysInMonth = DateTime.DaysInMonth(nam, thang);
                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        var date = new DateTime(nam, thang, day);
                        dictDoanhThu.TryGetValue(date, out var revData);
                        dictChiPhi.TryGetValue(date, out decimal cost);

                        decimal rev = revData.Revenue;
                        int inv = revData.Invoices;
                        decimal profit = rev - cost;

                        dataList.Add(new ThongKeRow
                        {
                            STT = day,
                            Ngay = date.ToString("dd/MM/yyyy"),
                            DoanhThu = rev,
                            ChiPhi = cost,
                            LoiNhuan = profit,
                            SoHoaDon = inv
                        });
                    }
                }
                else
                {
                    var allDates = dictDoanhThu.Keys.Union(dictChiPhi.Keys).OrderBy(d => d).ToList();
                    int stt = 1;
                    foreach (var date in allDates)
                    {
                        dictDoanhThu.TryGetValue(date, out var revData);
                        dictChiPhi.TryGetValue(date, out decimal cost);

                        decimal rev = revData.Revenue;
                        int inv = revData.Invoices;
                        decimal profit = rev - cost;

                        dataList.Add(new ThongKeRow
                        {
                            STT = stt++,
                            Ngay = date.ToString("dd/MM/yyyy"),
                            DoanhThu = rev,
                            ChiPhi = cost,
                            LoiNhuan = profit,
                            SoHoaDon = inv
                        });
                    }
                }

                // 4. Cập nhật 4 Metric Cards KPI
                decimal totalRev = dataList.Sum(x => x.DoanhThu);
                decimal totalCost = dataList.Sum(x => x.ChiPhi);
                decimal totalProfit = totalRev - totalCost;
                int totalInvoices = dataList.Sum(x => x.SoHoaDon);

                lblCard1Title.Text = "Doanh thu";
                lblCardDoanhThu.Text = $"{totalRev:N0} đ";

                lblCard2Title.Text = "Chi phí";
                lblCardChiPhi.Text = $"{totalCost:N0} đ";

                lblCard3Title.Text = "Lợi nhuận";
                lblCardLoiNhuan.Text = (totalProfit >= 0 ? "" : "-") + Math.Abs(totalProfit).ToString("N0") + " đ";
                lblCardLoiNhuan.ForeColor = totalProfit >= 0 ? Color.FromArgb(5, 150, 105) : Color.FromArgb(225, 29, 72);

                lblCard4Title.Text = "Số hóa đơn";
                lblCardSoHoaDon.Text = totalInvoices.ToString();

                _fullDataList = dataList;
                _currentPage = 1;
                RenderPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu thống kê:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTonKhoData()
        {
            try
            {
                var list = new SanPhamDAO().GetAll();
                int totalSP = list.Count;
                int canNhap = list.Count(p => p.SoLuongTon < p.MucTonToiThieu);
                int hetHang = list.Count(p => p.SoLuongTon <= 0);
                int totalTon = list.Sum(p => p.SoLuongTon);

                lblCard1Title.Text = "Tổng SP kinh doanh";
                lblCardDoanhThu.Text = totalSP.ToString() + " loại";

                lblCard2Title.Text = "Cần nhập thêm";
                lblCardChiPhi.Text = canNhap.ToString() + " loại";

                lblCard3Title.Text = "Hàng đã hết";
                lblCardLoiNhuan.Text = hetHang.ToString() + " loại";
                lblCardLoiNhuan.ForeColor = hetHang > 0 ? Color.FromArgb(225, 29, 72) : Color.FromArgb(5, 150, 105);

                lblCard4Title.Text = "Tổng số lượng tồn";
                lblCardSoHoaDon.Text = totalTon.ToString("N0") + " cái";

                int stt = 1;
                _fullTonKhoList = list.Select(p => new TonKhoRow
                {
                    STT = stt++,
                    MaSanPham = p.MaSanPham,
                    TenSanPham = p.TenSanPham,
                    TenDanhMuc = p.TenDanhMuc,
                    SoLuongTon = p.SoLuongTon,
                    MucTonToiThieu = p.MucTonToiThieu,
                    CanNhapThem = Math.Max(0, p.MucTonToiThieu - p.SoLuongTon),
                    TrangThai = p.TrangThaiDisplay
                }).ToList();

                _currentPage = 1;
                RenderPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu tồn kho:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadBanChayData()
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    SELECT
                        sp.MaSanPham,
                        sp.TenSanPham,
                        ISNULL(dm.TenDanhMuc, N'Khác') AS TenDanhMuc,
                        SUM(ct.SoLuong) AS TongDaBan,
                        SUM(ct.ThanhTien) AS TongDoanhThu
                    FROM ChiTietDonHang ct
                    INNER JOIN DonHang dh ON ct.MaDonHang = dh.MaDonHang
                    INNER JOIN SanPham sp ON ct.MaSanPham = sp.MaSanPham
                    LEFT JOIN DanhMucSanPham dm ON sp.MaDanhMuc = dm.MaDanhMuc
                    WHERE dh.TrangThaiDonHang IN ('Da xac nhan', 'Hoan tat')
                    GROUP BY sp.MaSanPham, sp.TenSanPham, dm.TenDanhMuc
                    ORDER BY SUM(ct.SoLuong) DESC";

                using var cmd = new SqlCommand(sql, conn);
                using var da = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);

                var list = new List<BanChayRow>();
                int rank = 1;
                foreach (DataRow r in dt.Rows)
                {
                    list.Add(new BanChayRow
                    {
                        STT = rank++,
                        MaSanPham = Convert.ToInt32(r["MaSanPham"]),
                        TenSanPham = r["TenSanPham"]?.ToString() ?? "",
                        TenDanhMuc = r["TenDanhMuc"]?.ToString() ?? "",
                        TongDaBan = Convert.ToInt32(r["TongDaBan"]),
                        TongDoanhThu = Convert.ToDecimal(r["TongDoanhThu"])
                    });
                }

                int totalBan = list.Sum(x => x.TongDaBan);
                decimal totalRev = list.Sum(x => x.TongDoanhThu);
                string top1Name = list.Count > 0 ? list[0].TenSanPham : "Chưa có";

                lblCard1Title.Text = "Tổng SP đã bán";
                lblCardDoanhThu.Text = totalBan.ToString("N0") + " cái";

                lblCard2Title.Text = "Doanh thu bán SP";
                lblCardChiPhi.Text = totalRev.ToString("N0") + " đ";

                lblCard3Title.Text = "Sản phẩm Top 1";
                lblCardLoiNhuan.Text = top1Name;
                lblCardLoiNhuan.ForeColor = Color.FromArgb(124, 58, 237);

                lblCard4Title.Text = "Số loại SP đã bán";
                lblCardSoHoaDon.Text = list.Count.ToString() + " loại";

                _fullBanChayList = list;
                _currentPage = 1;
                RenderPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu SP bán chạy:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetCurrentTotalCount()
        {
            if (_loaiBaoCaoMode == "TonKho") return _fullTonKhoList.Count;
            if (_loaiBaoCaoMode == "BanChay") return _fullBanChayList.Count;
            return _fullDataList.Count;
        }

        private void RenderPage()
        {
            if (_loaiBaoCaoMode == "TonKho") RenderTonKhoPage();
            else if (_loaiBaoCaoMode == "BanChay") RenderBanChayPage();
            else RenderDoanhThuPage();
        }

        private void RenderDoanhThuPage()
        {
            int totalCount = _fullDataList.Count;
            int totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling((double)totalCount / PageSize);

            if (_currentPage > totalPages) _currentPage = totalPages;
            if (_currentPage < 1) _currentPage = 1;

            var pageItems = _fullDataList
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            dgvThongKe.DataSource = pageItems.Select(x => new
            {
                x.STT,
                x.Ngay,
                DoanhThu = x.DoanhThu.ToString("N0") + " đ",
                ChiPhi = x.ChiPhi.ToString("N0") + " đ",
                LoiNhuan = (x.LoiNhuan >= 0 ? "" : "-") + Math.Abs(x.LoiNhuan).ToString("N0") + " đ",
                x.SoHoaDon
            }).ToList();

            if (dgvThongKe.Columns["STT"] != null) dgvThongKe.Columns["STT"].HeaderText = "STT";
            if (dgvThongKe.Columns["Ngay"] != null) dgvThongKe.Columns["Ngay"].HeaderText = "Ngày";
            if (dgvThongKe.Columns["DoanhThu"] != null) dgvThongKe.Columns["DoanhThu"].HeaderText = "Doanh thu";
            if (dgvThongKe.Columns["ChiPhi"] != null) dgvThongKe.Columns["ChiPhi"].HeaderText = "Chi phí";
            if (dgvThongKe.Columns["LoiNhuan"] != null) dgvThongKe.Columns["LoiNhuan"].HeaderText = "Lợi nhuận";
            if (dgvThongKe.Columns["SoHoaDon"] != null) dgvThongKe.Columns["SoHoaDon"].HeaderText = "Số hóa đơn";

            UpdatePaginationFooter(totalCount, totalPages, "ngày");
        }

        private void RenderTonKhoPage()
        {
            int totalCount = _fullTonKhoList.Count;
            int totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling((double)totalCount / PageSize);

            if (_currentPage > totalPages) _currentPage = totalPages;
            if (_currentPage < 1) _currentPage = 1;

            var pageItems = _fullTonKhoList
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            dgvThongKe.DataSource = pageItems.Select(x => new
            {
                x.STT,
                x.MaSanPham,
                x.TenSanPham,
                x.TenDanhMuc,
                x.SoLuongTon,
                x.MucTonToiThieu,
                x.CanNhapThem,
                x.TrangThai
            }).ToList();

            if (dgvThongKe.Columns["STT"] != null) dgvThongKe.Columns["STT"].HeaderText = "STT";
            if (dgvThongKe.Columns["MaSanPham"] != null) dgvThongKe.Columns["MaSanPham"].HeaderText = "Mã SP";
            if (dgvThongKe.Columns["TenSanPham"] != null) dgvThongKe.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";
            if (dgvThongKe.Columns["TenDanhMuc"] != null) dgvThongKe.Columns["TenDanhMuc"].HeaderText = "Danh Mục";
            if (dgvThongKe.Columns["SoLuongTon"] != null) dgvThongKe.Columns["SoLuongTon"].HeaderText = "Tồn Hiện Tại";
            if (dgvThongKe.Columns["MucTonToiThieu"] != null) dgvThongKe.Columns["MucTonToiThieu"].HeaderText = "Mức Tối Thiểu";
            if (dgvThongKe.Columns["CanNhapThem"] != null) dgvThongKe.Columns["CanNhapThem"].HeaderText = "Cần Nhập Thêm";
            if (dgvThongKe.Columns["TrangThai"] != null) dgvThongKe.Columns["TrangThai"].HeaderText = "Trạng Thái";

            UpdatePaginationFooter(totalCount, totalPages, "sản phẩm");
        }

        private void RenderBanChayPage()
        {
            int totalCount = _fullBanChayList.Count;
            int totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling((double)totalCount / PageSize);

            if (_currentPage > totalPages) _currentPage = totalPages;
            if (_currentPage < 1) _currentPage = 1;

            var pageItems = _fullBanChayList
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            dgvThongKe.DataSource = pageItems.Select(x => new
            {
                Top = $"Top {x.STT}",
                x.MaSanPham,
                x.TenSanPham,
                x.TenDanhMuc,
                TongDaBan = x.TongDaBan.ToString("N0") + " cái",
                TongDoanhThu = x.TongDoanhThu.ToString("N0") + " đ"
            }).ToList();

            if (dgvThongKe.Columns["Top"] != null) dgvThongKe.Columns["Top"].HeaderText = "Thứ Hạng";
            if (dgvThongKe.Columns["MaSanPham"] != null) dgvThongKe.Columns["MaSanPham"].HeaderText = "Mã SP";
            if (dgvThongKe.Columns["TenSanPham"] != null) dgvThongKe.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";
            if (dgvThongKe.Columns["TenDanhMuc"] != null) dgvThongKe.Columns["TenDanhMuc"].HeaderText = "Danh Mục";
            if (dgvThongKe.Columns["TongDaBan"] != null) dgvThongKe.Columns["TongDaBan"].HeaderText = "Đã Bán";
            if (dgvThongKe.Columns["TongDoanhThu"] != null) dgvThongKe.Columns["TongDoanhThu"].HeaderText = "Tổng Doanh Thu";

            UpdatePaginationFooter(totalCount, totalPages, "sản phẩm bán chạy");
        }

        private void UpdatePaginationFooter(int totalCount, int totalPages, string unitName)
        {
            int startIdx = totalCount == 0 ? 0 : (_currentPage - 1) * PageSize + 1;
            int endIdx = Math.Min(_currentPage * PageSize, totalCount);
            lblPageInfo.Text = $"Hiển thị {startIdx} - {endIdx} / Tổng {totalCount} {unitName} (Trang {_currentPage}/{totalPages})";

            btnPrev.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < totalPages;

            btnPrev.BackColor = btnPrev.Enabled ? Color.White : Color.FromArgb(241, 245, 249);
            btnPrev.ForeColor = btnPrev.Enabled ? Color.FromArgb(30, 41, 59) : Color.FromArgb(148, 163, 184);
            btnPrev.FlatAppearance.BorderColor = btnPrev.Enabled ? Color.FromArgb(203, 213, 225) : Color.FromArgb(226, 232, 240);
            btnPrev.FlatAppearance.BorderSize = 1;

            btnNext.BackColor = btnNext.Enabled ? Color.White : Color.FromArgb(241, 245, 249);
            btnNext.ForeColor = btnNext.Enabled ? Color.FromArgb(30, 41, 59) : Color.FromArgb(148, 163, 184);
            btnNext.FlatAppearance.BorderColor = btnNext.Enabled ? Color.FromArgb(203, 213, 225) : Color.FromArgb(226, 232, 240);
            btnNext.FlatAppearance.BorderSize = 1;

            pnlPageNumbers.Controls.Clear();
            for (int i = 1; i <= totalPages; i++)
            {
                int pageNum = i;
                var btnNum = new Button
                {
                    Text = pageNum.ToString(),
                    Size = new Size(36, 32),
                    Margin = new Padding(3, 0, 3, 0),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };

                if (pageNum == _currentPage)
                {
                    btnNum.BackColor = UIStyleHelper.PrimaryBlue;
                    btnNum.ForeColor = Color.White;
                    btnNum.FlatAppearance.BorderColor = UIStyleHelper.PrimaryBlue;
                    btnNum.FlatAppearance.BorderSize = 1;
                }
                else
                {
                    btnNum.BackColor = Color.White;
                    btnNum.ForeColor = Color.FromArgb(30, 41, 59);
                    btnNum.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
                    btnNum.FlatAppearance.BorderSize = 1;
                }

                btnNum.Click += (s, e) =>
                {
                    _currentPage = pageNum;
                    RenderPage();
                };
                pnlPageNumbers.Controls.Add(btnNum);
            }

            RepositionPaginationControls();
        }

        private void DgvThongKe_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (_loaiBaoCaoMode == "DoanhThu")
            {
                if (dgvThongKe.Columns[e.ColumnIndex].Name == "LoiNhuan" && e.Value != null)
                {
                    string strVal = e.Value.ToString() ?? "";
                    e.CellStyle.ForeColor = strVal.StartsWith("-")
                        ? Color.FromArgb(225, 29, 72)
                        : Color.FromArgb(5, 150, 105);
                    e.CellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                }
            }
            else if (_loaiBaoCaoMode == "TonKho")
            {
                if (dgvThongKe.Columns[e.ColumnIndex].Name == "CanNhapThem" && e.Value != null)
                {
                    if (int.TryParse(e.Value.ToString(), out int val) && val > 0)
                    {
                        e.CellStyle.ForeColor = Color.FromArgb(225, 29, 72);
                        e.CellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                    }
                }
            }
            else if (_loaiBaoCaoMode == "BanChay")
            {
                if (dgvThongKe.Columns[e.ColumnIndex].Name == "Top" && e.Value != null)
                {
                    string topStr = e.Value.ToString() ?? "";
                    if (topStr == "Top 1") e.CellStyle.ForeColor = Color.FromArgb(217, 119, 6);
                    else if (topStr == "Top 2") e.CellStyle.ForeColor = Color.FromArgb(37, 99, 235);
                    else if (topStr == "Top 3") e.CellStyle.ForeColor = Color.FromArgb(124, 58, 237);
                    e.CellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                }
            }
        }

        private void XuatBaoCaoCsv()
        {
            using var sfd = new SaveFileDialog
            {
                Filter = "File CSV (*.csv)|*.csv",
                FileName = $"BaoCao_{_loaiBaoCaoMode}_{DateTime.Now:yyyyMMdd}.csv"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sb = new System.Text.StringBuilder();
                    // Append headers
                    var colHeaders = new List<string>();
                    foreach (DataGridViewColumn col in dgvThongKe.Columns) colHeaders.Add(col.HeaderText);
                    sb.AppendLine(string.Join(",", colHeaders));

                    foreach (DataGridViewRow row in dgvThongKe.Rows)
                    {
                        if (row.IsNewRow) continue;
                        var rowVals = new List<string>();
                        foreach (DataGridViewColumn col in dgvThongKe.Columns)
                        {
                            string val = row.Cells[col.Index].Value?.ToString()?.Replace(",", "") ?? "";
                            rowVals.Add($"\"{val}\"");
                        }
                        sb.AppendLine(string.Join(",", rowVals));
                    }
                    System.IO.File.WriteAllText(sfd.FileName, sb.ToString(), System.Text.Encoding.UTF8);
                    MessageBox.Show("Đã xuất báo cáo thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi xuất báo cáo:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
