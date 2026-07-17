namespace QuanLyBanMayVT
{
    partial class frmDangNhap
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
            panelBackground = new Panel();
            panelCard = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            lblTenDangNhap = new Label();
            txtTenDangNhap = new TextBox();
            lblMatKhau = new Label();
            panelPassword = new Panel();
            txtMatKhau = new TextBox();
            btnTogglePassword = new Button();
            btnDangNhap = new Button();
            btnThoat = new Button();
            lblFooter = new Label();
            panelBackground.SuspendLayout();
            panelCard.SuspendLayout();
            panelPassword.SuspendLayout();
            SuspendLayout();
            // 
            // panelBackground
            // 
            panelBackground.BackColor = Color.FromArgb(15, 23, 42);
            panelBackground.Controls.Add(panelCard);
            panelBackground.Dock = DockStyle.Fill;
            panelBackground.Location = new Point(0, 0);
            panelBackground.Name = "panelBackground";
            panelBackground.Size = new Size(800, 520);
            panelBackground.TabIndex = 0;
            // 
            // panelCard
            // 
            panelCard.BackColor = Color.FromArgb(30, 41, 59);
            panelCard.Controls.Add(lblTitle);
            panelCard.Controls.Add(lblSubtitle);
            panelCard.Controls.Add(lblTenDangNhap);
            panelCard.Controls.Add(txtTenDangNhap);
            panelCard.Controls.Add(lblMatKhau);
            panelCard.Controls.Add(panelPassword);
            panelCard.Controls.Add(btnDangNhap);
            panelCard.Controls.Add(btnThoat);
            panelCard.Controls.Add(lblFooter);
            panelCard.Location = new Point(200, 10);
            panelCard.Name = "panelCard";
            panelCard.Size = new Size(400, 500);
            panelCard.TabIndex = 0;
            panelCard.Paint += PanelCard_Paint;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.ForeColor = Color.FromArgb(99, 179, 237);
            lblTitle.Location = new Point(0, 35);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(400, 45);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "🖥️ QuanLyBanMayVT";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblSubtitle
            // 
            lblSubtitle.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblSubtitle.ForeColor = Color.FromArgb(148, 163, 184);
            lblSubtitle.Location = new Point(0, 82);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(400, 22);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Hệ thống quản lý cửa hàng máy tính";
            lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblTenDangNhap
            // 
            lblTenDangNhap.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            lblTenDangNhap.ForeColor = Color.FromArgb(203, 213, 225);
            lblTenDangNhap.Location = new Point(40, 135);
            lblTenDangNhap.Name = "lblTenDangNhap";
            lblTenDangNhap.Size = new Size(140, 22);
            lblTenDangNhap.TabIndex = 2;
            lblTenDangNhap.Text = "Tên đăng nhập";
            // 
            // txtTenDangNhap
            // 
            txtTenDangNhap.BackColor = Color.FromArgb(51, 65, 85);
            txtTenDangNhap.BorderStyle = BorderStyle.FixedSingle;
            txtTenDangNhap.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            txtTenDangNhap.ForeColor = Color.White;
            txtTenDangNhap.Location = new Point(40, 162);
            txtTenDangNhap.Name = "txtTenDangNhap";
            txtTenDangNhap.PlaceholderText = "Nhập tên đăng nhập...";
            txtTenDangNhap.Size = new Size(320, 27);
            txtTenDangNhap.TabIndex = 3;
            // 
            // lblMatKhau
            // 
            lblMatKhau.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            lblMatKhau.ForeColor = Color.FromArgb(203, 213, 225);
            lblMatKhau.Location = new Point(40, 215);
            lblMatKhau.Name = "lblMatKhau";
            lblMatKhau.Size = new Size(100, 22);
            lblMatKhau.TabIndex = 4;
            lblMatKhau.Text = "Mật khẩu";
            // 
            // panelPassword
            // 
            panelPassword.BackColor = Color.FromArgb(51, 65, 85);
            panelPassword.Controls.Add(txtMatKhau);
            panelPassword.Controls.Add(btnTogglePassword);
            panelPassword.Location = new Point(40, 242);
            panelPassword.Name = "panelPassword";
            panelPassword.Size = new Size(320, 36);
            panelPassword.TabIndex = 5;
            // 
            // txtMatKhau
            // 
            txtMatKhau.BackColor = Color.FromArgb(51, 65, 85);
            txtMatKhau.BorderStyle = BorderStyle.None;
            txtMatKhau.Dock = DockStyle.Fill;
            txtMatKhau.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            txtMatKhau.ForeColor = Color.White;
            txtMatKhau.Location = new Point(0, 0);
            txtMatKhau.Name = "txtMatKhau";
            txtMatKhau.PasswordChar = '*';
            txtMatKhau.PlaceholderText = "Nhập mật khẩu...";
            txtMatKhau.Size = new Size(284, 20);
            txtMatKhau.TabIndex = 0;
            // 
            // btnTogglePassword
            // 
            btnTogglePassword.BackColor = Color.FromArgb(51, 65, 85);
            btnTogglePassword.Cursor = Cursors.Hand;
            btnTogglePassword.Dock = DockStyle.Right;
            btnTogglePassword.FlatAppearance.BorderSize = 0;
            btnTogglePassword.FlatStyle = FlatStyle.Flat;
            btnTogglePassword.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            btnTogglePassword.ForeColor = Color.FromArgb(148, 163, 184);
            btnTogglePassword.Location = new Point(284, 0);
            btnTogglePassword.Name = "btnTogglePassword";
            btnTogglePassword.Size = new Size(36, 36);
            btnTogglePassword.TabIndex = 1;
            btnTogglePassword.Text = "👁";
            btnTogglePassword.UseVisualStyleBackColor = false;
            btnTogglePassword.Click += btnTogglePassword_Click;
            // 
            // btnDangNhap
            // 
            btnDangNhap.BackColor = Color.FromArgb(59, 130, 246);
            btnDangNhap.Cursor = Cursors.Hand;
            btnDangNhap.FlatAppearance.BorderSize = 0;
            btnDangNhap.FlatStyle = FlatStyle.Flat;
            btnDangNhap.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btnDangNhap.ForeColor = Color.White;
            btnDangNhap.Location = new Point(40, 310);
            btnDangNhap.Name = "btnDangNhap";
            btnDangNhap.Size = new Size(320, 46);
            btnDangNhap.TabIndex = 6;
            btnDangNhap.Text = "Đăng nhập";
            btnDangNhap.UseVisualStyleBackColor = false;
            btnDangNhap.Click += btnDangNhap_Click;
            btnDangNhap.MouseEnter += btnDangNhap_MouseEnter;
            btnDangNhap.MouseLeave += btnDangNhap_MouseLeave;
            // 
            // btnThoat
            // 
            btnThoat.BackColor = Color.Transparent;
            btnThoat.Cursor = Cursors.Hand;
            btnThoat.FlatAppearance.BorderColor = Color.FromArgb(71, 85, 105);
            btnThoat.FlatStyle = FlatStyle.Flat;
            btnThoat.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            btnThoat.ForeColor = Color.FromArgb(148, 163, 184);
            btnThoat.Location = new Point(40, 368);
            btnThoat.Name = "btnThoat";
            btnThoat.Size = new Size(320, 40);
            btnThoat.TabIndex = 7;
            btnThoat.Text = "Thoát";
            btnThoat.UseVisualStyleBackColor = false;
            btnThoat.Click += btnThoat_Click;
            // 
            // lblFooter
            // 
            lblFooter.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            lblFooter.ForeColor = Color.FromArgb(71, 85, 105);
            lblFooter.Location = new Point(0, 455);
            lblFooter.Name = "lblFooter";
            lblFooter.Size = new Size(400, 30);
            lblFooter.TabIndex = 8;
            lblFooter.Text = "© 2025 QuanLyBanMayVT - Hệ thống quản lý cửa hàng";
            lblFooter.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // frmDangNhap
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 23, 42);
            ClientSize = new Size(800, 520);
            Controls.Add(panelBackground);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "frmDangNhap";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Đăng nhập - Quản lý bán máy vi tính";
            Load += frmDangNhap_Load;
            panelBackground.ResumeLayout(false);
            panelCard.ResumeLayout(false);
            panelCard.PerformLayout();
            panelPassword.ResumeLayout(false);
            panelPassword.PerformLayout();
            ResumeLayout(false);
        }



        #endregion

        // Controls
        private Panel panelBackground;
        private Panel panelCard;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblTenDangNhap;
        private TextBox txtTenDangNhap;
        private Label lblMatKhau;
        private Panel panelPassword;
        private TextBox txtMatKhau;
        private Button btnTogglePassword;
        private Button btnDangNhap;
        private Button btnThoat;
        private Label lblFooter;
    }
}
