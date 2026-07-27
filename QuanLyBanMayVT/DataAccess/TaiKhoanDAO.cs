using System.Data.SqlClient;
using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT.DataAccess
{
    /// <summary>
    /// DAO xử lý các thao tác CSDL liên quan đến bảng TaiKhoan
    /// </summary>
    public class TaiKhoanDAO
    {
        /// <summary>
        /// Xác thực đăng nhập bằng plain text.
        /// </summary>
        public TaiKhoan? XacThucDangNhap(string tenDangNhap, string matKhau)
        {
            try
            {
                // Hash mật khẩu trước khi so sánh với DB (DB lưu SHA-256)
                string hashedPassword = PasswordHasher.Hash(matKhau);

                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    SELECT MaTaiKhoan, TenDangNhap, MatKhau, VaiTro, TrangThai
                    FROM   TaiKhoan
                    WHERE  TenDangNhap = @TenDangNhap
                      AND  MatKhau     = @MatKhau
                      AND  TrangThai   = 1";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
                cmd.Parameters.AddWithValue("@MatKhau",     hashedPassword); // ← dùng hash

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new TaiKhoan
                    {
                        MaTaiKhoan  = (int)reader["MaTaiKhoan"],
                        TenDangNhap = (string)reader["TenDangNhap"],
                        MatKhau     = (string)reader["MatKhau"],
                        VaiTro      = TaiKhoan.ParseVaiTro((string)reader["VaiTro"]),
                        TrangThai   = (bool)reader["TrangThai"]
                    };
                }
                return null;    // Khônng đúng tên đăng nhập hoặc mật khẩu
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi kết nối CSDL:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>Kiểm tra tên đăng nhập đã tồn tại chưa</summary>
        public bool TonTaiTenDangNhap(string tenDangNhap)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = "SELECT COUNT(1) FROM TaiKhoan WHERE TenDangNhap = @TenDangNhap";
                using var cmd  = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
                return (int)cmd.ExecuteScalar()! > 0;
            }
            catch { return false; }
        }
    }
}
