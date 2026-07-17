using System.Data.SqlClient;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT.DataAccess
{
    /// <summary>
    /// DAO xử lý các thao tác CSDL liên quan đến bảng NhanVien
    /// </summary>
    public class NhanVienDAO
    {
        /// <summary>Lấy thông tin nhân viên theo MaTaiKhoan</summary>
        public NhanVien? GetByMaTaiKhoan(int maTaiKhoan)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    SELECT MaNhanVien, HoTen, Email, SoDienThoai, ChucVu, MaTaiKhoan
                    FROM   NhanVien
                    WHERE  MaTaiKhoan = @MaTaiKhoan";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new NhanVien
                    {
                        MaNhanVien  = (int)reader["MaNhanVien"],
                        HoTen       = (string)reader["HoTen"],
                        Email       = reader["Email"].ToString() ?? "",
                        SoDienThoai = reader["SoDienThoai"].ToString() ?? "",
                        ChucVu      = reader["ChucVu"].ToString() ?? "",
                        MaTaiKhoan  = (int)reader["MaTaiKhoan"]
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi tải thông tin nhân viên:\n{ex.Message}",
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
                    SELECT MaNhanVien, HoTen, Email, SoDienThoai, ChucVu, MaTaiKhoan
                    FROM   NhanVien
                    ORDER BY HoTen";

                using var cmd    = new SqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new NhanVien
                    {
                        MaNhanVien  = (int)reader["MaNhanVien"],
                        HoTen       = (string)reader["HoTen"],
                        Email       = reader["Email"].ToString() ?? "",
                        SoDienThoai = reader["SoDienThoai"].ToString() ?? "",
                        ChucVu      = reader["ChucVu"].ToString() ?? "",
                        MaTaiKhoan  = (int)reader["MaTaiKhoan"]
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi tải danh sách nhân viên:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return list;
        }
    }
}
