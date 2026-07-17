using System.Data.SqlClient;

namespace QuanLyBanMayVT.DataAccess
{
    /// <summary>
    /// Lớp tiện ích quản lý kết nối CSDL.
    /// Chỉ cần sửa CONNECTION_STRING để thay đổi database.
    /// </summary>
    public static class DatabaseHelper
    {
        // ══════════════════════════════════════════════════════════════════
        // ⚙️  CHUỖI KẾT NỐI — SỬA CHO PHÙ HỢP VỚI MÔI TRƯỜNG CỦA BẠN
        // Server:   (localdb)\MSSQLLocalDB
        // Database: QuanLyMayTinh
        // ══════════════════════════════════════════════════════════════════
        private static readonly string CONNECTION_STRING =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=QuanLyMayTinh;Integrated Security=True;TrustServerCertificate=True";

        /// <summary>Tạo và trả về SqlConnection đã được mở</summary>
        public static SqlConnection GetConnection()
        {
            var conn = new SqlConnection(CONNECTION_STRING);
            conn.Open();
            return conn;
        }

        /// <summary>Kiểm tra kết nối — trả về true nếu thành công</summary>
        public static bool TestConnection(out string errorMessage)
        {
            try
            {
                using var conn = GetConnection();
                errorMessage = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>Overload không cần out param</summary>
        public static bool TestConnection() => TestConnection(out _);
    }
}
