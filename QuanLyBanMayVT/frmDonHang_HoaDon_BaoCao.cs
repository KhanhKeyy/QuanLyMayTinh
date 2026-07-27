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
        private DataGridView dgvChiTiet = null!;
        private ComboBox cboTrangThai = null!;
        private Button btnXacNhan = null!;
        private Button btnHuy = null!;

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
                ForeColor = Color.White,
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

            tblTop.Controls.Add(lblTT);
            tblTop.Controls.Add(cboTrangThai);

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

            var split = new SplitContainer
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

            dgvChiTiet = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvChiTiet);

            split.Panel1.Controls.Add(dgvDonHang);
            split.Panel2.Controls.Add(dgvChiTiet);

            this.Controls.Add(split);
            this.Controls.Add(tblTop);
        }

        private void LoadData()
        {
            string? filterTT = cboTrangThai.SelectedIndex > 0 ? cboTrangThai.SelectedItem?.ToString() : null;
            var list = new DonHangDAO().GetAll(filterTT);

            dgvDonHang.DataSource = list.Select(d => new
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
        }

        private void DgvDonHang_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvDonHang.CurrentRow == null) return;
            int maDH = (int)dgvDonHang.CurrentRow.Cells["MaDonHang"].Value;
            var chiTiet = new DonHangDAO().GetChiTiet(maDH);

            dgvChiTiet.DataSource = chiTiet.Select(c => new
            {
                c.TenSanPham,
                c.SoLuong,
                DonGia = c.DonGiaFormatted,
                ThanhTien = c.ThanhTienFormatted
            }).ToList();

            if (dgvChiTiet.Columns["TenSanPham"] != null) dgvChiTiet.Columns["TenSanPham"].HeaderText = "Sản Phẩm";
            if (dgvChiTiet.Columns["SoLuong"] != null) dgvChiTiet.Columns["SoLuong"].HeaderText = "Số Lượng";
            if (dgvChiTiet.Columns["DonGia"] != null) dgvChiTiet.Columns["DonGia"].HeaderText = "Đơn Giá";
            if (dgvChiTiet.Columns["ThanhTien"] != null) dgvChiTiet.Columns["ThanhTien"].HeaderText = "Thành Tiền";
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

            tabControl = new TabControl { Dock = DockStyle.Fill };

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
    public class frmBaoCao : Form
    {
        private readonly string _loaiBaoCao;
        private TabControl tabControl = null!;

        private DataGridView dgvDoanhThu = null!;
        private DataGridView dgvTonKho = null!;
        private DataGridView dgvBanChay = null!;
        private Label lblTongDoanhThu = null!;

        public frmBaoCao(string loaiBaoCao = "DoanhThu")
        {
            _loaiBaoCao = loaiBaoCao;
            InitUI();
            LoadBaoCao();
        }

        private void InitUI()
        {
            this.Text = "Báo cáo thống kê";
            this.BackColor = UIStyleHelper.BgMain;
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F);

            tabControl = new TabControl { Dock = DockStyle.Fill };

            var tabDT = new TabPage("📈 Doanh thu theo tháng");
            tabDT.BackColor = UIStyleHelper.BgMain;

            lblTongDoanhThu = new Label
            {
                Text = "Tổng doanh thu: 0 đ",
                Dock = DockStyle.Top,
                Height = 45,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 211, 153),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0),
                BackColor = UIStyleHelper.BgCard
            };

            dgvDoanhThu = CreateDgv();
            tabDT.Controls.Add(dgvDoanhThu);
            tabDT.Controls.Add(lblTongDoanhThu);

            var tabTK = new TabPage("📊 Cảnh báo tồn kho");
            tabTK.BackColor = UIStyleHelper.BgMain;
            dgvTonKho = CreateDgv();
            tabTK.Controls.Add(dgvTonKho);

            var tabBC = new TabPage("🔥 Sản phẩm bán chạy");
            tabBC.BackColor = UIStyleHelper.BgMain;
            dgvBanChay = CreateDgv();
            tabBC.Controls.Add(dgvBanChay);

            tabControl.TabPages.Add(tabDT);
            tabControl.TabPages.Add(tabTK);
            tabControl.TabPages.Add(tabBC);

            if (_loaiBaoCao == "TonKho") tabControl.SelectedTab = tabTK;
            else if (_loaiBaoCao == "BanChay") tabControl.SelectedTab = tabBC;

            this.Controls.Add(tabControl);
        }

        private DataGridView CreateDgv()
        {
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgv);
            return dgv;
        }

        private void LoadBaoCao()
        {
            LoadDoanhThu();
            LoadTonKho();
            LoadBanChay();
        }

        private void LoadDoanhThu()
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    SELECT
                        YEAR(NgayThanhToan) AS Nam,
                        MONTH(NgayThanhToan) AS Thang,
                        COUNT(1) AS SoHoaDon,
                        SUM(TongTien) AS DoanhThu
                    FROM HoaDon
                    WHERE TrangThaiThanhToan = 'Da thanh toan'
                    GROUP BY YEAR(NgayThanhToan), MONTH(NgayThanhToan)
                    ORDER BY Nam DESC, Thang DESC";
                using var cmd = new SqlCommand(sql, conn);
                using var da = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);

                dgvDoanhThu.DataSource = dt;

                decimal tong = 0;
                foreach (DataRow r in dt.Rows)
                {
                    if (r["DoanhThu"] != DBNull.Value)
                        tong += Convert.ToDecimal(r["DoanhThu"]);
                }
                lblTongDoanhThu.Text = $"💰 TỔNG DOANH THU TOÀN BỘ: {tong:N0} đ";
            }
            catch { }
        }

        private void LoadTonKho()
        {
            var listCanNhap = new SanPhamDAO().GetCanNhap();
            dgvTonKho.DataSource = listCanNhap.Select(p => new
            {
                p.MaSanPham,
                p.TenSanPham,
                p.TenDanhMuc,
                p.SoLuongTon,
                p.MucTonToiThieu,
                CanNhapThem = p.MucTonToiThieu - p.SoLuongTon,
                p.TrangThaiDisplay
            }).ToList();

            if (dgvTonKho.Columns["MaSanPham"] != null) dgvTonKho.Columns["MaSanPham"].HeaderText = "Mã SP";
            if (dgvTonKho.Columns["TenSanPham"] != null) dgvTonKho.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";
            if (dgvTonKho.Columns["TenDanhMuc"] != null) dgvTonKho.Columns["TenDanhMuc"].HeaderText = "Danh Mục";
            if (dgvTonKho.Columns["SoLuongTon"] != null) dgvTonKho.Columns["SoLuongTon"].HeaderText = "Tồn Hiện Tại";
            if (dgvTonKho.Columns["MucTonToiThieu"] != null) dgvTonKho.Columns["MucTonToiThieu"].HeaderText = "Mức Tối Thiểu";
            if (dgvTonKho.Columns["CanNhapThem"] != null) dgvTonKho.Columns["CanNhapThem"].HeaderText = "Cần Nhập Ít Nhất";
            if (dgvTonKho.Columns["TrangThaiDisplay"] != null) dgvTonKho.Columns["TrangThaiDisplay"].HeaderText = "Trạng Thái";
        }

        private void LoadBanChay()
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    SELECT TOP 10
                        sp.MaSanPham,
                        sp.TenSanPham,
                        dm.TenDanhMuc,
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

                dgvBanChay.DataSource = dt;
            }
            catch { }
        }
    }
}
