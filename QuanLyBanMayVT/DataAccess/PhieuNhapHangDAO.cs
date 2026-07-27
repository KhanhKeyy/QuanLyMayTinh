using System.Data.SqlClient;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT.DataAccess
{
    public class PhieuNhapHangDAO
    {
        private static PhieuNhapHang MapRow(SqlDataReader r) => new PhieuNhapHang
        {
            MaPhieuNhap = (int)r["MaPhieuNhap"],
            MaQuanLy    = (int)r["MaQuanLy"],
            NgayNhap    = (DateTime)r["NgayNhap"],
            TrangThai   = (string)r["TrangThai"],
            TenQuanLy   = r.GetOrdinal("TenQuanLy") >= 0 && !r.IsDBNull(r.GetOrdinal("TenQuanLy"))
                          ? (string)r["TenQuanLy"] : ""
        };

        public List<PhieuNhapHang> GetAll()
        {
            var list = new List<PhieuNhapHang>();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    SELECT pn.*, nv.HoTen AS TenQuanLy
                    FROM PhieuNhapHang pn
                    LEFT JOIN NhanVien nv ON pn.MaQuanLy = nv.MaNhanVien
                    ORDER BY pn.NgayNhap DESC";
                using var cmd = new SqlCommand(sql, conn);
                using var r = cmd.ExecuteReader();
                while (r.Read()) list.Add(MapRow(r));
            }
            catch (Exception ex) { ShowError(ex); }
            return list;
        }

        /// <summary>Tạo phiếu nhập kèm chi tiết. Trả về MaPhieuNhap mới.</summary>
        public int Insert(PhieuNhapHang phieu, List<ChiTietPhieuNhap> chiTiet)
        {
            using var conn = DatabaseHelper.GetConnection();
            using var tran = conn.BeginTransaction();
            try
            {
                var cmdPN = new SqlCommand(@"
                    INSERT INTO PhieuNhapHang (MaQuanLy, NgayNhap, TrangThai)
                    OUTPUT INSERTED.MaPhieuNhap
                    VALUES (@mql, GETDATE(), 'Cho kiem tra')", conn, tran);
                cmdPN.Parameters.AddWithValue("@mql", phieu.MaQuanLy);
                int maPN = (int)cmdPN.ExecuteScalar()!;

                foreach (var ct in chiTiet)
                {
                    var cmdCT = new SqlCommand(@"
                        INSERT INTO ChiTietPhieuNhap (MaPhieuNhap, MaSanPham, SoLuongNhap, DonGiaNhap)
                        VALUES (@mpn, @msp, @sl, @dg)", conn, tran);
                    cmdCT.Parameters.AddWithValue("@mpn", maPN);
                    cmdCT.Parameters.AddWithValue("@msp", ct.MaSanPham);
                    cmdCT.Parameters.AddWithValue("@sl",  ct.SoLuongNhap);
                    cmdCT.Parameters.AddWithValue("@dg",  ct.DonGiaNhap);
                    cmdCT.ExecuteNonQuery();
                }
                tran.Commit();

                // ── Tự động tạo thông báo ────────────────────────────
                ThongBaoDAO.GuiThongBaoChoBanQuanLy("Phieu nhap hang",
                    $"🚚 Phiếu nhập hàng mới #{maPN} vừa được lập và đang chờ kiểm tra.", null);

                return maPN;
            }
            catch (Exception ex) { tran.Rollback(); ShowError(ex); return -1; }
        }

        public List<ChiTietPhieuNhap> GetChiTiet(int maPhieuNhap)
        {
            var list = new List<ChiTietPhieuNhap>();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    SELECT ct.*, sp.TenSanPham
                    FROM ChiTietPhieuNhap ct
                    LEFT JOIN SanPham sp ON ct.MaSanPham = sp.MaSanPham
                    WHERE ct.MaPhieuNhap = @id";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", maPhieuNhap);
                using var r = cmd.ExecuteReader();
                while (r.Read())
                    list.Add(new ChiTietPhieuNhap
                    {
                        MaChiTiet   = (int)r["MaChiTiet"],
                        MaPhieuNhap = (int)r["MaPhieuNhap"],
                        MaSanPham   = (int)r["MaSanPham"],
                        SoLuongNhap = (int)r["SoLuongNhap"],
                        DonGiaNhap  = (decimal)r["DonGiaNhap"],
                        TenSanPham  = r["TenSanPham"] as string ?? ""
                    });
            }
            catch (Exception ex) { ShowError(ex); }
            return list;
        }

        /// <summary>Duyệt phiếu nhập: cập nhật tồn kho + đổi TrangThai.</summary>
        public bool DuyetPhieu(int maPhieuNhap)
        {
            using var conn = DatabaseHelper.GetConnection();
            using var tran = conn.BeginTransaction();
            try
            {
                // Cập nhật tồn kho
                var cmdTon = new SqlCommand(@"
                    UPDATE sp SET sp.SoLuongTon = sp.SoLuongTon + ct.SoLuongNhap,
                        sp.TrangThai = 'Con hang'
                    FROM SanPham sp
                    INNER JOIN ChiTietPhieuNhap ct ON sp.MaSanPham = ct.MaSanPham
                    WHERE ct.MaPhieuNhap = @id AND sp.TrangThai <> 'Ngung KD'", conn, tran);
                cmdTon.Parameters.AddWithValue("@id", maPhieuNhap);
                cmdTon.ExecuteNonQuery();

                // Đổi trạng thái phiếu
                var cmdPN = new SqlCommand(
                    "UPDATE PhieuNhapHang SET TrangThai = 'Da nhap kho' WHERE MaPhieuNhap = @id", conn, tran);
                cmdPN.Parameters.AddWithValue("@id", maPhieuNhap);
                cmdPN.ExecuteNonQuery();

                tran.Commit();

                // ── Tự động tạo thông báo ────────────────────────────
                ThongBaoDAO.GuiThongBaoChoBanQuanLy("Phieu nhap hang",
                    $"✅ Phiếu nhập hàng #{maPhieuNhap} đã được duyệt và cập nhật số lượng vào kho!", null);

                return true;
            }
            catch (Exception ex) { tran.Rollback(); ShowError(ex); return false; }
        }

        private static void ShowError(Exception ex) =>
            MessageBox.Show($"Lỗi CSDL:\n{ex.Message}", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
