using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    public class frmChiTietThongBao : Form
    {
        private readonly ThongBao _thongBao;

        public frmChiTietThongBao(ThongBao thongBao)
        {
            _thongBao = thongBao;
            InitUI();
        }

        private void InitUI()
        {
            this.Text = "🔔 Chi tiết thông báo";
            this.Size = new Size(540, 420);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = UIStyleHelper.BgMain;
            this.Font = new Font("Segoe UI", 9.5F);

            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24),
                ColumnCount = 2,
                RowCount = 5,
                AutoSize = true
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            int r = 0;

            // Header Title
            var lblTitle = new Label
            {
                Text = "🔔 NỘI DUNG THÔNG BÁO",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 15)
            };
            tbl.Controls.Add(lblTitle, 0, r);
            tbl.SetColumnSpan(lblTitle, 2);

            // 1. Loại thông báo
            r++;
            tbl.Controls.Add(CreateLabel("Loại thông báo:"), 0, r);
            var lblLoai = new Label
            {
                Text = _thongBao.LoaiDisplay,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };
            tbl.Controls.Add(lblLoai, 1, r);

            // 2. Thời gian
            r++;
            tbl.Controls.Add(CreateLabel("Thời gian:"), 0, r);
            var lblTime = new Label
            {
                Text = _thongBao.NgayTao.ToString("dd/MM/yyyy HH:mm:ss"),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };
            tbl.Controls.Add(lblTime, 1, r);

            // 3. Nội dung thông báo
            r++;
            tbl.Controls.Add(CreateLabel("Nội dung:"), 0, r);
            var txtNoiDung = new TextBox
            {
                Text = _thongBao.NoiDung ?? "",
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                Height = 150,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(17, 24, 39),
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };
            tbl.Controls.Add(txtNoiDung, 1, r);

            // 4. Buttons
            r++;
            var pnlBtns = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Margin = new Padding(0, 18, 0, 0)
            };

            var btnDong = new Button
            {
                Text = "Đóng",
                Size = new Size(95, 36),
                BackColor = Color.FromArgb(226, 232, 240),
                ForeColor = Color.FromArgb(51, 65, 85),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDong.FlatAppearance.BorderSize = 0;
            btnDong.Click += (s, e) => this.Close();
            pnlBtns.Controls.Add(btnDong);

            // Nếu là Quản lý & Thông báo là Đề xuất SP -> Nút duyệt nhanh
            if (UserSession.IsQuanLy && (_thongBao.LoaiThongBao == "De xuat sp" || (_thongBao.NoiDung ?? "").Contains("Đề xuất")))
            {
                var btnDuyetDeXuat = new Button
                {
                    Text = "📋 Kiểm duyệt đề xuất này",
                    AutoSize = true,
                    Padding = new Padding(12, 0, 12, 0),
                    Height = 36,
                    Margin = new Padding(0, 0, 10, 0),
                    BackColor = UIStyleHelper.SuccessGreen,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnDuyetDeXuat.FlatAppearance.BorderSize = 0;
                btnDuyetDeXuat.Click += (s, e) =>
                {
                    using var dlg = new frmKiemDuyetDeXuat();
                    dlg.ShowDialog();
                };
                pnlBtns.Controls.Add(btnDuyetDeXuat);
            }

            tbl.Controls.Add(pnlBtns, 0, r);
            tbl.SetColumnSpan(pnlBtns, 2);

            this.Controls.Add(tbl);
        }

        private Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                Margin = new Padding(0, 4, 0, 0),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105)
            };
        }
    }
}
