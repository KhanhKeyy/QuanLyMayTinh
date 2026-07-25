using System.Data.SqlClient;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT.DataAccess
{
    public class PhuongThucThanhToanDAO
    {
        public List<PhuongThucThanhToan> GetAll()
        {
            var list = new List<PhuongThucThanhToan>();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                using var cmd = new SqlCommand("SELECT * FROM PhuongThucThanhToan ORDER BY TenPhuongThuc", conn);
                using var r = cmd.ExecuteReader();
                while (r.Read())
                    list.Add(new PhuongThucThanhToan
                    {
                        MaPhuongThucTT = (int)r["MaPhuongThucTT"],
                        TenPhuongThuc  = (string)r["TenPhuongThuc"]
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi CSDL:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return list;
        }
    }
}
