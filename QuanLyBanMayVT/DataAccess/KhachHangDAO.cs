using System.Data.SqlClient;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT.DataAccess
{
    /// <summary>
    /// DAO xử lý các thao tác CSDL liên quan đến bảng KhachHang
    /// </summary>
    public class KhachHangDAO
    {
        /// <summary>Lấy thông tin khách hàng theo MaTaiKhoan</summary>
        public KhachHang? GetByMaTaiKhoan(int maTaiKhoan)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    SELECT MaKhachHang, HoTen, Email, SoDienThoai, DiaChi, MaTaiKhoan
                    FROM   KhachHang
                    WHERE  MaTaiKhoan = @MaTaiKhoan";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new KhachHang
                    {
                        MaKhachHang = (int)reader["MaKhachHang"],
                        HoTen       = (string)reader["HoTen"],
                        Email       = reader["Email"].ToString() ?? "",
                        SoDienThoai = reader["SoDienThoai"].ToString() ?? "",
                        DiaChi      = reader["DiaChi"].ToString() ?? "",
                        MaTaiKhoan  = (int)reader["MaTaiKhoan"]
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi tải thông tin khách hàng:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>Lấy thông tin khách hàng theo MaKhachHang</summary>
        public KhachHang? GetById(int maKhachHang)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    SELECT MaKhachHang, HoTen, Email, SoDienThoai, DiaChi, MaTaiKhoan
                    FROM   KhachHang
                    WHERE  MaKhachHang = @mkh";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@mkh", maKhachHang);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new KhachHang
                    {
                        MaKhachHang = (int)reader["MaKhachHang"],
                        HoTen       = (string)reader["HoTen"],
                        Email       = reader["Email"].ToString() ?? "",
                        SoDienThoai = reader["SoDienThoai"].ToString() ?? "",
                        DiaChi      = reader["DiaChi"].ToString() ?? "",
                        MaTaiKhoan  = (int)reader["MaTaiKhoan"]
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải thông tin khách hàng:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>Lấy danh sách tất cả khách hàng</summary>
        public List<KhachHang> GetAll()
        {
            var list = new List<KhachHang>();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    SELECT MaKhachHang, HoTen, Email, SoDienThoai, DiaChi, MaTaiKhoan
                    FROM   KhachHang
                    ORDER BY HoTen";

                using var cmd    = new SqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new KhachHang
                    {
                        MaKhachHang = (int)reader["MaKhachHang"],
                        HoTen       = (string)reader["HoTen"],
                        Email       = reader["Email"].ToString() ?? "",
                        SoDienThoai = reader["SoDienThoai"].ToString() ?? "",
                        DiaChi      = reader["DiaChi"].ToString() ?? "",
                        MaTaiKhoan  = (int)reader["MaTaiKhoan"]
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi tải danh sách khách hàng:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return list;
        }
    }
}
