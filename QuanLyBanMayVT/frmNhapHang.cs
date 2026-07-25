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

            var pnlTopLP = new Panel { Dock = DockStyle.Top, Height = 105, Padding = new Padding(12), BackColor = UIStyleHelper.BgCard };

            pnlTopLP.Controls.Add(new Label { Text = "Sản phẩm:", Location = new Point(12, 16), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            cboSanPham = new ComboBox { Location = new Point(110, 12), Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
            UIStyleHelper.StyleComboBox(cboSanPham);
            cboSanPham.SelectedIndexChanged += CboSanPham_SelectedIndexChanged;
            pnlTopLP.Controls.Add(cboSanPham);

            pnlTopLP.Controls.Add(new Label { Text = "Số lượng nhập:", Location = new Point(395, 16), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            numSoLuong = new NumericUpDown { Location = new Point(515, 12), Width = 110, Minimum = 1, Maximum = 10000, Value = 10 };
            UIStyleHelper.StyleNumeric(numSoLuong);
            pnlTopLP.Controls.Add(numSoLuong);

            pnlTopLP.Controls.Add(new Label { Text = "Đơn giá nhập:", Location = new Point(12, 60), AutoSize = true, ForeColor = UIStyleHelper.TextMuted });
            numDonGia = new NumericUpDown { Location = new Point(110, 56), Width = 180, Maximum = 1000000000, Increment = 100000 };
            UIStyleHelper.StyleNumeric(numDonGia);
            pnlTopLP.Controls.Add(numDonGia);

            btnThemVaoPhieu = new Button
            {
                Text = "➕ Thêm vào danh sách nhập",
                Location = new Point(315, 54),
                Size = new Size(230, 35),
                BackColor = UIStyleHelper.PrimaryBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnThemVaoPhieu.FlatAppearance.BorderSize = 0;
            btnThemVaoPhieu.Click += BtnThemVaoPhieu_Click;
            pnlTopLP.Controls.Add(btnThemVaoPhieu);

            dgvChiTietTam = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvChiTietTam);

            var pnlBottomLP = new Panel { Dock = DockStyle.Bottom, Height = 55, Padding = new Padding(12), BackColor = UIStyleHelper.BgCard };
            lblTongGiaTri = new Label
            {
                Text = "Tổng giá trị phiếu nhập: 0 đ",
                Location = new Point(12, 16),
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 211, 153)
            };
            btnLapPhieu = new Button
            {
                Text = "✅ Hoàn tất lập phiếu nhập",
                Location = new Point(480, 10),
                Size = new Size(230, 35),
                BackColor = UIStyleHelper.SuccessGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLapPhieu.FlatAppearance.BorderSize = 0;
            btnLapPhieu.Click += BtnLapPhieu_Click;

            pnlBottomLP.Controls.Add(lblTongGiaTri);
            pnlBottomLP.Controls.Add(btnLapPhieu);

            tabLapPhieu.Controls.Add(dgvChiTietTam);
            tabLapPhieu.Controls.Add(pnlTopLP);
            tabLapPhieu.Controls.Add(pnlBottomLP);

            // Tab 2: Danh sách phiếu nhập
            tabDanhSach = new TabPage("📋 Danh sách phiếu nhập");
            tabDanhSach.BackColor = UIStyleHelper.BgMain;

            var pnlTopDS = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12), BackColor = UIStyleHelper.BgCard };
            btnDuyetPhieu = new Button
            {
                Text = "✅ DUYỆT PHIẾU (Nhập kho)",
                Location = new Point(12, 10),
                Size = new Size(230, 35),
                BackColor = UIStyleHelper.SuccessGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDuyetPhieu.FlatAppearance.BorderSize = 0;
            btnDuyetPhieu.Click += BtnDuyetPhieu_Click;
            pnlTopDS.Controls.Add(btnDuyetPhieu);

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
            tabDanhSach.Controls.Add(pnlTopDS);

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
