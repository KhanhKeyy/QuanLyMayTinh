using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    /// <summary>
    /// Form chính — sidebar trắng sáng với accordion sub-menu.
    /// </summary>
    public partial class frmMain : Form
    {
        // Keeps track of which accordion panel is open
        private Panel? _openAccordion;
        private Button? _activeParentBtn;

        public frmMain()
        {
            InitializeComponent();
            WireUpSidebarButtons();
        }

        // ═══════════════════════════════════════════════════════════════
        // LOAD
        // ═══════════════════════════════════════════════════════════════
        private void frmMain_Load(object sender, EventArgs e)
        {
            CapNhatThanhTieuDe();
            ApdungPhanQuyen();
            HienThiTrangChu();
        }

        private void frmMain_Resize(object? sender, EventArgs e)
            => RepositionRightButtons();

        // ═══════════════════════════════════════════════════════════════
        // WIRE UP SIDEBAR BUTTONS → ACCORDION
        // ═══════════════════════════════════════════════════════════════
        private void WireUpSidebarButtons()
        {
            // Dashboard — direct (no sub-items)
            btnDashboard.Click += (s, e) =>
            {
                CloseAccordion();
                SetActiveSidebarBtn(btnDashboard, isParent: true);
                HienThiTrangChu();
            };

            // Hàng hoá — accordion with sub-items
            btnHangHoa.Click += (s, e) => ToggleAccordion(btnHangHoa, new[]
            {
                ("Xem danh sách SP", (Action)(() => { SetActiveSidebarBtn(btnHangHoa, isParent:false); MoFormTrong(new frmSanPham()); })),
                ("Thêm sản phẩm",   (Action)(() => { SetActiveSidebarBtn(btnHangHoa, isParent:false); MoFormTrong(new frmSanPham(cheBoDuyet: true)); }))
            });

            // Nhập hàng — accordion
            btnNhapHang.Click += (s, e) => ToggleAccordion(btnNhapHang, new[]
            {
                ("Lập phiếu nhập", (Action)(() => { SetActiveSidebarBtn(btnNhapHang, isParent:false); MoFormTrong(new frmNhapHang()); })),
                ("Danh sách phiếu",(Action)(() => { SetActiveSidebarBtn(btnNhapHang, isParent:false); MoFormTrong(new frmNhapHang()); }))
            });

            // Đơn hàng — accordion
            btnDonHang.Click += (s, e) => ToggleAccordion(btnDonHang, new[]
            {
                ("Xem tất cả đơn",   (Action)(() => { SetActiveSidebarBtn(btnDonHang, isParent:false); MoFormTrong(new frmDonHang()); })),
                ("Xác nhận đơn hàng",(Action)(() =>
                {
                    if (!UserSession.IsNVBanHang && !UserSession.IsQuanLy)
                    {
                        MessageBox.Show("Bạn không có quyền xác nhận đơn hàng.",
                            "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    SetActiveSidebarBtn(btnDonHang, isParent:false);
                    MoFormTrong(new frmDonHang(cheBoDuyet: true));
                }))
            });

            // Hóa đơn — accordion
            btnHoaDon.Click += (s, e) => ToggleAccordion(btnHoaDon, new[]
            {
                ("Danh sách hóa đơn", (Action)(() => { SetActiveSidebarBtn(btnHoaDon, isParent:false); MoFormTrong(new frmHoaDon(defaultTabIndex: 0)); })),
                ("Lập hóa đơn mới",   (Action)(() => { SetActiveSidebarBtn(btnHoaDon, isParent:false); MoFormTrong(new frmHoaDon(defaultTabIndex: 1)); }))
            });

            // Tồn kho — direct
            btnTonKho.Click += (s, e) => ToggleAccordion(btnTonKho, new[]
            {
                ("Xem tồn kho",   (Action)(() => { SetActiveSidebarBtn(btnTonKho, isParent:false); MoFormTrong(new frmTonKho()); })),
                ("Cập nhật tồn kho",(Action)(() => { SetActiveSidebarBtn(btnTonKho, isParent:false); MoFormTrong(new frmTonKho()); }))
            });

            // Báo cáo — accordion
            btnBaoCao.Click += (s, e) => ToggleAccordion(btnBaoCao, new[]
            {
                ("Doanh thu",          (Action)(() => { SetActiveSidebarBtn(btnBaoCao, isParent:false); MoFormTrong(new frmBaoCao(loaiBaoCao: "DoanhThu")); })),
                ("Hàng tồn kho",       (Action)(() => { SetActiveSidebarBtn(btnBaoCao, isParent:false); MoFormTrong(new frmBaoCao(loaiBaoCao: "TonKho")); })),
                ("Sản phẩm bán chạy",  (Action)(() => { SetActiveSidebarBtn(btnBaoCao, isParent:false); MoFormTrong(new frmBaoCao(loaiBaoCao: "BanChay")); }))
            });

            // Nhân viên — direct
            btnNhanVien.Click += (s, e) =>
            {
                CloseAccordion();
                SetActiveSidebarBtn(btnNhanVien, isParent: true);
                MoFormTrong(new frmNhanVien());
            };

            // Danh mục — direct
            btnDanhMuc.Click += (s, e) =>
            {
                CloseAccordion();
                SetActiveSidebarBtn(btnDanhMuc, isParent: true);
                MoFormTrong(new frmDanhMuc());
            };
        }

        // ═══════════════════════════════════════════════════════════════
        // ACCORDION LOGIC
        // ═══════════════════════════════════════════════════════════════
        private void ToggleAccordion(Button parentBtn, (string label, Action action)[] items)
        {
            // If same button is clicked while open → close and return
            if (_openAccordion != null && _activeParentBtn == parentBtn)
            {
                CloseAccordion();
                return;
            }

            CloseAccordion();
            _activeParentBtn = parentBtn;

            // Style parent as expanded
            parentBtn.BackColor = Color.FromArgb(239, 246, 255);
            parentBtn.ForeColor = Color.FromArgb(37, 99, 235);
            parentBtn.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);

            // Build accordion panel
            int subH = items.Length * 38;
            var accordion = new Panel
            {
                BackColor = Color.FromArgb(248, 250, 252),
                Size      = new Size(240, subH),
                Location  = new Point(0, parentBtn.Bottom)
            };

            for (int i = 0; i < items.Length; i++)
            {
                var (label, action) = items[i];
                var subBtn = new Button
                {
                    Text      = "   › " + label,
                    Size      = new Size(240, 36),
                    Location  = new Point(0, i * 36),
                    Font      = new Font("Segoe UI", 9F),
                    ForeColor = Color.FromArgb(75, 85, 99),
                    BackColor = Color.FromArgb(248, 250, 252),
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding   = new Padding(16, 2, 4, 2),
                    Cursor    = Cursors.Hand,
                    UseCompatibleTextRendering = true,
                    AutoEllipsis = false
                };
                subBtn.FlatAppearance.BorderSize         = 0;
                subBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(239, 246, 255);
                subBtn.Click += (s, e) =>
                {
                    // Mark the sub-button as active
                    foreach (Control c in accordion.Controls)
                        if (c is Button b) { b.ForeColor = Color.FromArgb(75, 85, 99); b.Font = new Font("Segoe UI", 9.5F); }
                    subBtn.ForeColor = Color.FromArgb(37, 99, 235);
                    subBtn.Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                    action();
                };
                accordion.Controls.Add(subBtn);
            }

            _openAccordion = accordion;
            panelSidebar.Controls.Add(accordion);
            accordion.BringToFront();

            ReflowSidebarButtons();
        }

        private void CloseAccordion()
        {
            if (_openAccordion != null)
            {
                panelSidebar.Controls.Remove(_openAccordion);
                _openAccordion.Dispose();
                _openAccordion = null;
            }
            // Reset parent button style
            if (_activeParentBtn != null)
            {
                _activeParentBtn.BackColor = Color.White;
                _activeParentBtn.ForeColor = Color.FromArgb(55, 65, 81);
                _activeParentBtn.Font      = new Font("Segoe UI", 10F);
                _activeParentBtn = null;
            }
            ReflowSidebarButtons();
        }

        // Reflow all visible buttons (leaves space for open accordion)
        private void ReflowSidebarButtons()
        {
            Button[] ordered = { btnDashboard, btnHangHoa, btnNhapHang, btnDonHang,
                                 btnHoaDon, btnTonKho, btnBaoCao, btnNhanVien, btnDanhMuc };
            int y = panelSidebarUser.Bottom + 4;
            foreach (var b in ordered)
            {
                if (!b.Visible) continue;
                b.Location = new Point(0, y);
                y += 44;
                // If this button has an open accordion below it, skip space
                if (_activeParentBtn == b && _openAccordion != null)
                {
                    _openAccordion.Location = new Point(0, y);
                    y += _openAccordion.Height;
                }
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // ACTIVE BUTTON STYLING
        // ═══════════════════════════════════════════════════════════════
        private void SetActiveSidebarBtn(Button active, bool isParent)
        {
            // Reset all parent buttons (don't touch accordion panels)
            Button[] all = { btnDashboard, btnHangHoa, btnNhapHang, btnDonHang,
                             btnHoaDon, btnTonKho, btnBaoCao, btnNhanVien, btnDanhMuc };
            foreach (var b in all)
            {
                if (b == _activeParentBtn) continue; // accordion parent keeps its style
                b.BackColor = Color.White;
                b.ForeColor = Color.FromArgb(55, 65, 81);
                b.Font      = new Font("Segoe UI", 10F);
            }
            if (isParent)
            {
                active.BackColor = Color.FromArgb(239, 246, 255);
                active.ForeColor = Color.FromArgb(37, 99, 235);
                active.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // TOP BAR — position notification & logout at right
        // ═══════════════════════════════════════════════════════════════
        private Button? btnThongBao;

        private void CapNhatThanhTieuDe()
        {
            string vaiTroText = UserSession.IsQuanLy    ? "Quản lý"
                              : UserSession.IsKeToan    ? "Kế toán"
                              : UserSession.IsNVBanHang ? "NV Bán hàng"
                              : "Nhân viên";

            this.Text = $"Quản Lý Bán Máy Vi Tính – {UserSession.DisplayName}";

            lblTenNguoiDung.Font     = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTenNguoiDung.Text     = $"👤 {UserSession.DisplayName}";
            lblTenNguoiDung.AutoSize = true;
            lblTenNguoiDung.Location = new Point(12, 18);

            lblVaiTro.Font     = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblVaiTro.Text     = vaiTroText;
            lblVaiTro.AutoSize = true;
            lblVaiTro.Padding  = new Padding(8, 3, 8, 3);
            lblVaiTro.Location = new Point(lblTenNguoiDung.Right + 8, 17);

            lblVaiTro.BackColor = UserSession.IsQuanLy    ? Color.FromArgb(124, 58, 237)
                                : UserSession.IsKeToan    ? Color.FromArgb(5, 150, 105)
                                : UserSession.IsNVBanHang ? Color.FromArgb(37, 99, 235)
                                : Color.FromArgb(71, 85, 105);

            CapNhatBadgeThongBao();
        }

        public void CapNhatBadgeThongBao()
        {
            if (UserSession.CurrentAccount == null) return;
            int chuaDoc = new DataAccess.ThongBaoDAO().DemChuaDoc(UserSession.CurrentAccount.MaTaiKhoan);

            if (btnThongBao == null)
            {
                btnThongBao = new Button
                {
                    Size      = new Size(148, 32),
                    Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Cursor    = Cursors.Hand,
                    Name      = "btnThongBao",
                    UseCompatibleTextRendering = false
                };
                btnThongBao.FlatAppearance.BorderSize = 1;
                btnThongBao.Click += (s, e) =>
                {
                    using var dlg = new frmThongBao();
                    dlg.ShowDialog();
                    CapNhatBadgeThongBao();
                };
                panelTop.Controls.Add(btnThongBao);
            }

            if (chuaDoc > 0)
            {
                btnThongBao.Text      = $"🔔 TB ({chuaDoc})";
                btnThongBao.BackColor = Color.FromArgb(254, 242, 242);
                btnThongBao.ForeColor = Color.FromArgb(220, 38, 38);
                btnThongBao.FlatAppearance.BorderColor = Color.FromArgb(252, 165, 165);
                btnThongBao.FlatAppearance.MouseOverBackColor = Color.FromArgb(254, 226, 226);
            }
            else
            {
                btnThongBao.Text      = "🔔 Thông báo";
                btnThongBao.BackColor = Color.FromArgb(248, 250, 252);
                btnThongBao.ForeColor = Color.FromArgb(55, 65, 81);
                btnThongBao.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
                btnThongBao.FlatAppearance.MouseOverBackColor = Color.FromArgb(243, 244, 246);
            }

            RepositionRightButtons();
        }

        private void RepositionRightButtons()
        {
            if (panelTop == null || !panelTop.IsHandleCreated) return;

            int topBarW = panelTop.ClientSize.Width;
            int btnH    = 34;
            int btnTop  = (panelTop.Height - btnH) / 2;
            const int gap    = 8;
            const int margin = 14;

            // Logout — rightmost
            btnDangXuat.Size     = new Size(110, btnH);
            btnDangXuat.Location = new Point(topBarW - margin - btnDangXuat.Width, btnTop);

            // Notification — to the left of logout
            if (btnThongBao != null)
            {
                btnThongBao.Size     = new Size(148, btnH);
                btnThongBao.Location = new Point(btnDangXuat.Left - gap - btnThongBao.Width, btnTop);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // PHÂN QUYỀN
        // ═══════════════════════════════════════════════════════════════
        private void ApdungPhanQuyen()
        {
            menuHangHoa.Visible  = false; menuNhapHang.Visible = false;
            menuDonHang.Visible  = false; menuHoaDon.Visible   = false;
            menuTonKho.Visible   = false; menuBaoCao.Visible   = false;
            menuNhanVien.Visible = false; menuCaiDat.Visible   = false;

            btnHangHoa.Visible  = false; btnNhapHang.Visible = false;
            btnDonHang.Visible  = false; btnHoaDon.Visible   = false;
            btnTonKho.Visible   = false; btnBaoCao.Visible   = false;
            btnNhanVien.Visible = false; btnDanhMuc.Visible  = false;

            if (UserSession.IsNVBanHang)
            {
                btnHangHoa.Visible = true; btnDonHang.Visible = true; btnTonKho.Visible = true;
                menuHangHoa_Them.Visible = menuHangHoa_Sua.Visible = menuHangHoa_Xoa.Visible = false;
            }
            else if (UserSession.IsKeToan)
            {
                btnHangHoa.Visible = true; btnDonHang.Visible = true;
                btnHoaDon.Visible  = true; btnTonKho.Visible  = true;
                menuHangHoa_Them.Visible = menuHangHoa_Sua.Visible = menuHangHoa_Xoa.Visible = false;
            }
            else if (UserSession.IsQuanLy)
            {
                btnHangHoa.Visible = btnNhapHang.Visible = btnDonHang.Visible =
                btnHoaDon.Visible  = btnTonKho.Visible   = btnBaoCao.Visible  =
                btnNhanVien.Visible = btnDanhMuc.Visible = true;
                menuHangHoa_Them.Visible = menuHangHoa_Sua.Visible = menuHangHoa_Xoa.Visible = true;
            }

            ReflowSidebarButtons();
        }

        // ═══════════════════════════════════════════════════════════════
        // TRANG CHỦ
        // ═══════════════════════════════════════════════════════════════
        private void HienThiTrangChu()
        {
            panelContent.Controls.Clear();
            panelContent.Resize -= PanelContent_Resize;

            var lblTitle = new Label
            {
                Name      = "lblWelcome",
                Text      = $"Chào mừng, {UserSession.DisplayName}!",
                Font      = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                AutoSize  = false,
                Size      = new Size(700, 60),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblSub = new Label
            {
                Name      = "lblHuongDan",
                Text      = ObtainHuongDanTheoQuyen(),
                Font      = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize  = false,
                Size      = new Size(700, 170),
                TextAlign = ContentAlignment.TopLeft
            };

            panelContent.Controls.Add(lblTitle);
            panelContent.Controls.Add(lblSub);
            CenterTrangChu();
            panelContent.Resize += PanelContent_Resize;
        }

        private void PanelContent_Resize(object? sender, EventArgs e) => CenterTrangChu();

        private void CenterTrangChu()
        {
            var lw = panelContent.Controls["lblWelcome"]  as Label;
            var ls = panelContent.Controls["lblHuongDan"] as Label;
            if (lw == null || ls == null) return;
            int w = panelContent.ClientSize.Width;
            int h = panelContent.ClientSize.Height;
            int totalH = lw.Height + 16 + ls.Height;
            int startY = Math.Max(40, (h - totalH) / 2);
            lw.Left = Math.Max(32, (w - lw.Width) / 2);
            lw.Top  = startY;
            ls.Left = lw.Left;
            ls.Top  = lw.Bottom + 16;
        }

        private string ObtainHuongDanTheoQuyen()
        {
            if (UserSession.IsNVBanHang)
                return "📋 Chức năng của bạn:\n" +
                       "  • Kiểm tra tình trạng hàng hoá trong kho\n" +
                       "  • Xác nhận đơn hàng của khách và gửi đến kế toán\n\n" +
                       "Chọn mục trong thanh bên trái để bắt đầu làm việc.";
            if (UserSession.IsKeToan)
                return "💼 Chức năng của bạn:\n" +
                       "  • Lập hóa đơn bán hàng từ đơn hàng đã xác nhận\n" +
                       "  • Cập nhật trạng thái thanh toán\n" +
                       "  • Thông báo kết quả cho khách hàng\n" +
                       "  • Cập nhật số lượng tồn kho sau khi hoàn tất đơn\n\n" +
                       "Chọn mục trong thanh bên trái để bắt đầu làm việc.";
            if (UserSession.IsQuanLy)
                return "🏆 Chức năng của bạn:\n" +
                       "  • Quản lý thông tin sản phẩm và danh mục\n" +
                       "  • Lập phiếu nhập hàng khi tồn kho dưới mức quy định\n" +
                       "  • Theo dõi hoạt động nhập hàng và bán hàng\n" +
                       "  • Thống kê báo cáo doanh thu, tồn kho, sản phẩm bán chạy\n\n" +
                       "Chọn mục trong thanh bên trái để bắt đầu làm việc.";
            return "Chào mừng đến với hệ thống.";
        }

        // ═══════════════════════════════════════════════════════════════
        // NAVIGATION EVENT HANDLERS (kept for sub-item wiring compatibility)
        // ═══════════════════════════════════════════════════════════════
        private void menuHangHoa_XemDanhSach_Click(object? sender, EventArgs e)
            => MoFormTrong(new frmSanPham());

        private void menuHangHoa_Them_Click(object? sender, EventArgs e)
            => MoFormTrong(new frmSanPham(cheBoDuyet: true));

        private void menuNhapHang_Click(object? sender, EventArgs e)
            => MoFormTrong(new frmNhapHang());

        private void menuDonHang_XemDanhSach_Click(object? sender, EventArgs e)
            => MoFormTrong(new frmDonHang());

        private void menuDonHang_XacNhan_Click(object? sender, EventArgs e)
        {
            if (!UserSession.IsNVBanHang && !UserSession.IsQuanLy)
            {
                MessageBox.Show("Bạn không có quyền xác nhận đơn hàng.",
                    "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MoFormTrong(new frmDonHang(cheBoDuyet: true));
        }

        private void menuHoaDon_Click(object? sender, EventArgs e)
            => MoFormTrong(new frmHoaDon(defaultTabIndex: 0));

        private void menuHoaDon_LapMoi_Click(object? sender, EventArgs e)
            => MoFormTrong(new frmHoaDon(defaultTabIndex: 1));

        private void menuTonKho_Click(object? sender, EventArgs e)
            => MoFormTrong(new frmTonKho());

        private void menuBaoCao_DoanhThu_Click(object? sender, EventArgs e)
            => MoFormTrong(new frmBaoCao(loaiBaoCao: "DoanhThu"));

        private void menuBaoCao_TonKho_Click(object? sender, EventArgs e)
            => MoFormTrong(new frmBaoCao(loaiBaoCao: "TonKho"));

        private void menuBaoCao_BanChay_Click(object? sender, EventArgs e)
            => MoFormTrong(new frmBaoCao(loaiBaoCao: "BanChay"));

        private void menuNhanVien_Click(object? sender, EventArgs e)
            => MoFormTrong(new frmNhanVien());

        private void menuCaiDat_Click(object? sender, EventArgs e)
            => MoFormTrong(new frmDanhMuc());

        private void menuDangXuat_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc muốn đăng xuất?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes) this.Close();
        }

        // ═══════════════════════════════════════════════════════════════
        // MDI-LITE: mở form con trong panel nội dung
        // ═══════════════════════════════════════════════════════════════
        private void MoFormTrong(Form formCon)
        {
            panelContent.Controls.Clear();
            formCon.TopLevel        = false;
            formCon.FormBorderStyle = FormBorderStyle.None;
            formCon.Dock            = DockStyle.Fill;
            panelContent.Controls.Add(formCon);
            panelContent.Tag = formCon;
            formCon.Show();
        }
    }
}
