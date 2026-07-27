using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    public partial class frmKhachHang : Form
    {
        // ── Tabs ────────────────────────────────────────────────────
        private TabControl tabControl = null!;
        private TabPage tabSanPham = null!;
        private TabPage tabDonHang = null!;
        private TabPage tabBuild = null!;

        // ── Tab Cửa Hàng ────────────────────────────────────────────
        private DataGridView dgvSanPham = null!;
        private TextBox txtTimKiem = null!;
        private Panel? _selectedCard = null;
        private string _categoryKey = "all";
        private string _subComponentKey = "all_lk";
        private FlowLayoutPanel pnlSubLinhKien = null!;
        private Button? _selectedSubBtn = null;
        private List<DanhMucSanPham> _dmList = new();

        // ── Tab Đơn Hàng ────────────────────────────────────────────
        private DataGridView dgvDonHang = null!;
        private DataGridView dgvChiTietDH = null!;

        // ── Tab Build PC ─────────────────────────────────────────────
        private ComboBox cboCPU = null!, cboRAM = null!, cboGPU = null!;
        private ComboBox cboMainboard = null!, cboSSD = null!, cboNguon = null!, cboCase = null!;
        private Label lblBuildTotal = null!;
        private FlowLayoutPanel pnlSummaryItems = null!;

        private KhachHang? _khachHangCurrent;

        // ── Card danh mục chính ở Cửa Hàng (Gọn gàng) ─────────────────
        private static readonly (string Key, string Icon, string Name, Color Accent)[] Categories =
        {
            ("all",         "🛍️",  "Tất Cả",             Color.FromArgb(30,  41,  59)),
            ("gaming",      "🎮",  "PC Gaming",           Color.FromArgb(153, 27,  27)),
            ("vanphong",    "🏢",  "Máy Văn Phòng",       Color.FromArgb(6,   95,  70)),
            ("workstation", "⚙️",  "Workstation",         Color.FromArgb(88,  28, 135)),
            ("laptop",      "💻",  "Laptop",              Color.FromArgb(14,  62,  98)),
            ("linhkien",    "🔩",  "Linh Kiện PC",        Color.FromArgb(120, 53,  15)),
            ("ngoaivi",     "🖱️",  "Màn Hình & Ngoại Vi", Color.FromArgb(6,   78,  59)),
        };

        // ── Sub-categories cho Linh Kiện PC ──────────────────────────
        private static readonly (string Key, string Name)[] SubLinhKien =
        {
            ("all_lk",    "✨ Tất Cả Linh Kiện"),
            ("cpu",       "🔲 CPU / Vi Xử Lý"),
            ("mainboard", "🔌 Bo Mạch Chủ"),
            ("ram",       "💾 Bộ Nhớ RAM"),
            ("vga",       "🎨 Card Đồ Họa (VGA)"),
            ("ssd",       "💿 Ổ Cứng (SSD/HDD)"),
            ("nguon",     "⚡ Nguồn Máy Tính"),
            ("case",      "🖥️ Vỏ Case"),
        };

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
                _khachHangCurrent = new KhachHangDAO().GetByMaTaiKhoan(UserSession.CurrentAccount.MaTaiKhoan);

            _dmList = new DanhMucSanPhamDAO().GetAll();

            InitTabs();
            TaiDanhSachSanPham();
            NapComponentsBuild();
            CapNhatBadgeThongBao();
        }

        private Button? btnThongBao;

        private void PanelTop_Resize(object? sender, EventArgs e)
        {
            if (btnDangXuat != null)
                btnDangXuat.Left = panelTop.ClientSize.Width - btnDangXuat.Width - 20;

            if (btnThongBao != null && btnDangXuat != null)
                btnThongBao.Left = btnDangXuat.Left - btnThongBao.Width - 12;
        }

        public void CapNhatBadgeThongBao()
        {
            if (UserSession.CurrentAccount == null) return;
            int chuaDoc = new ThongBaoDAO().DemChuaDoc(UserSession.CurrentAccount.MaTaiKhoan);

            if (btnThongBao == null)
            {
                btnThongBao = new Button
                {
                    Height = 36,
                    AutoSize = true,
                    Top = 12,
                    BackColor = chuaDoc > 0 ? Color.FromArgb(225, 29, 72) : Color.FromArgb(51, 65, 85),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                    Padding = new Padding(12, 2, 12, 2)
                };
                btnThongBao.FlatAppearance.BorderSize = 0;
                btnThongBao.Click += (s, e) =>
                {
                    using var dlg = new frmThongBao();
                    dlg.ShowDialog();
                    CapNhatBadgeThongBao();
                };
                panelTop.Controls.Add(btnThongBao);
            }

            btnThongBao.BackColor = chuaDoc > 0 ? Color.FromArgb(225, 29, 72) : Color.FromArgb(51, 65, 85);
            btnThongBao.Text = chuaDoc > 0 ? $"🔔 Thông báo ({chuaDoc})" : "🔔 Thông báo";
            PanelTop_Resize(panelTop, EventArgs.Empty);
        }

        // ════════════════════════════════════════════════════════════
        // INIT TABS
        // ════════════════════════════════════════════════════════════
        private void InitTabs()
        {
            panelMain.Controls.Clear();

            tabControl = new TabControl { Dock = DockStyle.Fill };
            tabControl.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            tabControl.Padding = new Point(16, 6);

            tabSanPham = new TabPage("🏪  Cửa Hàng")         { BackColor = UIStyleHelper.BgMain };
            tabDonHang = new TabPage("📋  Đơn Hàng Của Tôi") { BackColor = UIStyleHelper.BgMain };
            tabBuild   = new TabPage("🔧  Build Cấu Hình PC") { BackColor = UIStyleHelper.BgMain };

            InitTab_CuaHang(tabSanPham);
            InitTab_DonHang(tabDonHang);
            InitTab_BuildPC(tabBuild);

            tabControl.TabPages.Add(tabSanPham);
            tabControl.TabPages.Add(tabDonHang);
            tabControl.TabPages.Add(tabBuild);
            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;

            panelMain.Controls.Add(tabControl);
        }

        // ════════════════════════════════════════════════════════════
        // TAB 1: CỬA HÀNG
        // ════════════════════════════════════════════════════════════
        private void InitTab_CuaHang(TabPage tab)
        {
            // 1. Search bar + Đặt mua
            var tblSearch = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(14, 10, 14, 10),
                BackColor = UIStyleHelper.BgCard,
                ColumnCount = 3, RowCount = 1
            };
            tblSearch.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblSearch.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tblSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblSearch.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            tblSearch.Controls.Add(new Label
            {
                Text = "🔍",
                AutoSize = true,
                Font = new Font("Segoe UI Emoji", 13F),
                ForeColor = Color.FromArgb(148, 163, 184),
                Margin = new Padding(0, 3, 8, 0)
            }, 0, 0);

            txtTimKiem = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(0, 2, 16, 2) };
            UIStyleHelper.StyleTextBox(txtTimKiem);
            txtTimKiem.Font = new Font("Segoe UI", 10.5F);
            try { txtTimKiem.PlaceholderText = "Tìm kiếm sản phẩm theo tên hoặc cấu hình..."; } catch { }
            txtTimKiem.TextChanged += (s, e) => TaiDanhSachSanPham();
            tblSearch.Controls.Add(txtTimKiem, 1, 0);

            var btnDH = new Button
            {
                Text = "🛒  Đặt mua SP đã chọn",
                AutoSize = true,
                Padding = new Padding(14, 5, 14, 5),
                BackColor = UIStyleHelper.PrimaryBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDH.FlatAppearance.BorderSize = 0;
            btnDH.Click += BtnDatHang_Click;
            tblSearch.Controls.Add(btnDH, 2, 0);

            // 2. Card danh mục chính (Gọn gàng 7 card)
            var pnlCats = new Panel
            {
                Dock = DockStyle.Top,
                Height = 115,
                BackColor = Color.FromArgb(15, 23, 42),
                Padding = new Padding(14, 10, 14, 10)
            };
            var flowCats = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.Transparent
            };

            Panel? firstCard = null;
            foreach (var (key, icon, name, accent) in Categories)
            {
                var card = CreateCategoryCard(key, icon, name, accent);
                flowCats.Controls.Add(card);
                if (key == "all") firstCard = card;
            }
            pnlCats.Controls.Add(flowCats);

            // 3. Sub-filter bar cho Linh Kiện (Ẩn mặc định, chỉ hiện khi chọn Linh Kiện)
            pnlSubLinhKien = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Visible = false, // Mặc định ẩn
                BackColor = Color.FromArgb(30, 41, 59),
                Padding = new Padding(14, 8, 14, 8),
                WrapContents = true
            };

            Button? firstSubBtn = null;
            foreach (var (subKey, subName) in SubLinhKien)
            {
                string capturedSubKey = subKey;
                var btnSub = new Button
                {
                    Text = subName,
                    AutoSize = true,
                    Padding = new Padding(12, 4, 12, 4),
                    Margin = new Padding(0, 0, 8, 4),
                    BackColor = Color.FromArgb(51, 65, 85),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnSub.FlatAppearance.BorderSize = 0;
                btnSub.Click += (s, e) => SelectSubLinhKien(capturedSubKey, btnSub);
                pnlSubLinhKien.Controls.Add(btnSub);

                if (subKey == "all_lk") firstSubBtn = btnSub;
            }
            if (firstSubBtn != null) SelectSubLinhKien("all_lk", firstSubBtn);

            // 4. Product Grid
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

            tab.Controls.Add(dgvSanPham);
            tab.Controls.Add(pnlSubLinhKien);
            tab.Controls.Add(pnlCats);
            tab.Controls.Add(tblSearch);

            if (firstCard != null) SelectCategory("all", firstCard);
        }

        private Panel CreateCategoryCard(string key, string icon, string name, Color accent)
        {
            var card = new Panel
            {
                Width = 125, Height = 92,
                BackColor = accent,
                Margin = new Padding(0, 0, 10, 0),
                Cursor = Cursors.Hand, Tag = key
            };
            var lI = new Label
            {
                Text = icon, Font = new Font("Segoe UI Emoji", 26F), ForeColor = Color.White,
                AutoSize = false, Size = new Size(125, 56), Location = new Point(0, 5),
                TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent
            };
            var lN = new Label
            {
                Text = name, Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(226, 232, 240), AutoSize = false,
                Size = new Size(125, 30), Location = new Point(0, 62),
                TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent
            };
            card.Controls.Add(lI); card.Controls.Add(lN);
            EventHandler click = (s, e) => SelectCategory(key, card);
            foreach (Control c in new Control[] { card, lI, lN })
            {
                c.Click += click; c.Cursor = Cursors.Hand;
                c.MouseEnter += (s, e) => { if (card != _selectedCard) card.BackColor = ControlPaint.Light(accent, 0.15f); };
                c.MouseLeave += (s, e) => { if (card != _selectedCard) card.BackColor = accent; };
            }
            return card;
        }

        private void SelectCategory(string key, Panel card)
        {
            if (_selectedCard != null)
            {
                var oldKey = _selectedCard.Tag?.ToString() ?? "all";
                _selectedCard.BackColor = Categories.FirstOrDefault(c => c.Key == oldKey).Accent;
            }
            _categoryKey = key; _selectedCard = card;
            card.BackColor = UIStyleHelper.PrimaryBlue;

            // Bấm vào Linh Kiện thì HIỆN Sub-filter bar, chọn loại khác thì ẨN
            if (key == "linhkien")
            {
                pnlSubLinhKien.Visible = true;
            }
            else
            {
                pnlSubLinhKien.Visible = false;
            }

            TaiDanhSachSanPham();
        }

        private void SelectSubLinhKien(string subKey, Button btnSub)
        {
            if (_selectedSubBtn != null)
            {
                _selectedSubBtn.BackColor = Color.FromArgb(51, 65, 85);
            }
            _subComponentKey = subKey;
            _selectedSubBtn = btnSub;
            btnSub.BackColor = UIStyleHelper.PrimaryBlue;

            TaiDanhSachSanPham();
        }

        private void TaiDanhSachSanPham()
        {
            if (dgvSanPham == null) return;

            string kw = txtTimKiem?.Text.Trim().ToLower() ?? "";
            var all = new SanPhamDAO().GetAll();

            int dmPC       = _dmList.FirstOrDefault(d => d.TenDanhMuc.ToLower().Contains("để bàn"))?.MaDanhMuc ?? -1;
            int dmLaptop   = _dmList.FirstOrDefault(d => d.TenDanhMuc.ToLower().Contains("xách tay"))?.MaDanhMuc ?? -1;
            int dmLinhKien = _dmList.FirstOrDefault(d => d.TenDanhMuc.ToLower().Contains("linh kiện"))?.MaDanhMuc ?? -1;
            int dmNgoaiVi  = _dmList.FirstOrDefault(d => d.TenDanhMuc.ToLower().Contains("ngoại vi"))?.MaDanhMuc ?? -1;

            IEnumerable<SanPham> filtered = all;
            switch (_categoryKey)
            {
                case "gaming":
                    filtered = all.Where(p => p.MaDanhMuc == dmPC &&
                        (p.TenSanPham.ToLower().Contains("gaming") || p.TenSanPham.ToLower().Contains("game") ||
                         p.TenSanPham.ToLower().Contains("rog") || (p.CauHinh ?? "").ToLower().Contains("rtx") ||
                         (p.CauHinh ?? "").ToLower().Contains("rx 7") || (p.CauHinh ?? "").ToLower().Contains("rx 6")));
                    break;
                case "vanphong":
                    filtered = all.Where(p => p.MaDanhMuc == dmPC &&
                        (p.TenSanPham.ToLower().Contains("văn phòng") || p.TenSanPham.ToLower().Contains("van phong") ||
                         p.TenSanPham.ToLower().Contains("office") || p.TenSanPham.ToLower().Contains("prodesk")));
                    break;
                case "workstation":
                    filtered = all.Where(p => p.MaDanhMuc == dmPC &&
                        (p.TenSanPham.ToLower().Contains("workstation") || p.TenSanPham.ToLower().Contains("xeon") ||
                         (p.CauHinh ?? "").ToLower().Contains("workstation")));
                    break;
                case "laptop":
                    filtered = all.Where(p => p.MaDanhMuc == dmLaptop);
                    break;
                case "linhkien":
                    // Lọc theo sub-filter của linh kiện
                    var lkList = all.Where(p => !IsFullSystem(p) && (p.MaDanhMuc == dmLinhKien || MatchKeywords(p, "cpu", "ram", "vga", "mainboard", "ssd", "hdd", "nguồn", "case", "psu"))).ToList();
                    filtered = _subComponentKey switch
                    {
                        "cpu"       => lkList.Where(p => MatchKeywords(p, "cpu", "core i", "ryzen", "intel", "processor")),
                        "mainboard" => lkList.Where(p => MatchKeywords(p, "mainboard", "bo mạch", "z790", "b660", "x570", "b450")),
                        "ram"       => lkList.Where(p => MatchKeywords(p, "ram", "ddr4", "ddr5", "ddr3")),
                        "vga"       => lkList.Where(p => MatchKeywords(p, "vga", "rtx", "gtx", "radeon", "rx ", "card đồ họa")),
                        "ssd"       => lkList.Where(p => MatchKeywords(p, "ssd", "hdd", "nvme", "sata", "ổ cứng", "m.2")),
                        "nguon"     => lkList.Where(p => MatchKeywords(p, "nguồn", "psu", "power supply", "650w", "750w", "850w")),
                        "case"      => lkList.Where(p => MatchKeywords(p, "case", "vỏ máy", "thùng máy", "mid tower")),
                        _           => lkList
                    };
                    break;
                case "ngoaivi":
                    filtered = all.Where(p => p.MaDanhMuc == dmNgoaiVi || MatchKeywords(p, "màn hình", "chuột", "bàn phím", "tai nghe", "loa"));
                    break;
            }

            if (!string.IsNullOrEmpty(kw))
                filtered = filtered.Where(p =>
                    p.TenSanPham.ToLower().Contains(kw) || (p.CauHinh ?? "").ToLower().Contains(kw));

            dgvSanPham.DataSource = filtered.Select(p => new
            {
                p.MaSanPham, p.TenSanPham, p.TenDanhMuc, p.CauHinh,
                GiaBan = p.GiaBanFormatted, p.SoLuongTon, TrangThai = p.TrangThaiDisplay
            }).ToList();

            if (dgvSanPham.Columns["MaSanPham"]  != null) dgvSanPham.Columns["MaSanPham"].HeaderText  = "Mã SP";
            if (dgvSanPham.Columns["TenSanPham"] != null) dgvSanPham.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";
            if (dgvSanPham.Columns["TenDanhMuc"] != null) dgvSanPham.Columns["TenDanhMuc"].HeaderText = "Danh Mục";
            if (dgvSanPham.Columns["CauHinh"]    != null) dgvSanPham.Columns["CauHinh"].HeaderText    = "Cấu Hình";
            if (dgvSanPham.Columns["GiaBan"]     != null) dgvSanPham.Columns["GiaBan"].HeaderText     = "Giá Bán";
            if (dgvSanPham.Columns["SoLuongTon"] != null) dgvSanPham.Columns["SoLuongTon"].HeaderText = "Số Lượng";
            if (dgvSanPham.Columns["TrangThai"]  != null) dgvSanPham.Columns["TrangThai"].HeaderText  = "Trạng Thái";
        }

        private static bool MatchKeywords(SanPham p, params string[] keywords)
        {
            string name = p.TenSanPham.ToLower();
            string spec = (p.CauHinh ?? "").ToLower();
            return keywords.Any(kw => name.Contains(kw) || spec.Contains(kw));
        }

        private static bool IsFullSystem(SanPham p)
        {
            string name = p.TenSanPham.ToLower();
            return name.Contains("pc ") || name.Contains("máy tính") || name.Contains("laptop") || name.Contains("workstation");
        }

        private void BtnDatHang_Click(object? sender, EventArgs e)
        {
            if (dgvSanPham.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int maSP = (int)dgvSanPham.CurrentRow.Cells["MaSanPham"].Value;
            var sp = new SanPhamDAO().GetById(maSP);
            if (sp == null || sp.SoLuongTon <= 0)
            {
                MessageBox.Show("Sản phẩm đã hết hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_khachHangCurrent == null)
            {
                MessageBox.Show("Không tìm thấy thông tin khách hàng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using var dlg = new frmDatHang(sp, _khachHangCurrent);
            if (dlg.ShowDialog() == DialogResult.OK) { TaiDanhSachSanPham(); TaiLichSuDonHang(); }
        }

        // ════════════════════════════════════════════════════════════
        // TAB 2: ĐƠN HÀNG
        // ════════════════════════════════════════════════════════════
        private void InitTab_DonHang(TabPage tab)
        {
            var lbl = new Label
            {
                Text = "📋  Lịch sử đơn hàng — click vào đơn để xem chi tiết sản phẩm",
                Dock = DockStyle.Top, Height = 36,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(148, 163, 184),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                BackColor = UIStyleHelper.BgCard
            };
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 260
            };
            dgvDonHang = new DataGridView
            {
                Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false, ReadOnly = true, AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvDonHang);
            dgvDonHang.SelectionChanged += DgvDonHang_SelectionChanged;

            dgvChiTietDH = new DataGridView
            {
                Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true, AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvChiTietDH);
            split.Panel1.Controls.Add(dgvDonHang);
            split.Panel2.Controls.Add(dgvChiTietDH);
            tab.Controls.Add(split);
            tab.Controls.Add(lbl);
        }

        private void TaiLichSuDonHang()
        {
            if (_khachHangCurrent == null) return;
            var list = new DonHangDAO().GetByMaKhachHang(_khachHangCurrent.MaKhachHang);
            dgvDonHang.DataSource = list.Select(d => new
            {
                d.MaDonHang, NgayDat = d.NgayDatHang, d.TenPhuongThuc,
                TongTien = d.TongTien.ToString("N0") + " đ", TrangThai = d.TrangThaiDisplay, d.GhiChu
            }).ToList();
            if (dgvDonHang.Columns["MaDonHang"]     != null) dgvDonHang.Columns["MaDonHang"].HeaderText     = "Mã Đơn";
            if (dgvDonHang.Columns["NgayDat"]       != null) dgvDonHang.Columns["NgayDat"].HeaderText       = "Ngày Đặt";
            if (dgvDonHang.Columns["TenPhuongThuc"] != null) dgvDonHang.Columns["TenPhuongThuc"].HeaderText = "Phương Thức TT";
            if (dgvDonHang.Columns["TongTien"]      != null) dgvDonHang.Columns["TongTien"].HeaderText      = "Tổng Tiền";
            if (dgvDonHang.Columns["TrangThai"]     != null) dgvDonHang.Columns["TrangThai"].HeaderText     = "Trạng Thái";
            if (dgvDonHang.Columns["GhiChu"]        != null) dgvDonHang.Columns["GhiChu"].HeaderText        = "Ghi Chú";
        }

        private void DgvDonHang_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvDonHang.CurrentRow == null) return;
            int maDH = (int)dgvDonHang.CurrentRow.Cells["MaDonHang"].Value;
            var ct = new DonHangDAO().GetChiTiet(maDH);
            dgvChiTietDH.DataSource = ct.Select(c => new
            {
                c.TenSanPham, c.SoLuong, DonGia = c.DonGiaFormatted, ThanhTien = c.ThanhTienFormatted
            }).ToList();
            if (dgvChiTietDH.Columns["TenSanPham"] != null) dgvChiTietDH.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";
            if (dgvChiTietDH.Columns["SoLuong"]    != null) dgvChiTietDH.Columns["SoLuong"].HeaderText    = "Số Lượng";
            if (dgvChiTietDH.Columns["DonGia"]     != null) dgvChiTietDH.Columns["DonGia"].HeaderText     = "Đơn Giá";
            if (dgvChiTietDH.Columns["ThanhTien"]  != null) dgvChiTietDH.Columns["ThanhTien"].HeaderText  = "Thành Tiền";
        }

        private void TabControl_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabDonHang) TaiLichSuDonHang();
        }

        // ════════════════════════════════════════════════════════════
        // TAB 3: BUILD CẤU HÌNH PC — Clean Row-by-Row
        // ════════════════════════════════════════════════════════════
        private void InitTab_BuildPC(TabPage tab)
        {
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 640
            };
            split.Panel1.BackColor = UIStyleHelper.BgMain;
            split.Panel2.BackColor = Color.FromArgb(22, 33, 54);

            // ──────────────────────────────────────────────────────────
            // CỘT TRÁI: BẢNG CHỌN LINH KIỆN TỪNG DÒNG
            // ──────────────────────────────────────────────────────────
            var pnlScroll = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = UIStyleHelper.BgMain,
                Padding = new Padding(16, 16, 16, 16)
            };

            var tblList = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 3,
                BackColor = Color.Transparent
            };
            tblList.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190F));
            tblList.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblList.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));

            // Dòng Tiêu đề (Header)
            int rowIdx = 0;
            tblList.RowCount++;
            tblList.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            
            tblList.Controls.Add(new Label { Text = "LOẠI LINH KIỆN", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(148, 163, 184), Anchor = AnchorStyles.Left, AutoSize = true }, 0, rowIdx);
            tblList.Controls.Add(new Label { Text = "CHỌN SẢN PHẨM LINH KIỆN", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(148, 163, 184), Anchor = AnchorStyles.Left, AutoSize = true }, 1, rowIdx);
            tblList.Controls.Add(new Label { Text = "ĐƠN GIÁ", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(148, 163, 184), Anchor = AnchorStyles.Right, AutoSize = true }, 2, rowIdx);

            // Các dòng sản phẩm
            cboCPU       = AddComponentRow(tblList, ref rowIdx, "🔲  Vi Xử Lý (CPU)",      "cpu");
            cboMainboard = AddComponentRow(tblList, ref rowIdx, "🔌  Bo Mạch Chủ (Main)", "mainboard");
            cboRAM       = AddComponentRow(tblList, ref rowIdx, "💾  Bộ Nhớ RAM",        "ram");
            cboGPU       = AddComponentRow(tblList, ref rowIdx, "🎨  Card Đồ Họa (VGA)", "gpu");
            cboSSD       = AddComponentRow(tblList, ref rowIdx, "💿  Ổ Cứng (SSD/HDD)",  "ssd");
            cboNguon     = AddComponentRow(tblList, ref rowIdx, "⚡  Nguồn (PSU)",       "nguon");
            cboCase      = AddComponentRow(tblList, ref rowIdx, "🖥️  Vỏ Case (Thùng máy)","case");

            pnlScroll.Controls.Add(tblList);
            split.Panel1.Controls.Add(pnlScroll);

            // ──────────────────────────────────────────────────────────
            // CỘT PHẢI: TÓM TẮT CẤU HÌNH VÀ NÚT ĐẶT MUA
            // ──────────────────────────────────────────────────────────
            var pnlRight = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };

            var lblBuildTitle = new Label
            {
                Text = "📋 TÓM TẮT CẤU HÌNH MÁY",
                Dock = DockStyle.Top,
                Height = 36,
                Font = new Font("Segoe UI", 11.5F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var sepRight = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Color.FromArgb(51, 65, 85), Margin = new Padding(0, 4, 0, 8) };

            pnlSummaryItems = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.Transparent
            };

            lblBuildTotal = new Label
            {
                Text = "💰 Tổng tiền: 0 đ",
                Dock = DockStyle.Bottom,
                Height = 40,
                Font = new Font("Segoe UI", 11.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 211, 153),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var btnOrder = new Button
            {
                Text = "🛒  ĐẶT TOÀN BỘ CẤU HÌNH",
                Dock = DockStyle.Bottom,
                Height = 44,
                BackColor = UIStyleHelper.PrimaryBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 6, 0, 0)
            };
            btnOrder.FlatAppearance.BorderSize = 0;
            btnOrder.Click += BtnDatCauHinh_Click;

            var btnReset = new Button
            {
                Text = "🔄  Bỏ chọn tất cả linh kiện",
                Dock = DockStyle.Bottom,
                Height = 34,
                BackColor = Color.FromArgb(51, 65, 85),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 6, 0, 4)
            };
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.Click += (s, e) => ResetAllSelections();

            pnlRight.Controls.Add(pnlSummaryItems);
            pnlRight.Controls.Add(lblBuildTotal);
            pnlRight.Controls.Add(btnOrder);
            pnlRight.Controls.Add(btnReset);
            pnlRight.Controls.Add(sepRight);
            pnlRight.Controls.Add(lblBuildTitle);

            split.Panel2.Controls.Add(pnlRight);
            tab.Controls.Add(split);
        }

        private ComboBox AddComponentRow(TableLayoutPanel tbl, ref int rowIdx, string labelText, string typeKey)
        {
            rowIdx++;
            tbl.RowCount++;
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));

            var lblName = new Label
            {
                Text = labelText,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(226, 232, 240),
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Margin = new Padding(0, 4, 8, 4)
            };

            var cbo = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Tag = typeKey,
                Margin = new Padding(0, 8, 12, 8)
            };
            UIStyleHelper.StyleComboBox(cbo);

            var lblPrice = new Label
            {
                Text = "—",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 211, 153),
                Anchor = AnchorStyles.Right,
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 4)
            };

            cbo.SelectedIndexChanged += (s, e) =>
            {
                if (cbo.SelectedItem is BuildComboItem it && it.Product != null)
                    lblPrice.Text = it.Product.GiaBanFormatted;
                else
                    lblPrice.Text = "—";

                CapNhatBuildSummary();
            };

            tbl.Controls.Add(lblName,  0, rowIdx);
            tbl.Controls.Add(cbo,      1, rowIdx);
            tbl.Controls.Add(lblPrice, 2, rowIdx);

            return cbo;
        }

        // ── Load CHỈ LINH KIỆN LẺ vào combobox Build PC ───────────────
        private void NapComponentsBuild()
        {
            var all = new SanPhamDAO().GetAll();
            var linhKienOnly = all.Where(p => !IsFullSystem(p)).ToList();

            var slots = new (ComboBox Cbo, string[] Keywords)[]
            {
                (cboCPU,       new[] {"cpu", "core i", "ryzen", "intel", "processor"}),
                (cboMainboard, new[] {"mainboard", "bo mạch", "z790", "b660", "x570", "b450"}),
                (cboRAM,       new[] {"ram", "ddr4", "ddr5", "ddr3"}),
                (cboGPU,       new[] {"vga", "rtx", "gtx", "radeon", "rx ", "card đồ họa"}),
                (cboSSD,       new[] {"ssd", "hdd", "nvme", "sata", "ổ cứng", "m.2"}),
                (cboNguon,     new[] {"nguồn", "psu", "power supply", "650w", "750w", "850w"}),
                (cboCase,      new[] {"case", "vỏ máy", "thùng máy", "mid tower"}),
            };

            foreach (var (cbo, keywords) in slots)
            {
                if (cbo == null) continue;
                var matching = linhKienOnly.Where(p => MatchKeywords(p, keywords)).OrderBy(p => p.GiaBan).ToList();

                cbo.Items.Clear();
                cbo.Items.Add(new BuildComboItem { Display = "-- Không chọn --", Product = null });
                foreach (var p in matching)
                    cbo.Items.Add(new BuildComboItem { Display = $"{p.TenSanPham} ({p.GiaBanFormatted})", Product = p });
                cbo.SelectedIndex = 0;
            }
        }

        private void ResetAllSelections()
        {
            var cbos = new[] { cboCPU, cboMainboard, cboRAM, cboGPU, cboSSD, cboNguon, cboCase };
            foreach (var c in cbos)
                if (c != null && c.Items.Count > 0) c.SelectedIndex = 0;
        }

        private void CapNhatBuildSummary()
        {
            decimal total = 0;
            pnlSummaryItems?.Controls.Clear();

            var slotNames = new[] { "CPU", "Mainboard", "RAM", "VGA", "SSD", "Nguồn", "Case" };
            var cbos = new[] { cboCPU, cboMainboard, cboRAM, cboGPU, cboSSD, cboNguon, cboCase };

            for (int i = 0; i < cbos.Length; i++)
            {
                if (cbos[i] != null && cbos[i].SelectedItem is BuildComboItem item && item.Product != null)
                {
                    total += item.Product.GiaBan;

                    if (pnlSummaryItems != null)
                    {
                        var row = new TableLayoutPanel
                        {
                            Width = pnlSummaryItems.Width - 24,
                            AutoSize = true,
                            ColumnCount = 2,
                            BackColor = Color.Transparent,
                            Margin = new Padding(0, 3, 0, 3)
                        };
                        row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
                        row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
                        row.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                        row.Controls.Add(new Label
                        {
                            Text = $"[{slotNames[i]}] {item.Product.TenSanPham}",
                            AutoSize = false,
                            Dock = DockStyle.Fill,
                            Font = new Font("Segoe UI", 8.5F),
                            ForeColor = Color.FromArgb(203, 213, 225),
                            TextAlign = ContentAlignment.MiddleLeft
                        }, 0, 0);
                        row.Controls.Add(new Label
                        {
                            Text = item.Product.GiaBanFormatted,
                            AutoSize = false,
                            Dock = DockStyle.Fill,
                            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                            ForeColor = Color.FromArgb(52, 211, 153),
                            TextAlign = ContentAlignment.MiddleRight
                        }, 1, 0);

                        pnlSummaryItems.Controls.Add(row);
                    }
                }
            }

            lblBuildTotal.Text = total > 0
                ? $"💰 Tổng tiền: {total:N0} đ"
                : "💰 Tổng tiền: 0 đ";
        }

        private void BtnDatCauHinh_Click(object? sender, EventArgs e)
        {
            var cbos = new[] { cboCPU, cboMainboard, cboRAM, cboGPU, cboSSD, cboNguon, cboCase };
            var selected = cbos.Where(c => c != null)
                               .Select(c => c!.SelectedItem as BuildComboItem)
                               .Where(it => it?.Product != null).ToList();

            if (selected.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một linh kiện.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_khachHangCurrent == null)
            {
                MessageBox.Show("Không tìm thấy thông tin khách hàng.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal total = selected.Sum(it => it!.Product!.GiaBan);
            string list = string.Join("\n", selected.Select(it => $"  • {it!.Product!.TenSanPham}: {it.Product.GiaBanFormatted}"));

            var res = MessageBox.Show(
                $"Xác nhận đặt cấu hình PC:\n\n{list}\n\n💰 Tổng tiền: {total:N0} đ\n\nBạn có muốn tiến hành đặt hàng không?",
                "Xác nhận đặt cấu hình", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res != DialogResult.Yes) return;

            var ptttList = new PhuongThucThanhToanDAO().GetAll();
            if (ptttList.Count == 0) { MessageBox.Show("Không có phương thức thanh toán.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            var dh = new DonHang
            {
                MaKhachHang    = _khachHangCurrent.MaKhachHang,
                MaPhuongThucTT = ptttList[0].MaPhuongThucTT,
                GhiChu         = "Build cấu hình PC linh kiện tự chọn"
            };
            var ctList = selected.Select(it => new ChiTietDonHang
            {
                MaSanPham = it!.Product!.MaSanPham, SoLuong = 1, DonGia = it.Product.GiaBan
            }).ToList();

            int maDH = new DonHangDAO().Insert(dh, ctList);
            if (maDH > 0)
            {
                MessageBox.Show($"🎉 Đặt cấu hình thành công!\nMã đơn hàng: #{maDH}\nNhân viên sẽ liên hệ xác nhận đơn hàng.",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TaiLichSuDonHang();
            }
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                this.Close();
        }

        // ── Inner helper ─────────────────────────────────────────────
        private class BuildComboItem
        {
            public string Display { get; set; } = "";
            public SanPham? Product { get; set; }
            public override string ToString() => Display;
        }
    }
}
