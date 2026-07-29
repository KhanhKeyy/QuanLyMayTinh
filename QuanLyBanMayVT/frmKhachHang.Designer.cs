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
            this.panelTop.Height = 55;
            this.panelTop.BackColor = Color.White;
            this.panelTop.Padding = new Padding(20, 0, 20, 0);
            this.panelTop.Controls.Add(this.lblChaoMung);
            this.panelTop.Controls.Add(this.btnDangXuat);
            this.panelTop.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(229, 231, 235), 1),
                    0, panelTop.Height - 1, panelTop.Width, panelTop.Height - 1);

            // ── lblChaoMung ──────────────────────────────────────────
            this.lblChaoMung.AutoSize = false;
            this.lblChaoMung.Size = new Size(500, 55);
            this.lblChaoMung.Location = new Point(20, 0);
            this.lblChaoMung.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            this.lblChaoMung.ForeColor = Color.FromArgb(17, 24, 39);
            this.lblChaoMung.TextAlign = ContentAlignment.MiddleLeft;
            this.lblChaoMung.Text = "Chào mừng!";
            this.lblChaoMung.Name = "lblChaoMung";

            // ── btnDangXuat ──────────────────────────────────────────
            this.btnDangXuat.Text = "🚪 Đăng xuất";
            this.btnDangXuat.Size = new Size(110, 34);
            this.btnDangXuat.Location = new Point(1060, 10);
            this.btnDangXuat.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnDangXuat.BackColor = Color.FromArgb(254, 242, 242);
            this.btnDangXuat.ForeColor = Color.FromArgb(220, 38, 38);
            this.btnDangXuat.FlatStyle = FlatStyle.Flat;
            this.btnDangXuat.FlatAppearance.BorderColor = Color.FromArgb(252, 165, 165);
            this.btnDangXuat.FlatAppearance.BorderSize = 1;
            this.btnDangXuat.Cursor = Cursors.Hand;
            this.btnDangXuat.Click += btnDangXuat_Click;
            this.btnDangXuat.Name = "btnDangXuat";

            // ── panelMain ────────────────────────────────────────────
            this.panelMain.Dock = DockStyle.Fill;
            this.panelMain.BackColor = Color.FromArgb(248, 250, 252);
            this.panelMain.Padding = new Padding(10);
            this.panelMain.Name = "panelMain";

            // ── frmKhachHang ─────────────────────────────────────────
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(1200, 700);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTop);
            this.BackColor = Color.FromArgb(248, 250, 252);
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
