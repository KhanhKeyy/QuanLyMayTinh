using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.DataAccess;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT
{
    public class frmDatHang : Form
    {
        private readonly SanPham _sanPham;
        private readonly KhachHang _khachHang;

        private NumericUpDown numSoLuong = null!;
        private ComboBox cboPTTT = null!;
        private TextBox txtGhiChu = null!;
        private Label lblTongTien = null!;
        private Button btnXacNhan = null!;
        private Button btnHuy = null!;

        public frmDatHang(SanPham sanPham, KhachHang khachHang)
        {
            _sanPham = sanPham;
            _khachHang = khachHang;
            InitComponent();
            LoadData();
        }

        private void InitComponent()
        {
            this.Text = "Xác nhận đặt hàng";
            this.Size = new Size(580, 530);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = UIStyleHelper.BgMain;
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10F);

            var lblHeader = new Label
            {
                Text = "🛒 XÁC NHẬN ĐẶT HÀNG",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(99, 179, 237),
                Dock = DockStyle.Top,
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = UIStyleHelper.BgCard
            };

            var panelBody = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(25),
                BackColor = UIStyleHelper.BgMain
            };

            var lblSPInfo = new Label
            {
                Text = $"Sản phẩm: {_sanPham.TenSanPham}\nĐơn giá: {_sanPham.GiaBanFormatted}\nTồn kho hiện tại: {_sanPham.SoLuongTon}",
                Location = new Point(25, 15),
                AutoSize = true,
                ForeColor = Color.FromArgb(226, 232, 240),
                Font = new Font("Segoe UI", 10.5F)
            };

            // Dùng vị trí Y rõ ràng, khoảng cách lớn giữa các dòng
            int labelX = 25;
            int inputX = 170;
            int inputWidth = 340;

            int y1 = 95;
            int y2 = 145;
            int y3 = 195;
            int y4 = 295;
            int y5 = 355;

            var lblSL = new Label { Text = "Số lượng mua:", Location = new Point(labelX, y1 + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted };
            numSoLuong = new NumericUpDown
            {
                Location = new Point(inputX, y1),
                Width = 150,
                Minimum = 1,
                Maximum = Math.Max(1, _sanPham.SoLuongTon),
                Value = 1
            };
            UIStyleHelper.StyleNumeric(numSoLuong);
            numSoLuong.ValueChanged += (s, e) => TinhTongTien();

            var lblPT = new Label { Text = "Phương thức TT:", Location = new Point(labelX, y2 + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted };
            cboPTTT = new ComboBox
            {
                Location = new Point(inputX, y2),
                Width = inputWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            UIStyleHelper.StyleComboBox(cboPTTT);

            var lblGC = new Label { Text = "Ghi chú giao hàng:", Location = new Point(labelX, y3 + 3), AutoSize = true, ForeColor = UIStyleHelper.TextMuted };
            txtGhiChu = new TextBox
            {
                Location = new Point(inputX, y3),
                Width = inputWidth,
                Height = 80,
                Multiline = true
            };
            UIStyleHelper.StyleTextBox(txtGhiChu);

            lblTongTien = new Label
            {
                Text = "Tổng tiền: 0 đ",
                Location = new Point(labelX, y4),
                AutoSize = true,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 211, 153)
            };

            btnXacNhan = new Button
            {
                Text = "🛒 Xác nhận Đặt hàng",
                Location = new Point(inputX, y5),
                AutoSize = true,
                Padding = new Padding(15, 8, 15, 8),
                BackColor = UIStyleHelper.PrimaryBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnXacNhan.FlatAppearance.BorderSize = 0;
            btnXacNhan.Click += BtnXacNhan_Click;

            btnHuy = new Button
            {
                Text = "Hủy bỏ",
                Location = new Point(inputX + 200, y5),
                AutoSize = true,
                Padding = new Padding(20, 8, 20, 8),
                BackColor = Color.FromArgb(100, 116, 139),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand
            };
            btnHuy.FlatAppearance.BorderSize = 0;
            btnHuy.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            panelBody.Controls.Add(lblSPInfo);
            panelBody.Controls.Add(lblSL);
            panelBody.Controls.Add(numSoLuong);
            panelBody.Controls.Add(lblPT);
            panelBody.Controls.Add(cboPTTT);
            panelBody.Controls.Add(lblGC);
            panelBody.Controls.Add(txtGhiChu);
            panelBody.Controls.Add(lblTongTien);
            panelBody.Controls.Add(btnXacNhan);
            panelBody.Controls.Add(btnHuy);

            this.Controls.Add(panelBody);
            this.Controls.Add(lblHeader);
        }

        private void LoadData()
        {
            var ptttList = new PhuongThucThanhToanDAO().GetAll();
            cboPTTT.DataSource = ptttList;
            cboPTTT.DisplayMember = "TenPhuongThuc";
            cboPTTT.ValueMember = "MaPhuongThucTT";
            TinhTongTien();
        }

        private void TinhTongTien()
        {
            decimal tong = _sanPham.GiaBan * numSoLuong.Value;
            lblTongTien.Text = $"Tổng tiền: {tong:N0} đ";
        }

        private void BtnXacNhan_Click(object? sender, EventArgs e)
        {
            if (cboPTTT.SelectedItem is not PhuongThucThanhToan selectedPT)
            {
                MessageBox.Show("Vui lòng chọn phương thức thanh toán.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int soLuong = (int)numSoLuong.Value;
            if (soLuong > _sanPham.SoLuongTon)
            {
                MessageBox.Show("Số lượng mua vượt quá số lượng tồn kho!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dh = new DonHang
            {
                MaKhachHang = _khachHang.MaKhachHang,
                MaPhuongThucTT = selectedPT.MaPhuongThucTT,
                GhiChu = txtGhiChu.Text.Trim()
            };

            var ctList = new List<ChiTietDonHang>
            {
                new ChiTietDonHang
                {
                    MaSanPham = _sanPham.MaSanPham,
                    SoLuong = soLuong,
                    DonGia = _sanPham.GiaBan
                }
            };

            int maDH = new DonHangDAO().Insert(dh, ctList);
            if (maDH > 0)
            {
                MessageBox.Show($"Đặt hàng thành công!\nMã đơn hàng của bạn: #{maDH}\nNhân viên sẽ sớm xác nhận đơn hàng.",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
