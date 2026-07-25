using System.Data.SqlClient;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT.DataAccess
{
    public class DanhMucSanPhamDAO
    {
        private static DanhMucSanPham MapRow(SqlDataReader r) => new DanhMucSanPham
        {
            MaDanhMuc  = (int)r["MaDanhMuc"],
            TenDanhMuc = (string)r["TenDanhMuc"],
            MoTa       = r["MoTa"] as string
        };

        public List<DanhMucSanPham> GetAll()
        {
            var list = new List<DanhMucSanPham>();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                using var cmd = new SqlCommand("SELECT * FROM DanhMucSanPham ORDER BY TenDanhMuc", conn);
                using var r = cmd.ExecuteReader();
                while (r.Read()) list.Add(MapRow(r));
            }
            catch (Exception ex) { ShowError(ex); }
            return list;
        }

        public bool Insert(DanhMucSanPham dm)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                using var cmd = new SqlCommand(
                    "INSERT INTO DanhMucSanPham (TenDanhMuc, MoTa) VALUES (@ten, @mota)", conn);
                cmd.Parameters.AddWithValue("@ten",  dm.TenDanhMuc);
                cmd.Parameters.AddWithValue("@mota", (object?)dm.MoTa ?? DBNull.Value);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex) { ShowError(ex); return false; }
        }

        public bool Update(DanhMucSanPham dm)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                using var cmd = new SqlCommand(
                    "UPDATE DanhMucSanPham SET TenDanhMuc=@ten, MoTa=@mota WHERE MaDanhMuc=@id", conn);
                cmd.Parameters.AddWithValue("@ten",  dm.TenDanhMuc);
                cmd.Parameters.AddWithValue("@mota", (object?)dm.MoTa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@id",   dm.MaDanhMuc);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex) { ShowError(ex); return false; }
        }

        public bool Delete(int maDanhMuc)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                using var check = new SqlCommand(
                    "SELECT COUNT(1) FROM SanPham WHERE MaDanhMuc = @id", conn);
                check.Parameters.AddWithValue("@id", maDanhMuc);
                if ((int)check.ExecuteScalar()! > 0)
                {
                    MessageBox.Show("Danh mục này đang có sản phẩm, không thể xóa.",
                        "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                using var cmd = new SqlCommand("DELETE FROM DanhMucSanPham WHERE MaDanhMuc=@id", conn);
                cmd.Parameters.AddWithValue("@id", maDanhMuc);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex) { ShowError(ex); return false; }
        }

        private static void ShowError(Exception ex) =>
            MessageBox.Show($"Lỗi CSDL:\n{ex.Message}", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
