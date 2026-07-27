using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    public class frmNhapHang : Form
    {
        private TabControl tabControl = null!;
        private TabPage tabLapPhieu = null!;
        private TabPage tabDanhSach = null!;

        // Controls Tab Lap Phieu
        private ComboBox cboSanPham = null!;
        private NumericUpDown numSoLuong = null!;
        private NumericUpDown numDonGia = null!;
        private Button btnThemVaoPhieu = null!;
        private DataGridView dgvChiTietTam = null!;
        private Button btnLapPhieu = null!;
        private Label lblTongGiaTri = null!;

        private readonly List<ChiTietPhieuNhap> _dsChiTietTam = new();

        // Controls Tab Danh Sach
        private DataGridView dgvPhieuNhap = null!;
        private DataGridView dgvChiTietPN = null!;
        private Button btnDuyetPhieu = null!;
        private ComboBox _cboFilterPhieu = null!;

        public frmNhapHang()
        {
            InitUI();
            LoadSanPhamCombo();
            LoadPhieuNhap();
        }

        private void InitUI()
        {
            this.Text = "Quản lý nhập hàng";
            this.BackColor = UIStyleHelper.BgMain;
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F);

            tabControl = new TabControl { Dock = DockStyle.Fill };

            // Tab 1: Lập phiếu nhập
            tabLapPhieu = new TabPage("🚚 Lập phiếu nhập hàng");
            tabLapPhieu.BackColor = UIStyleHelper.BgMain;

            // ── Top: chọn sản phẩm + số lượng + đơn giá ──────────────
            var tblTopLP = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(12, 10, 12, 10),
                BackColor = UIStyleHelper.BgCard,
                ColumnCount = 8,
                RowCount = 2,
            };
            tblTopLP.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblTopLP.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            for (int i = 0; i < 8; i++)
                tblTopLP.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            // Row 0: Sản phẩm + Số lượng
            var lblSP = new Label { Text = "Sản phẩm:", AutoSize = true, ForeColor = UIStyleHelper.TextMuted, Margin = new Padding(0, 6, 8, 0) };
            cboSanPham = new ComboBox { Width = 260, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(0, 3, 20, 0) };
            UIStyleHelper.StyleComboBox(cboSanPham);
            cboSanPham.SelectedIndexChanged += CboSanPham_SelectedIndexChanged;

            var lblSL = new Label { Text = "Số lượng nhập:", AutoSize = true, ForeColor = UIStyleHelper.TextMuted, Margin = new Padding(0, 6, 8, 0) };
            numSoLuong = new NumericUpDown { Width = 110, Minimum = 1, Maximum = 10000, Value = 10, Margin = new Padding(0, 3, 0, 0) };
            UIStyleHelper.StyleNumeric(numSoLuong);

            tblTopLP.Controls.Add(lblSP,     0, 0);
            tblTopLP.Controls.Add(cboSanPham, 1, 0);
            tblTopLP.Controls.Add(lblSL,      2, 0);
            tblTopLP.Controls.Add(numSoLuong, 3, 0);

            // Row 1: Đơn giá + nút Thêm
            var lblDG = new Label { Text = "Đơn giá nhập:", AutoSize = true, ForeColor = UIStyleHelper.TextMuted, Margin = new Padding(0, 6, 8, 4) };
            numDonGia = new NumericUpDown { Width = 180, Maximum = 1_000_000_000, Increment = 100000, Margin = new Padding(0, 3, 20, 4) };
            UIStyleHelper.StyleNumeric(numDonGia);

            btnThemVaoPhieu = new Button
            {
                Text = "➕ Thêm vào danh sách nhập",
                AutoSize = true,
                Padding = new Padding(14, 6, 14, 6),
                Margin = new Padding(0, 0, 0, 4),
                BackColor = UIStyleHelper.PrimaryBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnThemVaoPhieu.FlatAppearance.BorderSize = 0;
            btnThemVaoPhieu.Click += BtnThemVaoPhieu_Click;

            tblTopLP.Controls.Add(lblDG,          0, 1);
            tblTopLP.Controls.Add(numDonGia,       1, 1);
            tblTopLP.Controls.Add(btnThemVaoPhieu, 2, 1);
            tblTopLP.SetColumnSpan(btnThemVaoPhieu, 2);

            // ── Grid tạm + Bottom ────────────────────────────────────
            dgvChiTietTam = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvChiTietTam);

            var tblBottom = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(12, 10, 12, 10),
                BackColor = UIStyleHelper.BgCard,
                ColumnCount = 3,
                RowCount = 1,
            };
            tblBottom.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblBottom.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tblBottom.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            lblTongGiaTri = new Label
            {
                Text = "Tổng giá trị phiếu nhập: 0 đ",
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 211, 153),
                Margin = new Padding(0, 6, 0, 0),
                Dock = DockStyle.Fill
            };
            btnLapPhieu = new Button
            {
                Text = "✅ Hoàn tất lập phiếu nhập",
                AutoSize = true,
                Padding = new Padding(14, 6, 14, 6),
                Margin = new Padding(0, 0, 0, 0),
                BackColor = UIStyleHelper.SuccessGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLapPhieu.FlatAppearance.BorderSize = 0;
            btnLapPhieu.Click += BtnLapPhieu_Click;

            tblBottom.Controls.Add(lblTongGiaTri, 0, 0);
            tblBottom.Controls.Add(btnLapPhieu,   2, 0);

            tabLapPhieu.Controls.Add(dgvChiTietTam);
            tabLapPhieu.Controls.Add(tblTopLP);
            tabLapPhieu.Controls.Add(tblBottom);

            // Tab 2: Danh sách phiếu nhập
            tabDanhSach = new TabPage("📋 Danh sách phiếu nhập");
            tabDanhSach.BackColor = UIStyleHelper.BgMain;

            var tblTopDS = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(12, 10, 12, 10),
                BackColor = UIStyleHelper.BgCard,
                ColumnCount = 10,
                RowCount = 1,
            };
            tblTopDS.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            for (int i = 0; i < 10; i++)
                tblTopDS.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            // ── Filter trạng thái phiếu nhập ────────────────────────
            var lblFilter = new Label
            {
                Text = "Lọc trạng thái:",
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Margin = new Padding(0, 6, 8, 0)
            };
            _cboFilterPhieu = new ComboBox
            {
                Width = 170,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 3, 20, 0)
            };
            UIStyleHelper.StyleComboBox(_cboFilterPhieu);
            _cboFilterPhieu.Items.AddRange(new object[] { "-- Tất cả --", "⏳ Chờ kiểm tra", "✅ Đã nhập kho" });
            _cboFilterPhieu.SelectedIndex = 0;
            _cboFilterPhieu.SelectedIndexChanged += (s, e) => LoadPhieuNhap();

            btnDuyetPhieu = new Button
            {
                Text = "✅ Duyệt phiếu (Nhập kho)",
                AutoSize = true,
                Padding = new Padding(14, 6, 14, 6),
                Margin = new Padding(0, 0, 0, 0),
                BackColor = UIStyleHelper.SuccessGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDuyetPhieu.FlatAppearance.BorderSize = 0;
            btnDuyetPhieu.Click += BtnDuyetPhieu_Click;

            tblTopDS.Controls.Add(lblFilter);
            tblTopDS.Controls.Add(_cboFilterPhieu);
            tblTopDS.Controls.Add(btnDuyetPhieu);

            var splitDS = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 240
            };

            dgvPhieuNhap = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvPhieuNhap);
            dgvPhieuNhap.SelectionChanged += DgvPhieuNhap_SelectionChanged;

            dgvChiTietPN = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvChiTietPN);

            splitDS.Panel1.Controls.Add(dgvPhieuNhap);
            splitDS.Panel2.Controls.Add(dgvChiTietPN);

            tabDanhSach.Controls.Add(splitDS);
            tabDanhSach.Controls.Add(tblTopDS);

            tabControl.TabPages.Add(tabLapPhieu);
            tabControl.TabPages.Add(tabDanhSach);
            tabControl.SelectedIndexChanged += (s, e) =>
            {
                if (tabControl.SelectedTab == tabDanhSach) LoadPhieuNhap();
            };

            this.Controls.Add(tabControl);
        }

        private void LoadSanPhamCombo()
        {
            var list = new SanPhamDAO().GetAll();
            cboSanPham.DataSource = list;
            cboSanPham.DisplayMember = "TenSanPham";
            cboSanPham.ValueMember = "MaSanPham";
        }

        private void CboSanPham_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cboSanPham.SelectedItem is SanPham sp)
            {
                numDonGia.Value = Math.Round(sp.GiaBan * 0.8m);
            }
        }

        private void BtnThemVaoPhieu_Click(object? sender, EventArgs e)
        {
            if (cboSanPham.SelectedItem is not SanPham sp) return;

            int sl = (int)numSoLuong.Value;
            decimal dg = numDonGia.Value;

            var item = _dsChiTietTam.FirstOrDefault(x => x.MaSanPham == sp.MaSanPham);
            if (item != null)
            {
                item.SoLuongNhap += sl;
                item.DonGiaNhap = dg;
            }
            else
            {
                _dsChiTietTam.Add(new ChiTietPhieuNhap
                {
                    MaSanPham = sp.MaSanPham,
                    TenSanPham = sp.TenSanPham,
                    SoLuongNhap = sl,
                    DonGiaNhap = dg
                });
            }

            CapNhatGridTam();
        }

        private void CapNhatGridTam()
        {
            dgvChiTietTam.DataSource = null;
            dgvChiTietTam.DataSource = _dsChiTietTam.Select(x => new
            {
                x.TenSanPham,
                x.SoLuongNhap,
                DonGiaNhap = x.DonGiaNhapFormatted,
                ThanhTien = x.ThanhTienFormatted
            }).ToList();

            if (dgvChiTietTam.Columns["TenSanPham"] != null) dgvChiTietTam.Columns["TenSanPham"].HeaderText = "Sản Phẩm";
            if (dgvChiTietTam.Columns["SoLuongNhap"] != null) dgvChiTietTam.Columns["SoLuongNhap"].HeaderText = "Số Lượng Nhập";
            if (dgvChiTietTam.Columns["DonGiaNhap"] != null) dgvChiTietTam.Columns["DonGiaNhap"].HeaderText = "Đơn Giá Nhập";
            if (dgvChiTietTam.Columns["ThanhTien"] != null) dgvChiTietTam.Columns["ThanhTien"].HeaderText = "Thành Tiền";

            decimal tong = _dsChiTietTam.Sum(x => x.ThanhTien);
            lblTongGiaTri.Text = $"Tổng giá trị phiếu nhập: {tong:N0} đ";
        }

        private void BtnLapPhieu_Click(object? sender, EventArgs e)
        {
            if (_dsChiTietTam.Count == 0)
            {
                MessageBox.Show("Chưa có sản phẩm nào trong danh sách nhập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var currentNV = UserSession.CurrentNhanVien;
            int maQL = currentNV != null ? currentNV.MaNhanVien : 1;

            var phieu = new PhieuNhapHang { MaQuanLy = maQL };
            int maPN = new PhieuNhapHangDAO().Insert(phieu, _dsChiTietTam);
            if (maPN > 0)
            {
                MessageBox.Show($"Lập phiếu nhập #{maPN} thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _dsChiTietTam.Clear();
                CapNhatGridTam();
                tabControl.SelectedTab = tabDanhSach;
                LoadPhieuNhap();
            }
        }

        private void LoadPhieuNhap()
        {
            var list = new PhieuNhapHangDAO().GetAll();

            // Lọc theo trạng thái nếu có chọn
            string filterText = _cboFilterPhieu?.SelectedItem?.ToString() ?? "-- Tất cả --";
            if (filterText == "⏳ Chờ kiểm tra")
                list = list.Where(p => p.TrangThai == "Cho kiem tra").ToList();
            else if (filterText == "✅ Đã nhập kho")
                list = list.Where(p => p.TrangThai == "Da nhap kho").ToList();

            dgvPhieuNhap.DataSource = list.Select(p => new
            {
                p.MaPhieuNhap,
                p.TenQuanLy,
                p.NgayNhap,
                TrangThai = p.TrangThaiDisplay
            }).ToList();

            if (dgvPhieuNhap.Columns["MaPhieuNhap"] != null) dgvPhieuNhap.Columns["MaPhieuNhap"].HeaderText = "Mã Phiếu";
            if (dgvPhieuNhap.Columns["TenQuanLy"] != null) dgvPhieuNhap.Columns["TenQuanLy"].HeaderText = "Người Lập (Quản Lý)";
            if (dgvPhieuNhap.Columns["NgayNhap"] != null) dgvPhieuNhap.Columns["NgayNhap"].HeaderText = "Ngày Nhập";
            if (dgvPhieuNhap.Columns["TrangThai"] != null) dgvPhieuNhap.Columns["TrangThai"].HeaderText = "Trạng Thái";
        }

        private void DgvPhieuNhap_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvPhieuNhap.CurrentRow == null) return;
            int maPN = (int)dgvPhieuNhap.CurrentRow.Cells["MaPhieuNhap"].Value;
            var chiTiet = new PhieuNhapHangDAO().GetChiTiet(maPN);

            dgvChiTietPN.DataSource = chiTiet.Select(c => new
            {
                c.TenSanPham,
                c.SoLuongNhap,
                DonGiaNhap = c.DonGiaNhapFormatted,
                ThanhTien = c.ThanhTienFormatted
            }).ToList();

            if (dgvChiTietPN.Columns["TenSanPham"] != null) dgvChiTietPN.Columns["TenSanPham"].HeaderText = "Sản Phẩm";
            if (dgvChiTietPN.Columns["SoLuongNhap"] != null) dgvChiTietPN.Columns["SoLuongNhap"].HeaderText = "Số Lượng";
            if (dgvChiTietPN.Columns["DonGiaNhap"] != null) dgvChiTietPN.Columns["DonGiaNhap"].HeaderText = "Đơn Giá Nhập";
            if (dgvChiTietPN.Columns["ThanhTien"] != null) dgvChiTietPN.Columns["ThanhTien"].HeaderText = "Thành Tiền";
        }

        private void BtnDuyetPhieu_Click(object? sender, EventArgs e)
        {
            if (dgvPhieuNhap.CurrentRow == null) return;
            int maPN = (int)dgvPhieuNhap.CurrentRow.Cells["MaPhieuNhap"].Value;

            var res = MessageBox.Show($"Xác nhận duyệt phiếu nhập #{maPN} và cộng số lượng vào tồn kho?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                if (new PhieuNhapHangDAO().DuyetPhieu(maPN))
                {
                    MessageBox.Show($"Đã duyệt phiếu nhập #{maPN} thành công! Số lượng tồn kho đã được tăng.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadPhieuNhap();
                }
            }
        }
    }
}
