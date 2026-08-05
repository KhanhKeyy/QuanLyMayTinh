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
    public class frmGioHang : Form
    {
        private readonly KhachHang _khachHang;
        private DataGridView dgvGioHang = null!;
        private ComboBox cboPTTT = null!;
        private TextBox txtGhiChu = null!;
        private Label lblTongTien = null!;
        private Button btnDatHang = null!;
        private Button btnXoaTatCa = null!;
        private Button btnTangSL = null!;
        private Button btnGiamSL = null!;
        private Button btnXoaItem = null!;

        public frmGioHang(KhachHang khachHang)
        {
            _khachHang = khachHang;
            InitUI();
            LoadData();
        }

        private void InitUI()
        {
            this.Text = "🛒 Giỏ Hàng Của Bạn";
            this.Size = new Size(860, 580);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = UIStyleHelper.BgMain;
            this.Font = new Font("Segoe UI", 9.5F);

            // ── TOP PANEL ───────────────────────────────────────────────
            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = UIStyleHelper.BgCard,
                Padding = new Padding(15, 0, 15, 0)
            };

            var lblHeader = new Label
            {
                Text = "🛒 GIỎ HÀNG THUẬN TIỆN — Xem lại & Quản lý các món hàng đã chọn",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlTop.Controls.Add(lblHeader);

            // ── BOTTOM PANEL (Thanh toán & Đặt hàng) ─────────────────────
            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 150,
                BackColor = UIStyleHelper.BgCard,
                Padding = new Padding(20, 12, 20, 12)
            };

            lblTongTien = new Label
            {
                Text = "Tổng tiền: 0 đ (0 sản phẩm)",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(16, 185, 129),
                AutoSize = true,
                Location = new Point(20, 15)
            };

            var lblPT = new Label
            {
                Text = "Phương thức TT:",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                Location = new Point(20, 55),
                AutoSize = true
            };

            cboPTTT = new ComboBox
            {
                Location = new Point(145, 52),
                Width = 240,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            UIStyleHelper.StyleComboBox(cboPTTT);

            var lblGC = new Label
            {
                Text = "Ghi chú giao hàng:",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                Location = new Point(410, 15),
                AutoSize = true
            };

            txtGhiChu = new TextBox
            {
                Location = new Point(410, 42),
                Width = 410,
                Height = 45,
                Multiline = true,
                PlaceholderText = "Nhập địa chỉ giao hàng chi tiết, yêu cầu đặc biệt..."
            };
            UIStyleHelper.StyleTextBox(txtGhiChu);

            btnDatHang = new Button
            {
                Text = "🚀 ĐẶT HÀNG TẤT CẢ",
                Size = new Size(200, 38),
                Location = new Point(410, 96),
                BackColor = UIStyleHelper.PrimaryBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDatHang.FlatAppearance.BorderSize = 0;
            btnDatHang.Click += BtnDatHang_Click;

            btnXoaTatCa = new Button
            {
                Text = "🗑️ Xóa tất cả",
                Size = new Size(130, 38),
                Location = new Point(620, 96),
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnXoaTatCa.FlatAppearance.BorderSize = 0;
            btnXoaTatCa.Click += (s, e) =>
            {
                if (GioHangManager.Items.Count == 0) return;
                if (MessageBox.Show("Xóa toàn bộ sản phẩm trong giỏ hàng?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    GioHangManager.XoaTatCa();
                    LoadData();
                }
            };

            var btnDong = new Button
            {
                Text = "Đóng",
                Size = new Size(80, 38),
                Location = new Point(760, 96),
                BackColor = Color.FromArgb(226, 232, 240),
                ForeColor = Color.FromArgb(51, 65, 85),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDong.FlatAppearance.BorderSize = 0;
            btnDong.Click += (s, e) => this.Close();

            pnlBottom.Controls.Add(lblTongTien);
            pnlBottom.Controls.Add(lblPT);
            pnlBottom.Controls.Add(cboPTTT);
            pnlBottom.Controls.Add(lblGC);
            pnlBottom.Controls.Add(txtGhiChu);
            pnlBottom.Controls.Add(btnDatHang);
            pnlBottom.Controls.Add(btnXoaTatCa);
            pnlBottom.Controls.Add(btnDong);

            // ── ACTION PANEL (Tăng/Giảm/Xóa dòng) ──────────────────────
            var pnlActions = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 48,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(15, 8, 15, 8)
            };

            var flowAct = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            btnTangSL = new Button
            {
                Text = "➕ Tăng SL (+1)",
                AutoSize = true,
                Padding = new Padding(10, 0, 10, 0),
                Height = 32,
                Margin = new Padding(0, 0, 10, 0),
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnTangSL.FlatAppearance.BorderSize = 0;
            btnTangSL.Click += BtnTangSL_Click;

            btnGiamSL = new Button
            {
                Text = "➖ Giảm SL (-1)",
                AutoSize = true,
                Padding = new Padding(10, 0, 10, 0),
                Height = 32,
                Margin = new Padding(0, 0, 10, 0),
                BackColor = Color.FromArgb(245, 158, 11),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGiamSL.FlatAppearance.BorderSize = 0;
            btnGiamSL.Click += BtnGiamSL_Click;

            btnXoaItem = new Button
            {
                Text = "🗑️ Xóa món này",
                AutoSize = true,
                Padding = new Padding(10, 0, 10, 0),
                Height = 32,
                Margin = new Padding(0, 0, 0, 0),
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnXoaItem.FlatAppearance.BorderSize = 0;
            btnXoaItem.Click += BtnXoaItem_Click;

            flowAct.Controls.Add(btnTangSL);
            flowAct.Controls.Add(btnGiamSL);
            flowAct.Controls.Add(btnXoaItem);
            pnlActions.Controls.Add(flowAct);

            // ── DATAGRIDVIEW ───────────────────────────────────────────
            dgvGioHang = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvGioHang);

            this.Controls.Add(dgvGioHang);
            this.Controls.Add(pnlActions);
            this.Controls.Add(pnlBottom);
            this.Controls.Add(pnlTop);
        }

        private void LoadData()
        {
            var ptttList = new PhuongThucThanhToanDAO().GetAll();
            cboPTTT.DataSource = ptttList;
            cboPTTT.DisplayMember = "TenPhuongThuc";
            cboPTTT.ValueMember = "MaPhuongThucTT";

            RenderCart();
        }

        private void RenderCart()
        {
            var items = GioHangManager.Items;

            dgvGioHang.DataSource = items.Select(i => new
            {
                i.SanPham.MaSanPham,
                i.SanPham.TenSanPham,
                DonGia = i.SanPham.GiaBanFormatted,
                i.SoLuong,
                ThanhTien = i.ThanhTienFormatted
            }).ToList();

            if (dgvGioHang.Columns["MaSanPham"] != null)
            {
                var col = dgvGioHang.Columns["MaSanPham"];
                col.HeaderText = "Mã SP";
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                col.Width = 70;
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (dgvGioHang.Columns["TenSanPham"] != null) dgvGioHang.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";
            if (dgvGioHang.Columns["DonGia"] != null) dgvGioHang.Columns["DonGia"].HeaderText = "Đơn Giá";
            if (dgvGioHang.Columns["SoLuong"] != null)
            {
                var col = dgvGioHang.Columns["SoLuong"];
                col.HeaderText = "Số Lượng";
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                col.Width = 100;
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (dgvGioHang.Columns["ThanhTien"] != null) dgvGioHang.Columns["ThanhTien"].HeaderText = "Thành Tiền";

            lblTongTien.Text = $"Tổng tiền: {GioHangManager.TongTienFormatted} ({GioHangManager.TongSoLuong} sản phẩm)";
            btnDatHang.Enabled = items.Count > 0;
            btnXoaTatCa.Enabled = items.Count > 0;
            btnTangSL.Enabled = items.Count > 0;
            btnGiamSL.Enabled = items.Count > 0;
            btnXoaItem.Enabled = items.Count > 0;
        }

        private void BtnTangSL_Click(object? sender, EventArgs e)
        {
            if (dgvGioHang.CurrentRow == null) return;
            int maSP = (int)dgvGioHang.CurrentRow.Cells["MaSanPham"].Value;
            var item = GioHangManager.Items.FirstOrDefault(i => i.SanPham.MaSanPham == maSP);
            if (item != null)
            {
                GioHangManager.CapNhatSoLuong(maSP, item.SoLuong + 1);
                RenderCart();
            }
        }

        private void BtnGiamSL_Click(object? sender, EventArgs e)
        {
            if (dgvGioHang.CurrentRow == null) return;
            int maSP = (int)dgvGioHang.CurrentRow.Cells["MaSanPham"].Value;
            var item = GioHangManager.Items.FirstOrDefault(i => i.SanPham.MaSanPham == maSP);
            if (item != null)
            {
                GioHangManager.CapNhatSoLuong(maSP, item.SoLuong - 1);
                RenderCart();
            }
        }

        private void BtnXoaItem_Click(object? sender, EventArgs e)
        {
            if (dgvGioHang.CurrentRow == null) return;
            int maSP = (int)dgvGioHang.CurrentRow.Cells["MaSanPham"].Value;
            GioHangManager.XoaKhoiGio(maSP);
            RenderCart();
        }

        private void BtnDatHang_Click(object? sender, EventArgs e)
        {
            var items = GioHangManager.Items;
            if (items.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống! Vui lòng chọn sản phẩm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboPTTT.SelectedItem is not PhuongThucThanhToan selectedPT)
            {
                MessageBox.Show("Vui lòng chọn phương thức thanh toán.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra tồn kho cho từng sản phẩm trong giỏ
            foreach (var item in items)
            {
                var spReal = new SanPhamDAO().GetById(item.SanPham.MaSanPham);
                if (spReal == null || spReal.SoLuongTon < item.SoLuong)
                {
                    MessageBox.Show($"⚠️ Rất tiếc, sản phẩm '{item.SanPham.TenSanPham}' chỉ còn {spReal?.SoLuongTon ?? 0} cái trong kho!\nVui lòng giảm số lượng mua.", "Tồn kho không đủ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            var dh = new DonHang
            {
                MaKhachHang = _khachHang.MaKhachHang,
                MaPhuongThucTT = selectedPT.MaPhuongThucTT,
                GhiChu = txtGhiChu.Text.Trim()
            };

            var ctList = items.Select(i => new ChiTietDonHang
            {
                MaSanPham = i.SanPham.MaSanPham,
                SoLuong = i.SoLuong,
                DonGia = i.SanPham.GiaBan
            }).ToList();

            int newDonHangId = new DonHangDAO().Insert(dh, ctList);
            if (newDonHangId > 0)
            {
                GioHangManager.XoaTatCa();
                MessageBox.Show($"🎉 Đặt hàng thành công!\nMã đơn hàng của bạn là #{newDonHangId}.\nHệ thống đã gửi đơn hàng tới Bộ phận Bán hàng để xử lý.",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
