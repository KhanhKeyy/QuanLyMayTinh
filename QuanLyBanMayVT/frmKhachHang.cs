using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    public partial class frmKhachHang : Form
    {
        private TabControl tabControl = null!;
        private TabPage tabSanPham = null!;
        private TabPage tabDonHang = null!;

        // Controls Tab SanPham
        private DataGridView dgvSanPham = null!;
        private TextBox txtTimKiem = null!;
        private ComboBox cboDanhMuc = null!;
        private Button btnDatHang = null!;

        // Controls Tab DonHang
        private DataGridView dgvDonHang = null!;
        private DataGridView dgvChiTietDH = null!;

        private KhachHang? _khachHangCurrent;

        public frmKhachHang()
        {
            InitializeComponent();
        }

        private void frmKhachHang_Load(object sender, EventArgs e)
        {
            this.Text = $"Cửa Hàng Máy Vi Tính  |  {UserSession.DisplayName}";
            lblChaoMung.Text = $"Chào mừng, {UserSession.DisplayName}! 👋";

            panelTop.Resize += PanelTop_Resize;
            PanelTop_Resize(panelTop, EventArgs.Empty);

            _khachHangCurrent = UserSession.CurrentKhachHang;
            if (_khachHangCurrent == null && UserSession.CurrentAccount != null)
            {
                _khachHangCurrent = new KhachHangDAO().GetByMaTaiKhoan(UserSession.CurrentAccount.MaTaiKhoan);
            }

            InitTabs();
            TaiDanhMuc();
            TaiDanhSachSanPham();
        }

        private void PanelTop_Resize(object? sender, EventArgs e)
        {
            btnDangXuat.Left = panelTop.ClientSize.Width - btnDangXuat.Width - 20;
        }

        private void InitTabs()
        {
            panelMain.Controls.Clear();

            tabControl = new TabControl { Dock = DockStyle.Fill };

            // TAB 1: Danh sách sản phẩm
            tabSanPham = new TabPage("🛍️ Danh mục & Đặt hàng");
            tabSanPham.BackColor = UIStyleHelper.BgMain;

            var pnlHeaderSP = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(12, 12, 12, 12),
                BackColor = UIStyleHelper.BgCard,
                WrapContents = false,
                AutoScroll = true
            };

            var lblTK = new Label
            {
                Text = "🔍 Tìm kiếm:",
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Margin = new Padding(0, 6, 8, 0)
            };
            txtTimKiem = new TextBox { Width = 220, Margin = new Padding(0, 2, 20, 0) };
            UIStyleHelper.StyleTextBox(txtTimKiem);
            txtTimKiem.TextChanged += (s, e) => TaiDanhSachSanPham();

            var lblDM = new Label
            {
                Text = "Danh mục:",
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Margin = new Padding(0, 6, 8, 0)
            };
            cboDanhMuc = new ComboBox { Width = 220, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(0, 2, 20, 0) };
            UIStyleHelper.StyleComboBox(cboDanhMuc);
            cboDanhMuc.SelectedIndexChanged += (s, e) => TaiDanhSachSanPham();

            btnDatHang = new Button
            {
                Text = "🛒 Đặt mua sản phẩm đã chọn",
                AutoSize = true,
                Padding = new Padding(15, 6, 15, 6),
                Margin = new Padding(0, 0, 0, 0),
                BackColor = UIStyleHelper.PrimaryBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDatHang.FlatAppearance.BorderSize = 0;
            btnDatHang.Click += BtnDatHang_Click;

            pnlHeaderSP.Controls.Add(lblTK);
            pnlHeaderSP.Controls.Add(txtTimKiem);
            pnlHeaderSP.Controls.Add(lblDM);
            pnlHeaderSP.Controls.Add(cboDanhMuc);
            pnlHeaderSP.Controls.Add(btnDatHang);

            dgvSanPham = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvSanPham);

            tabSanPham.Controls.Add(dgvSanPham);
            tabSanPham.Controls.Add(pnlHeaderSP);

            // TAB 2: Đơn hàng của tôi
            tabDonHang = new TabPage("📋 Đơn hàng của tôi");
            tabDonHang.BackColor = UIStyleHelper.BgMain;

            var splitDonHang = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 280
            };

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
            dgvDonHang.SelectionChanged += DgvDonHang_SelectionChanged;

            dgvChiTietDH = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvChiTietDH);

            splitDonHang.Panel1.Controls.Add(dgvDonHang);
            splitDonHang.Panel2.Controls.Add(dgvChiTietDH);
            tabDonHang.Controls.Add(splitDonHang);

            tabControl.TabPages.Add(tabSanPham);
            tabControl.TabPages.Add(tabDonHang);
            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;

            panelMain.Controls.Add(tabControl);
        }

        private void TaiDanhMuc()
        {
            var listDM = new DanhMucSanPhamDAO().GetAll();
            listDM.Insert(0, new DanhMucSanPham { MaDanhMuc = 0, TenDanhMuc = "-- Tất cả danh mục --" });
            cboDanhMuc.DataSource = listDM;
            cboDanhMuc.DisplayMember = "TenDanhMuc";
            cboDanhMuc.ValueMember = "MaDanhMuc";
        }

        private void TaiDanhSachSanPham()
        {
            string kw = txtTimKiem.Text.Trim();
            int maDM = cboDanhMuc.SelectedValue is int id ? id : 0;
            var list = new SanPhamDAO().GetAll(kw, maDM, chiConHang: true);

            dgvSanPham.DataSource = list.Select(p => new
            {
                p.MaSanPham,
                p.TenSanPham,
                p.TenDanhMuc,
                p.CauHinh,
                GiaBan = p.GiaBanFormatted,
                p.SoLuongTon,
                TrangThai = p.TrangThaiDisplay
            }).ToList();

            if (dgvSanPham.Columns["MaSanPham"] != null)
                dgvSanPham.Columns["MaSanPham"].HeaderText = "Mã SP";
            if (dgvSanPham.Columns["TenSanPham"] != null)
                dgvSanPham.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";
            if (dgvSanPham.Columns["TenDanhMuc"] != null)
                dgvSanPham.Columns["TenDanhMuc"].HeaderText = "Danh Mục";
            if (dgvSanPham.Columns["CauHinh"] != null)
                dgvSanPham.Columns["CauHinh"].HeaderText = "Cấu Hình";
            if (dgvSanPham.Columns["GiaBan"] != null)
                dgvSanPham.Columns["GiaBan"].HeaderText = "Giá Bán";
            if (dgvSanPham.Columns["SoLuongTon"] != null)
                dgvSanPham.Columns["SoLuongTon"].HeaderText = "Số Lượng";
            if (dgvSanPham.Columns["TrangThai"] != null)
                dgvSanPham.Columns["TrangThai"].HeaderText = "Trạng Thái";
        }

        private void BtnDatHang_Click(object? sender, EventArgs e)
        {
            if (dgvSanPham.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần đặt hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int maSP = (int)dgvSanPham.CurrentRow.Cells["MaSanPham"].Value;
            var sanPham = new SanPhamDAO().GetById(maSP);

            if (sanPham == null || sanPham.SoLuongTon <= 0)
            {
                MessageBox.Show("Sản phẩm đã hết hàng hoặc không khả dụng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_khachHangCurrent == null)
            {
                MessageBox.Show("Không tìm thấy thông tin hồ sơ khách hàng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using var dlg = new frmDatHang(sanPham, _khachHangCurrent);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                TaiDanhSachSanPham();
                TaiLichSuDonHang();
            }
        }

        private void TabControl_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabDonHang)
            {
                TaiLichSuDonHang();
            }
        }

        private void TaiLichSuDonHang()
        {
            if (_khachHangCurrent == null) return;
            var listDH = new DonHangDAO().GetByMaKhachHang(_khachHangCurrent.MaKhachHang);

            dgvDonHang.DataSource = listDH.Select(d => new
            {
                d.MaDonHang,
                d.NgayDatHang,
                d.TenPhuongThuc,
                TongTien = d.TongTien.ToString("N0") + " đ",
                TrangThai = d.TrangThaiDisplay,
                d.GhiChu
            }).ToList();

            if (dgvDonHang.Columns["MaDonHang"] != null) dgvDonHang.Columns["MaDonHang"].HeaderText = "Mã Đơn";
            if (dgvDonHang.Columns["NgayDatHang"] != null) dgvDonHang.Columns["NgayDatHang"].HeaderText = "Ngày Đặt";
            if (dgvDonHang.Columns["TenPhuongThuc"] != null) dgvDonHang.Columns["TenPhuongThuc"].HeaderText = "Phương Thức TT";
            if (dgvDonHang.Columns["TongTien"] != null) dgvDonHang.Columns["TongTien"].HeaderText = "Tổng Tiền";
            if (dgvDonHang.Columns["TrangThai"] != null) dgvDonHang.Columns["TrangThai"].HeaderText = "Trạng Thái";
            if (dgvDonHang.Columns["GhiChu"] != null) dgvDonHang.Columns["GhiChu"].HeaderText = "Ghi Chú";
        }

        private void DgvDonHang_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvDonHang.CurrentRow == null) return;
            int maDH = (int)dgvDonHang.CurrentRow.Cells["MaDonHang"].Value;
            var chiTiet = new DonHangDAO().GetChiTiet(maDH);

            dgvChiTietDH.DataSource = chiTiet.Select(c => new
            {
                c.TenSanPham,
                SoLuong = c.SoLuong,
                DonGia = c.DonGiaFormatted,
                ThanhTien = c.ThanhTienFormatted
            }).ToList();

            if (dgvChiTietDH.Columns["TenSanPham"] != null) dgvChiTietDH.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";
            if (dgvChiTietDH.Columns["SoLuong"] != null) dgvChiTietDH.Columns["SoLuong"].HeaderText = "Số Lượng";
            if (dgvChiTietDH.Columns["DonGia"] != null) dgvChiTietDH.Columns["DonGia"].HeaderText = "Đơn Giá";
            if (dgvChiTietDH.Columns["ThanhTien"] != null) dgvChiTietDH.Columns["ThanhTien"].HeaderText = "Thành Tiền";
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc muốn đăng xuất?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                this.Close();
        }
    }
}
