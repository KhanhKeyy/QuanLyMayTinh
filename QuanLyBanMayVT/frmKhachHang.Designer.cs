namespace QuanLyBanMayVT
{
    partial class frmKhachHang
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.panelTop     = new Panel();
            this.lblChaoMung  = new Label();
            this.btnDangXuat  = new Button();
            this.lblSearch    = new Label();
            this.txtTimKiem   = new TextBox();
            this.panelMain    = new Panel();
            this.lblPlaceholder = new Label();
            this.btnDatHang   = new Button();

            this.panelTop.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();

            // ── panelTop ─────────────────────────────────────────────
            this.panelTop.Dock = DockStyle.Top;
            this.panelTop.Height = 80;
            this.panelTop.BackColor = Color.FromArgb(30, 41, 59);
            this.panelTop.Padding = new Padding(20, 0, 20, 0);
            this.panelTop.Controls.Add(this.lblChaoMung);
            this.panelTop.Controls.Add(this.btnDangXuat);
            this.panelTop.Controls.Add(this.lblSearch);
            this.panelTop.Controls.Add(this.txtTimKiem);

            // ── lblChaoMung ──────────────────────────────────────────
            this.lblChaoMung.AutoSize = false;
            this.lblChaoMung.Size = new Size(400, 80);
            this.lblChaoMung.Location = new Point(20, 0);
            this.lblChaoMung.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblChaoMung.ForeColor = Color.FromArgb(99, 179, 237);
            this.lblChaoMung.TextAlign = ContentAlignment.MiddleLeft;
            this.lblChaoMung.Text = "Chào mừng!";
            this.lblChaoMung.Name = "lblChaoMung";

            // ── txtTimKiem ───────────────────────────────────────────
            this.txtTimKiem.Size = new Size(260, 32);
            this.txtTimKiem.Location = new Point(430, 24);
            this.txtTimKiem.Font = new Font("Segoe UI", 10F);
            this.txtTimKiem.BackColor = Color.FromArgb(51, 65, 85);
            this.txtTimKiem.ForeColor = Color.White;
            this.txtTimKiem.BorderStyle = BorderStyle.FixedSingle;
            this.txtTimKiem.PlaceholderText = "🔍 Tìm kiếm sản phẩm...";
            this.txtTimKiem.Name = "txtTimKiem";

            // ── lblSearch - spacer ───────────────────────────────────
            this.lblSearch.AutoSize = false;
            this.lblSearch.Size = new Size(1, 1);
            this.lblSearch.Location = new Point(420, 24);
            this.lblSearch.Name = "lblSearch";

            // ── btnDangXuat ──────────────────────────────────────────
            this.btnDangXuat.Text = "🚪 Đăng xuất";
            this.btnDangXuat.Size = new Size(110, 36);
            this.btnDangXuat.Location = new Point(1060, 22);
            this.btnDangXuat.Font = new Font("Segoe UI", 9F);
            this.btnDangXuat.BackColor = Color.FromArgb(127, 29, 29);
            this.btnDangXuat.ForeColor = Color.White;
            this.btnDangXuat.FlatStyle = FlatStyle.Flat;
            this.btnDangXuat.FlatAppearance.BorderSize = 0;
            this.btnDangXuat.Cursor = Cursors.Hand;
            this.btnDangXuat.Click += btnDangXuat_Click;
            this.btnDangXuat.Name = "btnDangXuat";

            // ── panelMain ────────────────────────────────────────────
            this.panelMain.Dock = DockStyle.Fill;
            this.panelMain.BackColor = Color.FromArgb(15, 23, 42);
            this.panelMain.Padding = new Padding(20);
            this.panelMain.AutoScroll = true;
            this.panelMain.Controls.Add(this.lblPlaceholder);
            this.panelMain.Controls.Add(this.btnDatHang);

            // ── lblPlaceholder ───────────────────────────────────────
            this.lblPlaceholder.Text =
                "🖥️  Danh sách sản phẩm sẽ hiển thị ở đây\n\n" +
                "Bạn có thể xem thông tin sản phẩm, cấu hình, giá bán\nvà tình trạng còn hàng.";
            this.lblPlaceholder.Font = new Font("Segoe UI", 12F);
            this.lblPlaceholder.ForeColor = Color.FromArgb(100, 116, 139);
            this.lblPlaceholder.TextAlign = ContentAlignment.MiddleCenter;
            this.lblPlaceholder.AutoSize = false;
            this.lblPlaceholder.Size = new Size(800, 200);
            this.lblPlaceholder.Location = new Point(150, 80);
            this.lblPlaceholder.Name = "lblPlaceholder";

            // ── btnDatHang ───────────────────────────────────────────
            this.btnDatHang.Text = "🛒  Đặt mua sản phẩm đã chọn";
            this.btnDatHang.Size = new Size(300, 50);
            this.btnDatHang.Location = new Point(350, 320);
            this.btnDatHang.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.btnDatHang.BackColor = Color.FromArgb(59, 130, 246);
            this.btnDatHang.ForeColor = Color.White;
            this.btnDatHang.FlatStyle = FlatStyle.Flat;
            this.btnDatHang.FlatAppearance.BorderSize = 0;
            this.btnDatHang.Cursor = Cursors.Hand;
            this.btnDatHang.Click += btnDatHang_Click;
            this.btnDatHang.Name = "btnDatHang";

            // ── frmKhachHang ─────────────────────────────────────────
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1200, 700);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTop);
            this.BackColor = Color.FromArgb(15, 23, 42);
            this.Name = "frmKhachHang";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Text = "Cửa hàng máy vi tính";
            this.Load += frmKhachHang_Load;

            this.panelMain.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private Panel panelTop = null!;
        private Label lblChaoMung = null!;
        private Label lblSearch = null!;
        private TextBox txtTimKiem = null!;
        private Button btnDangXuat = null!;
        private Panel panelMain = null!;
        private Label lblPlaceholder = null!;
        private Button btnDatHang = null!;
    }
}
