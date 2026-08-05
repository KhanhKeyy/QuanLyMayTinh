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
    /// <summary>
    /// Form Quản lý & Cập nhật số lượng Tồn kho (Kèm phân trang cho tất cả các Role)
    /// </summary>
    public class frmTonKho : Form
    {
        private DataGridView dgvTonKho = null!;
        private Button btnCapNhat = null!;
        private NumericUpDown numSoLuongTon = null!;
        private Label lblSelectedSP = null!;
        private int _selectedId = 0;

        // Phân trang
        private int _currentPage = 1;
        private const int PageSize = 10;
        private List<SanPham> _fullList = new();

        private Panel pnlPagination = null!;
        private Label lblPageInfo = null!;
        private Button btnPrev = null!;
        private Button btnNext = null!;
        private FlowLayoutPanel pnlPageNumbers = null!;

        public frmTonKho()
        {
            InitUI();
            LoadData();
        }

        private void InitUI()
        {
            this.Text = "Tình trạng tồn kho";
            this.BackColor = UIStyleHelper.BgMain;
            this.ForeColor = Color.FromArgb(17, 24, 39);
            this.Font = new Font("Segoe UI", 9.5F);

            // ── TOP PANEL (Cập nhật tồn kho) ──────────────────────────
            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = UIStyleHelper.BgCard
            };

            var flowTop = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(15, 12, 15, 10),
                BackColor = UIStyleHelper.BgCard
            };

            lblSelectedSP = new Label
            {
                Text = "Sản phẩm chọn: (chưa chọn)",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235),
                Margin = new Padding(0, 6, 25, 0)
            };

            var lblSL = new Label
            {
                Text = "Số lượng tồn mới:",
                AutoSize = true,
                ForeColor = Color.FromArgb(17, 24, 39),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Margin = new Padding(0, 6, 8, 0)
            };

            numSoLuongTon = new NumericUpDown
            {
                Width = 110,
                Maximum = 10000,
                Margin = new Padding(0, 2, 20, 0)
            };
            UIStyleHelper.StyleNumeric(numSoLuongTon);

            btnCapNhat = new Button
            {
                Text = "💾 Cập nhật tồn kho",
                Size = new Size(170, 34),
                Margin = new Padding(0, 0, 0, 0),
                BackColor = UIStyleHelper.SuccessGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCapNhat.FlatAppearance.BorderSize = 0;
            btnCapNhat.Click += BtnCapNhat_Click;

            flowTop.Controls.Add(lblSelectedSP);
            flowTop.Controls.Add(lblSL);
            flowTop.Controls.Add(numSoLuongTon);
            flowTop.Controls.Add(btnCapNhat);

            pnlTop.Controls.Add(flowTop);
            if (UserSession.IsKeToan)
            {
                pnlTop.Visible = false;
            }

            // ── DATAGRIDVIEW ───────────────────────────────────────────
            dgvTonKho = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            UIStyleHelper.StyleDataGridView(dgvTonKho);
            dgvTonKho.SelectionChanged += DgvTonKho_SelectionChanged;

            // ── PAGINATION BAR (Bottom) ───────────────────────────────
            pnlPagination = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 55,
                BackColor = UIStyleHelper.BgCard,
                Padding = new Padding(15, 10, 15, 10)
            };
            pnlPagination.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240), 1), 0, 0, pnlPagination.Width, 0);

            lblPageInfo = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(15, 18)
            };

            btnPrev = new Button
            {
                Text = "◀ Trước",
                Size = new Size(80, 32),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPrev.Click += (s, e) => { if (_currentPage > 1) { _currentPage--; RenderPage(); } };

            btnNext = new Button
            {
                Text = "Sau ▶",
                Size = new Size(80, 32),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnNext.Click += (s, e) => {
                int totalPages = _fullList.Count == 0 ? 1 : (int)Math.Ceiling((double)_fullList.Count / PageSize);
                if (_currentPage < totalPages) { _currentPage++; RenderPage(); }
            };

            pnlPageNumbers = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent
            };

            pnlPagination.Controls.Add(lblPageInfo);
            pnlPagination.Controls.Add(btnPrev);
            pnlPagination.Controls.Add(pnlPageNumbers);
            pnlPagination.Controls.Add(btnNext);
            pnlPagination.Resize += (s, e) => RepositionPaginationControls();

            this.Controls.Add(dgvTonKho);
            this.Controls.Add(pnlPagination);
            this.Controls.Add(pnlTop);
        }

        private void RepositionPaginationControls()
        {
            if (pnlPagination == null) return;
            int rightX = pnlPagination.ClientSize.Width - 15;
            btnNext.Left = rightX - btnNext.Width;
            btnNext.Top = 11;

            pnlPageNumbers.Left = btnNext.Left - pnlPageNumbers.Width - 6;
            pnlPageNumbers.Top = 11;

            btnPrev.Left = pnlPageNumbers.Left - btnPrev.Width - 6;
            btnPrev.Top = 11;
        }

        private void LoadData()
        {
            _fullList = new SanPhamDAO().GetAll();
            _currentPage = 1;
            RenderPage();
        }

        private void RenderPage()
        {
            int totalCount = _fullList.Count;
            int totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling((double)totalCount / PageSize);

            if (_currentPage > totalPages) _currentPage = totalPages;
            if (_currentPage < 1) _currentPage = 1;

            var pageItems = _fullList
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            dgvTonKho.DataSource = pageItems.Select(p => new
            {
                p.MaSanPham,
                p.TenSanPham,
                p.TenDanhMuc,
                p.SoLuongTon,
                p.MucTonToiThieu,
                CanhBao = p.DuoiMucToiThieu ? "⚠️ THẤP" : "OK",
                TrangThai = p.TrangThaiDisplay
            }).ToList();

            if (dgvTonKho.Columns["MaSanPham"] != null) dgvTonKho.Columns["MaSanPham"].HeaderText = "Mã SP";
            if (dgvTonKho.Columns["TenSanPham"] != null) dgvTonKho.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";
            if (dgvTonKho.Columns["TenDanhMuc"] != null) dgvTonKho.Columns["TenDanhMuc"].HeaderText = "Danh Mục";
            if (dgvTonKho.Columns["SoLuongTon"] != null) dgvTonKho.Columns["SoLuongTon"].HeaderText = "Số Lượng Tồn";
            if (dgvTonKho.Columns["MucTonToiThieu"] != null) dgvTonKho.Columns["MucTonToiThieu"].HeaderText = "Mức Tối Thiểu";
            if (dgvTonKho.Columns["CanhBao"] != null) dgvTonKho.Columns["CanhBao"].HeaderText = "Cảnh Báo";
            if (dgvTonKho.Columns["TrangThai"] != null) dgvTonKho.Columns["TrangThai"].HeaderText = "Trạng Thái";

            // Footer info & buttons
            int startIdx = totalCount == 0 ? 0 : (_currentPage - 1) * PageSize + 1;
            int endIdx = Math.Min(_currentPage * PageSize, totalCount);
            lblPageInfo.Text = $"Hiển thị {startIdx} - {endIdx} / Tổng {totalCount} sản phẩm (Trang {_currentPage}/{totalPages})";

            btnPrev.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < totalPages;

            btnPrev.BackColor = btnPrev.Enabled ? Color.White : Color.FromArgb(241, 245, 249);
            btnPrev.ForeColor = btnPrev.Enabled ? Color.FromArgb(30, 41, 59) : Color.FromArgb(148, 163, 184);
            btnPrev.FlatAppearance.BorderColor = btnPrev.Enabled ? Color.FromArgb(203, 213, 225) : Color.FromArgb(226, 232, 240);
            btnPrev.FlatAppearance.BorderSize = 1;

            btnNext.BackColor = btnNext.Enabled ? Color.White : Color.FromArgb(241, 245, 249);
            btnNext.ForeColor = btnNext.Enabled ? Color.FromArgb(30, 41, 59) : Color.FromArgb(148, 163, 184);
            btnNext.FlatAppearance.BorderColor = btnNext.Enabled ? Color.FromArgb(203, 213, 225) : Color.FromArgb(226, 232, 240);
            btnNext.FlatAppearance.BorderSize = 1;

            pnlPageNumbers.Controls.Clear();
            for (int i = 1; i <= totalPages; i++)
            {
                int pageNum = i;
                var btnNum = new Button
                {
                    Text = pageNum.ToString(),
                    Size = new Size(36, 32),
                    Margin = new Padding(3, 0, 3, 0),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };

                if (pageNum == _currentPage)
                {
                    btnNum.BackColor = UIStyleHelper.PrimaryBlue;
                    btnNum.ForeColor = Color.White;
                    btnNum.FlatAppearance.BorderColor = UIStyleHelper.PrimaryBlue;
                    btnNum.FlatAppearance.BorderSize = 1;
                }
                else
                {
                    btnNum.BackColor = Color.White;
                    btnNum.ForeColor = Color.FromArgb(30, 41, 59);
                    btnNum.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
                    btnNum.FlatAppearance.BorderSize = 1;
                }

                btnNum.Click += (s, e) =>
                {
                    _currentPage = pageNum;
                    RenderPage();
                };
                pnlPageNumbers.Controls.Add(btnNum);
            }

            RepositionPaginationControls();
        }

        private void DgvTonKho_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvTonKho.CurrentRow == null) return;
            _selectedId = (int)dgvTonKho.CurrentRow.Cells["MaSanPham"].Value;
            var sp = new SanPhamDAO().GetById(_selectedId);
            if (sp == null) return;

            lblSelectedSP.Text = $"Sản phẩm: {sp.TenSanPham}";
            numSoLuongTon.Value = sp.SoLuongTon;
        }

        private void BtnCapNhat_Click(object? sender, EventArgs e)
        {
            if (_selectedId <= 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần cập nhật tồn kho.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var sp = new SanPhamDAO().GetById(_selectedId);
            if (sp == null) return;

            int delta = (int)numSoLuongTon.Value - sp.SoLuongTon;
            if (new SanPhamDAO().CapNhatSoLuongTon(_selectedId, delta))
            {
                MessageBox.Show("Cập nhật số lượng tồn kho thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
        }
    }
}
