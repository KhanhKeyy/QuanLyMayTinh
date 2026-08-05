using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    public partial class frmThongBao : Form
    {
        private DataGridView dgvThongBao = null!;
        private Label lblThongKe = null!;
        private Button btnDanhDauDaDoc = null!;
        private int _maTaiKhoan;
        private List<ThongBao> _currentList = new();

        public frmThongBao()
        {
            InitializeComponent();
            _maTaiKhoan = UserSession.CurrentAccount?.MaTaiKhoan ?? 0;
        }

        private void InitializeComponent()
        {
            this.Text = "🔔 Danh Sách Thông Báo";
            this.Size = new Size(880, 540);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = UIStyleHelper.BgMain;
            this.ForeColor = Color.FromArgb(17, 24, 39);
            this.Font = new Font("Segoe UI", 9.5F);

            // ── Top Panel ─────────────────────────────────────────────
            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = UIStyleHelper.BgCard
            };

            lblThongKe = new Label
            {
                Text = "🔔 Đang tải thông báo...",
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Location = new Point(20, 18)
            };

            btnDanhDauDaDoc = new Button
            {
                Text = "✅ Đánh dấu tất cả đã đọc",
                AutoSize = true,
                Padding = new Padding(14, 6, 14, 6),
                Height = 34,
                Location = new Point(0, 15),
                BackColor = UIStyleHelper.PrimaryBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDanhDauDaDoc.FlatAppearance.BorderSize = 0;
            btnDanhDauDaDoc.Click += BtnDanhDauDaDoc_Click;

            pnlTop.Controls.Add(lblThongKe);
            pnlTop.Controls.Add(btnDanhDauDaDoc);
            pnlTop.Resize += (s, e) =>
            {
                btnDanhDauDaDoc.Top = 15;
                btnDanhDauDaDoc.Left = pnlTop.ClientSize.Width - btnDanhDauDaDoc.Width - 20;
            };

            // ── Grid Thông Báo ────────────────────────────────────────
            dgvThongBao = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvThongBao);
            dgvThongBao.CellContentClick += DgvThongBao_CellContentClick;

            this.Controls.Add(dgvThongBao);
            this.Controls.Add(pnlTop);

            this.Load += (s, e) => TaiDanhSachThongBao();
        }

        public void TaiDanhSachThongBao()
        {
            if (_maTaiKhoan == 0) return;

            _currentList = new ThongBaoDAO().GetByMaTaiKhoan(_maTaiKhoan);
            int chuaDoc = _currentList.Count(t => !t.DaDoc);

            lblThongKe.Text = chuaDoc > 0
                ? $"🔔 Có {chuaDoc} thông báo chưa đọc"
                : "🔔 Không có thông báo mới";

            dgvThongBao.Columns.Clear();

            dgvThongBao.DataSource = _currentList.Select(t => new
            {
                t.MaThongBao,
                Loai = t.LoaiDisplay,
                NoiDung = t.NoiDung,
                NgayTao = t.NgayTao.ToString("dd/MM/yyyy HH:mm"),
                TrangThai = t.DaDoc ? "✅ Đã đọc" : "🔴 Chưa đọc"
            }).ToList();

            // 1. Mã TB (Nới rộng lên 80px để tiêu đề không bị nhảy 2 dòng, Căn giữa Tiêu đề và Ô)
            if (dgvThongBao.Columns["MaThongBao"] != null)
            {
                var col = dgvThongBao.Columns["MaThongBao"];
                col.HeaderText = "Mã TB";
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                col.Width = 80;
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // 2. Loại Thông Báo (Căn trái cả Tiêu đề và Ô)
            if (dgvThongBao.Columns["Loai"] != null)
            {
                var col = dgvThongBao.Columns["Loai"];
                col.HeaderText = "Loại Thông Báo";
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }

            // 3. Nội Dung (Căn trái cả Tiêu đề và Ô)
            if (dgvThongBao.Columns["NoiDung"] != null)
            {
                var col = dgvThongBao.Columns["NoiDung"];
                col.HeaderText = "Nội Dung";
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }

            // 4. Thời Gian (Căn giữa cả Tiêu đề và Ô)
            if (dgvThongBao.Columns["NgayTao"] != null)
            {
                var col = dgvThongBao.Columns["NgayTao"];
                col.HeaderText = "Thời Gian";
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                col.Width = 140;
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // 5. Trạng Thái (Căn giữa cả Tiêu đề và Ô)
            if (dgvThongBao.Columns["TrangThai"] != null)
            {
                var col = dgvThongBao.Columns["TrangThai"];
                col.HeaderText = "Trạng Thái";
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                col.Width = 115;
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // 6. Cột nút Xem Chi Tiết / Đọc (Căn giữa cả Tiêu đề và Ô)
            var btnColDoc = new DataGridViewButtonColumn
            {
                Name = "colDoc",
                HeaderText = "Xem Nội Dung",
                Text = "👁️ Đọc",
                UseColumnTextForButtonValue = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 135,
                HeaderCell = { Style = { Alignment = DataGridViewContentAlignment.MiddleCenter } },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(12, 10, 12, 10)
                }
            };
            dgvThongBao.Columns.Add(btnColDoc);

            // 7. Cột nút Duyệt Đề Xuất cho Quản Lý (Căn giữa cả Tiêu đề và Ô)
            if (UserSession.IsQuanLy)
            {
                var btnColDuyet = new DataGridViewButtonColumn
                {
                    Name = "colDuyet",
                    HeaderText = "Duyệt Đề Xuất",
                    Text = "📋 Duyệt đề xuất",
                    UseColumnTextForButtonValue = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Width = 145,
                    HeaderCell = { Style = { Alignment = DataGridViewContentAlignment.MiddleCenter } },
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleCenter,
                        Padding = new Padding(8, 10, 8, 10)
                    }
                };
                dgvThongBao.Columns.Add(btnColDuyet);
            }

            if (btnDanhDauDaDoc.Parent != null)
            {
                btnDanhDauDaDoc.Left = btnDanhDauDaDoc.Parent.ClientSize.Width - btnDanhDauDaDoc.Width - 20;
            }
        }

        private void DgvThongBao_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _currentList.Count) return;

            string colName = dgvThongBao.Columns[e.ColumnIndex].Name;
            var selectedTB = _currentList[e.RowIndex];

            if (colName == "colDoc")
            {
                // Mở cửa sổ Chi tiết Thông báo
                using var dlg = new frmChiTietThongBao(selectedTB);
                dlg.ShowDialog();

                // ĐÁNH DẤU ĐÃ ĐỌC SAU KHI ĐÓNG CỬA SỔ
                new ThongBaoDAO().DanhDauDaDoc(selectedTB.MaThongBao);
                TaiDanhSachThongBao();

                if (Application.OpenForms["frmMain"] is frmMain mainForm)
                {
                    mainForm.CapNhatBadgeThongBao();
                }
            }
            else if (colName == "colDuyet")
            {
                // Quản lý nhấn nút Duyệt Đề Xuất trong bảng thông báo
                using var dlg = new frmKiemDuyetDeXuat();
                dlg.ShowDialog();
                TaiDanhSachThongBao();

                if (Application.OpenForms["frmMain"] is frmMain mainForm)
                {
                    mainForm.CapNhatBadgeThongBao();
                }
            }
        }

        private void BtnDanhDauDaDoc_Click(object? sender, EventArgs e)
        {
            if (_maTaiKhoan == 0) return;
            var list = new ThongBaoDAO().GetByMaTaiKhoan(_maTaiKhoan, chiChuaDoc: true);
            var dao = new ThongBaoDAO();
            foreach (var tb in list)
            {
                dao.DanhDauDaDoc(tb.MaThongBao);
            }
            TaiDanhSachThongBao();

            if (Application.OpenForms["frmMain"] is frmMain mainForm)
            {
                mainForm.CapNhatBadgeThongBao();
            }
        }
    }
}
