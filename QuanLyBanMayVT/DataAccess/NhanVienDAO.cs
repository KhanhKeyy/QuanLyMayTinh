using System.Data.SqlClient;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT.DataAccess
{
    /// <summary>
    /// DAO xử lý các thao tác CSDL liên quan đến bảng NhanVien
    /// </summary>
    public class NhanVienDAO
    {
        private static NhanVien MapRow(SqlDataReader r) => new NhanVien
        {
            MaNhanVien  = (int)r["MaNhanVien"],
            HoTen       = (string)r["HoTen"],
            Email       = r["Email"] as string ?? "",
            SoDienThoai = r["SoDienThoai"] as string ?? "",
            ChucVu      = r["ChucVu"] as string ?? "",
            MaTaiKhoan  = (int)r["MaTaiKhoan"],
            NgayVaoLam  = r["NgayVaoLam"] as DateTime?
        };

        /// <summary>Lấy thông tin nhân viên theo MaTaiKhoan</summary>
        public NhanVien? GetByMaTaiKhoan(int maTaiKhoan)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    SELECT MaNhanVien, HoTen, Email, SoDienThoai, ChucVu, MaTaiKhoan, NgayVaoLam
                    FROM   NhanVien
                    WHERE  MaTaiKhoan = @MaTaiKhoan";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
                using var reader = cmd.ExecuteReader();
                return reader.Read() ? MapRow(reader) : null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải thông tin nhân viên:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>Lấy danh sách tất cả nhân viên</summary>
        public List<NhanVien> GetAll()
        {
            var list = new List<NhanVien>();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    SELECT MaNhanVien, HoTen, Email, SoDienThoai, ChucVu, MaTaiKhoan, NgayVaoLam
                    FROM   NhanVien
                    ORDER BY MaNhanVien";
                using var cmd    = new SqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read()) list.Add(MapRow(reader));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách nhân viên:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return list;
        }

        /// <summary>
        /// Thêm nhân viên mới kèm tạo TaiKhoan tự động.
        /// </summary>
        public bool Insert(NhanVien nv, string tenDangNhap, string matKhauHash)
        {
            using var conn = DatabaseHelper.GetConnection();
            using var tran = conn.BeginTransaction();
            try
            {
                var cmdTK = new SqlCommand(@"
                    INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, NgayTao)
                    OUTPUT INSERTED.MaTaiKhoan
                    VALUES (@td, @mk, @vt, 1, GETDATE())", conn, tran);
                cmdTK.Parameters.AddWithValue("@td", tenDangNhap);
                cmdTK.Parameters.AddWithValue("@mk", matKhauHash);
                cmdTK.Parameters.AddWithValue("@vt", nv.ChucVu);
                int maTK = (int)cmdTK.ExecuteScalar()!;

                var cmdNV = new SqlCommand(@"
                    INSERT INTO NhanVien (MaTaiKhoan, HoTen, ChucVu, SoDienThoai, Email, NgayVaoLam)
                    VALUES (@mtk, @ht, @cv, @sdt, @email, @nvl)", conn, tran);
                cmdNV.Parameters.AddWithValue("@mtk",   maTK);
                cmdNV.Parameters.AddWithValue("@ht",    nv.HoTen);
                cmdNV.Parameters.AddWithValue("@cv",    nv.ChucVu);
                cmdNV.Parameters.AddWithValue("@sdt",   (object?)nv.SoDienThoai ?? DBNull.Value);
                cmdNV.Parameters.AddWithValue("@email", (object?)nv.Email ?? DBNull.Value);
                cmdNV.Parameters.AddWithValue("@nvl",   (object?)nv.NgayVaoLam ?? DBNull.Value);
                cmdNV.ExecuteNonQuery();

                tran.Commit();
                return true;
            }
            catch (Exception ex) { tran.Rollback(); ShowError(ex); return false; }
        }

        public bool Update(NhanVien nv)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    UPDATE NhanVien SET
                        HoTen       = @ht,
                        ChucVu      = @cv,
                        SoDienThoai = @sdt,
                        Email       = @email,
                        NgayVaoLam  = @nvl
                    WHERE MaNhanVien = @id;
                    UPDATE TaiKhoan SET VaiTro = @cv
                    WHERE MaTaiKhoan = @mtk";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ht",    nv.HoTen);
                cmd.Parameters.AddWithValue("@cv",    nv.ChucVu);
                cmd.Parameters.AddWithValue("@sdt",   (object?)nv.SoDienThoai ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@email", (object?)nv.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@nvl",   (object?)nv.NgayVaoLam ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@id",    nv.MaNhanVien);
                cmd.Parameters.AddWithValue("@mtk",   nv.MaTaiKhoan);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex) { ShowError(ex); return false; }
        }

        public bool Delete(int maNhanVien)
        {
            using var conn = DatabaseHelper.GetConnection();
            using var tran = conn.BeginTransaction();
            try
            {
                var cmdGet = new SqlCommand("SELECT MaTaiKhoan FROM NhanVien WHERE MaNhanVien = @id", conn, tran);
                cmdGet.Parameters.AddWithValue("@id", maNhanVien);
                var maTK = cmdGet.ExecuteScalar();
                if (maTK == null) return false;

                var cmdNV = new SqlCommand("DELETE FROM NhanVien WHERE MaNhanVien = @id", conn, tran);
                cmdNV.Parameters.AddWithValue("@id", maNhanVien);
                cmdNV.ExecuteNonQuery();

                var cmdTK = new SqlCommand("DELETE FROM TaiKhoan WHERE MaTaiKhoan = @mtk", conn, tran);
                cmdTK.Parameters.AddWithValue("@mtk", maTK);
                cmdTK.ExecuteNonQuery();

                tran.Commit();
                return true;
            }
            catch (Exception ex) { tran.Rollback(); ShowError(ex); return false; }
        }

        private static void ShowError(Exception ex) =>
            MessageBox.Show($"Lỗi CSDL:\n{ex.Message}", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
