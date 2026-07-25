using System.Data.SqlClient;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT.DataAccess
{
    public class ThongBaoDAO
    {
        public List<ThongBao> GetByMaTaiKhoan(int maTaiKhoan, bool chiChuaDoc = false)
        {
            var list = new List<ThongBao>();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                string sql = @"SELECT * FROM ThongBao WHERE MaTaiKhoanNhan = @mtk";
                if (chiChuaDoc) sql += " AND DaDoc = 0";
                sql += " ORDER BY NgayTao DESC";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@mtk", maTaiKhoan);
                using var r = cmd.ExecuteReader();
                while (r.Read())
                    list.Add(new ThongBao
                    {
                        MaThongBao      = (int)r["MaThongBao"],
                        LoaiThongBao    = (string)r["LoaiThongBao"],
                        NoiDung         = r["NoiDung"] as string,
                        NgayTao         = (DateTime)r["NgayTao"],
                        MaTaiKhoanNhan  = r["MaTaiKhoanNhan"] as int?,
                        MaSanPham       = r["MaSanPham"] as int?,
                        MaDonHang       = r["MaDonHang"] as int?,
                        DaDoc           = (bool)r["DaDoc"]
                    });
            }
            catch (Exception ex) { ShowError(ex); }
            return list;
        }

        public int DemChuaDoc(int maTaiKhoan)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                using var cmd = new SqlCommand(
                    "SELECT COUNT(1) FROM ThongBao WHERE MaTaiKhoanNhan = @mtk AND DaDoc = 0", conn);
                cmd.Parameters.AddWithValue("@mtk", maTaiKhoan);
                return (int)cmd.ExecuteScalar()!;
            }
            catch { return 0; }
        }

        public bool Insert(ThongBao tb)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    INSERT INTO ThongBao (LoaiThongBao, NoiDung, NgayTao, MaTaiKhoanNhan, MaSanPham, MaDonHang, DaDoc)
                    VALUES (@loai, @nd, GETDATE(), @mtk, @msp, @mdh, 0)";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@loai", tb.LoaiThongBao);
                cmd.Parameters.AddWithValue("@nd",   (object?)tb.NoiDung ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@mtk",  (object?)tb.MaTaiKhoanNhan ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@msp",  (object?)tb.MaSanPham ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@mdh",  (object?)tb.MaDonHang ?? DBNull.Value);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex) { ShowError(ex); return false; }
        }

        public bool DanhDauDaDoc(int maThongBao)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                using var cmd = new SqlCommand(
                    "UPDATE ThongBao SET DaDoc = 1 WHERE MaThongBao = @id", conn);
                cmd.Parameters.AddWithValue("@id", maThongBao);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch { return false; }
        }

        private static void ShowError(Exception ex) =>
            MessageBox.Show($"Lỗi CSDL:\n{ex.Message}", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
