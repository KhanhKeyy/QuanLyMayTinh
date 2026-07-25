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
            this.panelTop = new Panel();
            this.lblChaoMung = new Label();
            this.btnDangXuat = new Button();
            this.panelMain = new Panel();

            this.panelTop.SuspendLayout();
            this.SuspendLayout();

            // ── panelTop ─────────────────────────────────────────────
            this.panelTop.Dock = DockStyle.Top;
            this.panelTop.Height = 60;
            this.panelTop.BackColor = Color.FromArgb(30, 41, 59);
            this.panelTop.Padding = new Padding(20, 0, 20, 0);
            this.panelTop.Controls.Add(this.lblChaoMung);
            this.panelTop.Controls.Add(this.btnDangXuat);

            // ── lblChaoMung ──────────────────────────────────────────
            this.lblChaoMung.AutoSize = false;
            this.lblChaoMung.Size = new Size(500, 60);
            this.lblChaoMung.Location = new Point(20, 0);
            this.lblChaoMung.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblChaoMung.ForeColor = Color.FromArgb(99, 179, 237);
            this.lblChaoMung.TextAlign = ContentAlignment.MiddleLeft;
            this.lblChaoMung.Text = "Chào mừng!";
            this.lblChaoMung.Name = "lblChaoMung";

            // ── btnDangXuat ──────────────────────────────────────────
            this.btnDangXuat.Text = "🚪 Đăng xuất";
            this.btnDangXuat.Size = new Size(110, 36);
            this.btnDangXuat.Location = new Point(1060, 12);
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
            this.panelMain.Padding = new Padding(10);
            this.panelMain.Name = "panelMain";

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

            this.panelTop.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private Panel panelTop = null!;
        private Label lblChaoMung = null!;
        private Button btnDangXuat = null!;
        private Panel panelMain = null!;
    }
}
