using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    /// <summary>
    /// Form chính dành cho Nhân viên (bán hàng / kế toán / quản lý).
    /// Menu sẽ được ẩn/hiện động dựa theo ChucVu của nhân viên.
    /// </summary>
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        // ═══════════════════════════════════════════════════════════════
        // LOAD FORM → PHÂN QUYỀN MENU
        // ═══════════════════════════════════════════════════════════════
        private void frmMain_Load(object sender, EventArgs e)
        {
            // Hiển thị tên người dùng và vai trò trên thanh tiêu đề
            CapNhatThanhTieuDe();

            // Ẩn/hiện menu theo quyền hạn
            ApdungPhanQuyen();

            // Hiển thị trang chủ mặc định
            HienThiTrangChu();
        }

        // ═══════════════════════════════════════════════════════════════
        // CẬP NHẬT TIÊU ĐỀ & THÔNG TIN NGƯỜI DÙNG
        // ═══════════════════════════════════════════════════════════════
        private void CapNhatThanhTieuDe()
        {
            string vaiTroText = UserSession.IsQuanLy ? "Quản lý"
                              : UserSession.IsKeToan ? "Kế toán"
                              : UserSession.IsNVBanHang ? "Nhân viên bán hàng"
                              : "Nhân viên";

            this.Text = $"Quản Lý Bán Máy Vi Tính  |  {UserSession.DisplayName}  [{vaiTroText}]";

            lblTenNguoiDung.Text = $"👤 {UserSession.DisplayName}";
            lblVaiTro.Text = vaiTroText;

            // Màu badge theo vai trò
            lblVaiTro.BackColor = UserSession.IsQuanLy    ? Color.FromArgb(124, 58, 237)   // Tím - Manager
                                : UserSession.IsKeToan    ? Color.FromArgb(5, 150, 105)    // Xanh lá - Accountant
                                : UserSession.IsNVBanHang ? Color.FromArgb(59, 130, 246)   // Xanh - Sales
                                : Color.FromArgb(71, 85, 105);
        }

        // ═══════════════════════════════════════════════════════════════
        // CORE: ÁP DỤNG PHÂN QUYỀN CHO MENU
        // ═══════════════════════════════════════════════════════════════
        private void ApdungPhanQuyen()
        {
            // ── Reset: ẩn tất cả menu trước ─────────────────────────
            menuHangHoa.Visible         = false;
            menuNhapHang.Visible        = false;
            menuDonHang.Visible         = false;
            menuHoaDon.Visible          = false;
            menuTonKho.Visible          = false;
            menuBaoCao.Visible          = false;
            menuNhanVien.Visible        = false;
            menuCaiDat.Visible          = false;

            // ── Phân quyền theo chức vụ ─────────────────────────────
            if (UserSession.IsNVBanHang)
            {
                // Nhân viên bán hàng: kiểm tra hàng, xác nhận đơn
                menuHangHoa.Visible     = true;   // Xem sản phẩm (chỉ đọc)
                menuDonHang.Visible     = true;   // Xác nhận đơn hàng
                menuTonKho.Visible      = true;   // Xem tình trạng tồn kho

                // Giới hạn sub-item trong menu Hàng hoá (chỉ xem, không sửa)
                menuHangHoa_Them.Visible    = false;
                menuHangHoa_Sua.Visible     = false;
                menuHangHoa_Xoa.Visible     = false;
            }
            else if (UserSession.IsKeToan)
            {
                // Kế toán: hóa đơn, thanh toán, tồn kho
                menuHangHoa.Visible     = true;   // Xem sản phẩm
                menuHoaDon.Visible      = true;   // Lập hóa đơn
                menuTonKho.Visible      = true;   // Cập nhật tồn kho
                menuDonHang.Visible     = true;   // Xem đơn hàng đã xác nhận

                menuHangHoa_Them.Visible    = false;
                menuHangHoa_Sua.Visible     = false;
                menuHangHoa_Xoa.Visible     = false;
            }
            else if (UserSession.IsQuanLy)
            {
                // Quản lý: full access
                menuHangHoa.Visible     = true;
                menuNhapHang.Visible    = true;
                menuDonHang.Visible     = true;
                menuHoaDon.Visible      = true;
                menuTonKho.Visible      = true;
                menuBaoCao.Visible      = true;
                menuNhanVien.Visible    = true;
                menuCaiDat.Visible      = true;

                menuHangHoa_Them.Visible    = true;
                menuHangHoa_Sua.Visible     = true;
                menuHangHoa_Xoa.Visible     = true;
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // HIỂN THỊ NỘI DUNG TRANG CHỦ
        // ═══════════════════════════════════════════════════════════════
        private void HienThiTrangChu()
        {
            panelContent.Controls.Clear();
            // Tháo event cũ (nếu có) trước khi gắn lại, tránh đăng ký trùng
            panelContent.Resize -= PanelContent_Resize;

            var lblWelcome = new Label
            {
                Name      = "lblWelcome",
                Text      = $"Chào mừng, {UserSession.DisplayName}!",
                Font      = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(99, 179, 237),
                AutoSize  = false,
                Size      = new Size(600, 60),
                TextAlign = ContentAlignment.MiddleCenter,
            };

            var lblHuongDan = new Label
            {
                Name      = "lblHuongDan",
                Text      = ObtainHuongDanTheoQuyen(),
                Font      = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize  = false,
                Size      = new Size(620, 200),
                TextAlign = ContentAlignment.TopCenter,
            };

            panelContent.Controls.Add(lblWelcome);
            panelContent.Controls.Add(lblHuongDan);

            // Căn giữa ngay lần đầu rồi gắn sự kiện Resize
            CenterTrangChu();
            panelContent.Resize += PanelContent_Resize;
        }

        // ═══════════════════════════════════════════════════════════════
        // CĂN GIỮA ĐỘNG — dùng chung cho cả frmMain
        // ═══════════════════════════════════════════════════════════════
        private void PanelContent_Resize(object? sender, EventArgs e) => CenterTrangChu();

        private void CenterTrangChu()
        {
            var lblWelcome  = panelContent.Controls["lblWelcome"]  as Label;
            var lblHuongDan = panelContent.Controls["lblHuongDan"] as Label;
            if (lblWelcome == null || lblHuongDan == null) return;

            int w = panelContent.ClientSize.Width;
            int h = panelContent.ClientSize.Height;

            // Tổng chiều cao của cụm nội dung (gap = 16px giữa 2 label)
            int totalH = lblWelcome.Height + 16 + lblHuongDan.Height;
            int startY = (h - totalH) / 2;

            lblWelcome.Left  = (w - lblWelcome.Width)  / 2;
            lblWelcome.Top   = startY;

            lblHuongDan.Left = (w - lblHuongDan.Width) / 2;
            lblHuongDan.Top  = lblWelcome.Bottom + 16;
        }

        private string ObtainHuongDanTheoQuyen()
        {
            if (UserSession.IsNVBanHang)
                return "📋 Chức năng của bạn:\n" +
                       "  • Kiểm tra tình trạng hàng hoá trong kho\n" +
                       "  • Xác nhận đơn hàng của khách và gửi đến kế toán\n\n" +
                       "Chọn menu bên trên để bắt đầu làm việc.";

            if (UserSession.IsKeToan)
                return "💼 Chức năng của bạn:\n" +
                       "  • Lập hóa đơn bán hàng từ đơn hàng đã xác nhận\n" +
                       "  • Cập nhật trạng thái thanh toán\n" +
                       "  • Thông báo kết quả cho khách hàng\n" +
                       "  • Cập nhật số lượng tồn kho sau khi hoàn tất đơn\n\n" +
                       "Chọn menu bên trên để bắt đầu làm việc.";

            if (UserSession.IsQuanLy)
                return "🏆 Chức năng của bạn:\n" +
                       "  • Quản lý thông tin sản phẩm và danh mục\n" +
                       "  • Lập phiếu nhập hàng khi tồn kho dưới mức quy định\n" +
                       "  • Theo dõi hoạt động nhập hàng và bán hàng\n" +
                       "  • Thống kê báo cáo doanh thu, tồn kho, sản phẩm bán chạy\n\n" +
                       "Chọn menu bên trên để bắt đầu làm việc.";

            return "Chào mừng đến với hệ thống.";
        }

        // ═══════════════════════════════════════════════════════════════
        // SỰ KIỆN MENU - ĐIỀU HƯỚNG ĐẾN CÁC FORM CON
        // ═══════════════════════════════════════════════════════════════

        // ── Hàng hoá ────────────────────────────────────────────────
        private void menuHangHoa_XemDanhSach_Click(object sender, EventArgs e)
            => MoFormTrong(new frmSanPham());

        private void menuHangHoa_Them_Click(object sender, EventArgs e)
            => MoFormTrong(new frmSanPham(cheBoDuyet: true));

        // ── Nhập hàng ──────────────────────────────────────────────
        private void menuNhapHang_Click(object? sender, EventArgs e)
            => MoFormTrong(new frmNhapHang());

        // ── Đơn hàng ────────────────────────────────────────────────
        private void menuDonHang_XemDanhSach_Click(object sender, EventArgs e)
            => MoFormTrong(new frmDonHang());

        private void menuDonHang_XacNhan_Click(object sender, EventArgs e)
        {
            if (!UserSession.IsNVBanHang && !UserSession.IsQuanLy)
            {
                MessageBox.Show("Bạn không có quyền xác nhận đơn hàng.",
                    "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MoFormTrong(new frmDonHang(cheBoDuyet: true));
        }

        // ── Hóa đơn ─────────────────────────────────────────────────
        private void menuHoaDon_Click(object? sender, EventArgs e)
            => MoFormTrong(new frmHoaDon());

        // ── Tồn kho ──────────────────────────────────────────────────
        private void menuTonKho_Click(object? sender, EventArgs e)
            => MoFormTrong(new frmTonKho());

        // ── Báo cáo ──────────────────────────────────────────────────
        private void menuBaoCao_DoanhThu_Click(object sender, EventArgs e)
            => MoFormTrong(new frmBaoCao(loaiBaoCao: "DoanhThu"));

        private void menuBaoCao_TonKho_Click(object sender, EventArgs e)
            => MoFormTrong(new frmBaoCao(loaiBaoCao: "TonKho"));

        private void menuBaoCao_BanChay_Click(object sender, EventArgs e)
            => MoFormTrong(new frmBaoCao(loaiBaoCao: "BanChay"));

        // ── Nhân viên ────────────────────────────────────────────────
        private void menuNhanVien_Click(object? sender, EventArgs e)
            => MoFormTrong(new frmNhanVien());

        // ── Cài đặt / Danh mục ───────────────────────────────────────
        private void menuCaiDat_Click(object? sender, EventArgs e)
            => MoFormTrong(new frmDanhMuc());

        // ── Đăng xuất ────────────────────────────────────────────────
        private void menuDangXuat_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Bạn có chắc muốn đăng xuất?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                this.Close(); // → Trigger FormClosed → quay về frmDangNhap
        }

        // ═══════════════════════════════════════════════════════════════
        // TIỆN ÍCH: MỞ FORM CON TRONG PANEL NỘI DUNG (MDI-LITE)
        // ═══════════════════════════════════════════════════════════════
        private void MoFormTrong(Form formCon)
        {
            panelContent.Controls.Clear();

            formCon.TopLevel = false;
            formCon.FormBorderStyle = FormBorderStyle.None;
            formCon.Dock = DockStyle.Fill;

            panelContent.Controls.Add(formCon);
            panelContent.Tag = formCon;
            formCon.Show();
        }
    }
}
