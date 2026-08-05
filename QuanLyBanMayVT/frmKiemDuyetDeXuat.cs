using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    public class frmKiemDuyetDeXuat : Form
    {
        private DataGridView dgvDeXuat = null!;
        private ComboBox cboFilterTrangThai = null!;
        private Button btnDuyet = null!;
        private Button btnTuChoi = null!;
        private Label lblCount = null!;

        public frmKiemDuyetDeXuat()
        {
            InitUI();
            LoadData();
        }

        private void InitUI()
        {
            this.Text = "📋 Kiểm duyệt đề xuất thêm sản phẩm (Quản lý)";
            this.Size = new Size(920, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = UIStyleHelper.BgMain;
            this.Font = new Font("Segoe UI", 9.5F);

            // ── TOP PANEL ───────────────────────────────────────────────
            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                Padding = new Padding(15, 10, 15, 10),
                BackColor = UIStyleHelper.BgCard
            };

            var flowTop = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = UIStyleHelper.BgCard
            };

            lblCount = new Label
            {
                Text = "Đề xuất chờ duyệt: 0",
                AutoSize = true,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235),
                Margin = new Padding(0, 6, 20, 0)
            };

            var lblFilter = new Label
            {
                Text = "Trạng thái:",
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F),
                Margin = new Padding(0, 6, 8, 0)
            };

            cboFilterTrangThai = new ComboBox
            {
                Width = 140,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 2, 25, 0)
            };
            UIStyleHelper.StyleComboBox(cboFilterTrangThai);
            cboFilterTrangThai.Items.AddRange(new object[] { "Cho duyet", "Da duyet", "Tu choi", "Tất cả" });
            cboFilterTrangThai.SelectedIndex = 0;
            cboFilterTrangThai.SelectedIndexChanged += (s, e) => LoadData();

            btnDuyet = new Button
            {
                Text = "✅ Duyệt & Thêm sản phẩm",
                AutoSize = true,
                Padding = new Padding(12, 0, 12, 0),
                Height = 34,
                Margin = new Padding(0, 0, 10, 0),
                BackColor = UIStyleHelper.SuccessGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDuyet.FlatAppearance.BorderSize = 0;
            btnDuyet.Click += BtnDuyet_Click;

            btnTuChoi = new Button
            {
                Text = "❌ Từ chối đề xuất",
                AutoSize = true,
                Padding = new Padding(12, 0, 12, 0),
                Height = 34,
                Margin = new Padding(0, 0, 0, 0),
                BackColor = UIStyleHelper.DangerRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnTuChoi.FlatAppearance.BorderSize = 0;
            btnTuChoi.Click += BtnTuChoi_Click;

            flowTop.Controls.Add(lblCount);
            flowTop.Controls.Add(lblFilter);
            flowTop.Controls.Add(cboFilterTrangThai);
            flowTop.Controls.Add(btnDuyet);
            flowTop.Controls.Add(btnTuChoi);

            pnlTop.Controls.Add(flowTop);

            // ── DATAGRIDVIEW ───────────────────────────────────────────
            dgvDeXuat = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvDeXuat);

            this.Controls.Add(dgvDeXuat);
            this.Controls.Add(pnlTop);
        }

        private void LoadData()
        {
            string filter = cboFilterTrangThai.SelectedItem?.ToString() ?? "Cho duyet";
            string? filterParam = filter == "Tất cả" ? null : filter;

            var list = new YeuCauThemSanPhamDAO().GetAll(filterParam);
            int choDuyetCount = new YeuCauThemSanPhamDAO().DemChoDuyet();
            lblCount.Text = $"Đề xuất chờ duyệt: {choDuyetCount}";

            dgvDeXuat.DataSource = list.Select(x => new
            {
                x.MaYeuCau,
                x.TenSanPham,
                x.TenDanhMuc,
                x.GiaBanFormatted,
                x.SoLuongTon,
                x.MucTonToiThieu,
                x.TenNhanVienDeXuat,
                NgayGui = x.NgayDeXuat.ToString("dd/MM/yyyy HH:mm"),
                TrangThai = x.TrangThaiDisplay,
                x.LyDoDeXuat
            }).ToList();

            if (dgvDeXuat.Columns["MaYeuCau"] != null) dgvDeXuat.Columns["MaYeuCau"].HeaderText = "Mã YC";
            if (dgvDeXuat.Columns["TenSanPham"] != null) dgvDeXuat.Columns["TenSanPham"].HeaderText = "Tên SP Đề Xuất";
            if (dgvDeXuat.Columns["TenDanhMuc"] != null) dgvDeXuat.Columns["TenDanhMuc"].HeaderText = "Danh Mục";
            if (dgvDeXuat.Columns["GiaBanFormatted"] != null) dgvDeXuat.Columns["GiaBanFormatted"].HeaderText = "Giá Đề Xuất";
            if (dgvDeXuat.Columns["SoLuongTon"] != null) dgvDeXuat.Columns["SoLuongTon"].HeaderText = "Số Lượng";
            if (dgvDeXuat.Columns["MucTonToiThieu"] != null) dgvDeXuat.Columns["MucTonToiThieu"].HeaderText = "Tối Thiểu";
            if (dgvDeXuat.Columns["TenNhanVienDeXuat"] != null) dgvDeXuat.Columns["TenNhanVienDeXuat"].HeaderText = "NV Đề Xuất";
            if (dgvDeXuat.Columns["NgayGui"] != null) dgvDeXuat.Columns["NgayGui"].HeaderText = "Ngày Gửi";
            if (dgvDeXuat.Columns["TrangThai"] != null) dgvDeXuat.Columns["TrangThai"].HeaderText = "Trạng Thái";
            if (dgvDeXuat.Columns["LyDoDeXuat"] != null) dgvDeXuat.Columns["LyDoDeXuat"].HeaderText = "Lý Do Đề Xuất";
        }

        private void BtnDuyet_Click(object? sender, EventArgs e)
        {
            if (dgvDeXuat.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn đề xuất cần duyệt.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int maYC = (int)dgvDeXuat.CurrentRow.Cells["MaYeuCau"].Value;
            string tenSP = dgvDeXuat.CurrentRow.Cells["TenSanPham"].Value?.ToString() ?? "";

            var res = MessageBox.Show($"Xác nhận PHÊ DUYỆT đề xuất #{maYC} ('{tenSP}') và tự động thêm vào Danh mục Sản phẩm?",
                "Xác nhận phê duyệt", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (res == DialogResult.Yes)
            {
                var currentNV = UserSession.CurrentNhanVien;
                int maNV = currentNV != null ? currentNV.MaNhanVien : 1;

                if (new YeuCauThemSanPhamDAO().DuyetYeuCau(maYC, maNV))
                {
                    MessageBox.Show($"🎉 Đã phê duyệt đề xuất #{maYC} thành công!\nSản phẩm '{tenSP}' đã chính thức được thêm vào hệ thống.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
            }
        }

        private void BtnTuChoi_Click(object? sender, EventArgs e)
        {
            if (dgvDeXuat.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn đề xuất cần từ chối.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int maYC = (int)dgvDeXuat.CurrentRow.Cells["MaYeuCau"].Value;
            string tenSP = dgvDeXuat.CurrentRow.Cells["TenSanPham"].Value?.ToString() ?? "";

            string lyDo = Microsoft.VisualBasic.Interaction.InputBox(
                $"Nhập lý do từ chối đề xuất #{maYC} ('{tenSP}'):",
                "Từ chối đề xuất", "Không phù hợp danh mục kinh doanh hiện tại");

            if (string.IsNullOrEmpty(lyDo)) return;

            var currentNV = UserSession.CurrentNhanVien;
            int maNV = currentNV != null ? currentNV.MaNhanVien : 1;

            if (new YeuCauThemSanPhamDAO().TuChoiYeuCau(maYC, maNV, lyDo))
            {
                MessageBox.Show($"Đã từ chối đề xuất #{maYC}.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
        }
    }
}
