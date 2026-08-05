using System.Data.SqlClient;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT.DataAccess
{
    public class HoaDonDAO
    {
        private static HoaDon MapRow(SqlDataReader r) => new HoaDon
        {
            MaHoaDon           = (int)r["MaHoaDon"],
            MaDonHang          = (int)r["MaDonHang"],
            MaKeToan           = (int)r["MaKeToan"],
            NgayLapHoaDon      = (DateTime)r["NgayLapHoaDon"],
            TongTien           = (decimal)r["TongTien"],
            TrangThaiThanhToan = (string)r["TrangThaiThanhToan"],
            NgayThanhToan      = r["NgayThanhToan"] as DateTime?,
            TenKeToan          = r.GetOrdinal("TenKeToan") >= 0 && !r.IsDBNull(r.GetOrdinal("TenKeToan"))
                                 ? (string)r["TenKeToan"] : "",
            TenKhachHang       = r.GetOrdinal("TenKhachHang") >= 0 && !r.IsDBNull(r.GetOrdinal("TenKhachHang"))
                                 ? (string)r["TenKhachHang"] : "",
            MaKhachHang        = r.GetOrdinal("MaKhachHang") >= 0 && !r.IsDBNull(r.GetOrdinal("MaKhachHang"))
                                 ? (int)r["MaKhachHang"] : 0
        };

        private const string SelectJoin = @"
            SELECT hd.*,
                   nv.HoTen AS TenKeToan,
                   kh.HoTen AS TenKhachHang,
                   kh.MaKhachHang
            FROM HoaDon hd
            LEFT JOIN NhanVien nv ON hd.MaKeToan = nv.MaNhanVien
            LEFT JOIN DonHang dh ON hd.MaDonHang = dh.MaDonHang
            LEFT JOIN KhachHang kh ON dh.MaKhachHang = kh.MaKhachHang";

        public List<HoaDon> GetAll()
        {
            var list = new List<HoaDon>();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                using var cmd = new SqlCommand(SelectJoin + " ORDER BY hd.MaHoaDon DESC", conn);
                using var r = cmd.ExecuteReader();
                while (r.Read()) list.Add(MapRow(r));
            }
            catch (Exception ex) { ShowError(ex); }
            return list;
        }

        public HoaDon? GetById(int maHoaDon)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                using var cmd = new SqlCommand(SelectJoin + " WHERE hd.MaHoaDon = @id", conn);
                cmd.Parameters.AddWithValue("@id", maHoaDon);
                using var r = cmd.ExecuteReader();
                return r.Read() ? MapRow(r) : null;
            }
            catch (Exception ex) { ShowError(ex); return null; }
        }

        public HoaDon? GetByMaDonHang(int maDonHang)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                using var cmd = new SqlCommand(SelectJoin + " WHERE hd.MaDonHang = @id", conn);
                cmd.Parameters.AddWithValue("@id", maDonHang);
                using var r = cmd.ExecuteReader();
                return r.Read() ? MapRow(r) : null;
            }
            catch (Exception ex) { ShowError(ex); return null; }
        }

        /// <summary>Lập hóa đơn từ đơn hàng đã xác nhận. Trả về MaHoaDon mới.</summary>
        public int Insert(HoaDon hd)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    INSERT INTO HoaDon (MaDonHang, MaKeToan, NgayLapHoaDon, TongTien, TrangThaiThanhToan)
                    OUTPUT INSERTED.MaHoaDon
                    VALUES (@mdh, @mkt, GETDATE(), @tt, 'Chua thanh toan')";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@mdh", hd.MaDonHang);
                cmd.Parameters.AddWithValue("@mkt", hd.MaKeToan);
                cmd.Parameters.AddWithValue("@tt",  hd.TongTien);
                return (int)cmd.ExecuteScalar()!;
            }
            catch (Exception ex) { ShowError(ex); return -1; }
        }

        /// <summary>Cập nhật trạng thái thanh toán và cập nhật tồn kho khi 'Da thanh toan'.</summary>
        public bool CapNhatTrangThaiThanhToan(int maHoaDon, string trangThai)
        {
            using var conn = DatabaseHelper.GetConnection();
            using var tran = conn.BeginTransaction();
            try
            {
                // Lấy MaDonHang
                var cmdGet = new SqlCommand("SELECT MaDonHang FROM HoaDon WHERE MaHoaDon = @id", conn, tran);
                cmdGet.Parameters.AddWithValue("@id", maHoaDon);
                int maDonHang = (int)cmdGet.ExecuteScalar()!;

                // Cập nhật trạng thái hóa đơn
                string ngayTT = trangThai == "Da thanh toan" ? ", NgayThanhToan = GETDATE()" : "";
                var cmdHD = new SqlCommand(
                    $"UPDATE HoaDon SET TrangThaiThanhToan = @tt{ngayTT} WHERE MaHoaDon = @id", conn, tran);
                cmdHD.Parameters.AddWithValue("@tt", trangThai);
                cmdHD.Parameters.AddWithValue("@id", maHoaDon);
                cmdHD.ExecuteNonQuery();

                if (trangThai == "Da thanh toan")
                {
                    // Cập nhật trạng thái đơn hàng → Hoàn tất
                    var cmdDH = new SqlCommand(
                        "UPDATE DonHang SET TrangThaiDonHang = 'Hoan tat' WHERE MaDonHang = @mdh", conn, tran);
                    cmdDH.Parameters.AddWithValue("@mdh", maDonHang);
                    cmdDH.ExecuteNonQuery();

                    // Giảm tồn kho theo từng chi tiết đơn hàng
                    var cmdCT = new SqlCommand(@"
                        UPDATE sp SET sp.SoLuongTon = sp.SoLuongTon - ct.SoLuong,
                            sp.TrangThai = CASE WHEN (sp.SoLuongTon - ct.SoLuong) <= 0 THEN 'Het hang' ELSE sp.TrangThai END
                        FROM SanPham sp
                        INNER JOIN ChiTietDonHang ct ON sp.MaSanPham = ct.MaSanPham
                        WHERE ct.MaDonHang = @mdh AND sp.TrangThai <> 'Ngung KD'", conn, tran);
                    cmdCT.Parameters.AddWithValue("@mdh", maDonHang);
                    cmdCT.ExecuteNonQuery();

                    // Gửi thông báo cho khách hàng
                    var cmdKH = new SqlCommand(@"
                        SELECT tk.MaTaiKhoan FROM TaiKhoan tk
                        INNER JOIN KhachHang kh ON tk.MaTaiKhoan = kh.MaTaiKhoan
                        INNER JOIN DonHang dh ON kh.MaKhachHang = dh.MaKhachHang
                        WHERE dh.MaDonHang = @mdh", conn, tran);
                    cmdKH.Parameters.AddWithValue("@mdh", maDonHang);
                    var maTK = cmdKH.ExecuteScalar();
                    if (maTK != null)
                    {
                        var cmdTB = new SqlCommand(@"
                            INSERT INTO ThongBao (LoaiThongBao, NoiDung, NgayTao, MaTaiKhoanNhan, MaDonHang, DaDoc)
                            VALUES ('Ket qua don hang', N'Đơn hàng #' + CAST(@mdh AS NVARCHAR) + N' đã thanh toán thành công. Cảm ơn bạn!',
                                    GETDATE(), @mtk, @mdh, 0)", conn, tran);
                        cmdTB.Parameters.AddWithValue("@mdh", maDonHang);
                        cmdTB.Parameters.AddWithValue("@mtk", maTK);
                        cmdTB.ExecuteNonQuery();
                    }
                }

                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                ShowError(ex);
                return false;
            }
        }

        private static void ShowError(Exception ex) =>
            MessageBox.Show($"Lỗi CSDL:\n{ex.Message}", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
