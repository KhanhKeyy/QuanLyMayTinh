using System.Data.SqlClient;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT.DataAccess
{
    public class SanPhamDAO
    {
        private SanPham MapRow(SqlDataReader r) => new SanPham
        {
            MaSanPham      = (int)r["MaSanPham"],
            MaDanhMuc      = (int)r["MaDanhMuc"],
            TenSanPham     = (string)r["TenSanPham"],
            CauHinh        = r["CauHinh"] as string,
            GiaBan         = (decimal)r["GiaBan"],
            SoLuongTon     = (int)r["SoLuongTon"],
            MucTonToiThieu = (int)r["MucTonToiThieu"],
            HinhAnh        = r["HinhAnh"] as string,
            TrangThai      = (string)r["TrangThai"],
            TenDanhMuc     = r.GetOrdinal("TenDanhMuc") >= 0 && !r.IsDBNull(r.GetOrdinal("TenDanhMuc"))
                             ? (string)r["TenDanhMuc"] : ""
        };

        private const string SelectJoin = @"
            SELECT sp.*, dm.TenDanhMuc
            FROM SanPham sp
            LEFT JOIN DanhMucSanPham dm ON sp.MaDanhMuc = dm.MaDanhMuc";

        public List<SanPham> GetAll(string? keyword = null, int? maDanhMuc = null, bool chiConHang = false)
        {
            var list = new List<SanPham>();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                var where = new List<string>();
                if (!string.IsNullOrWhiteSpace(keyword))
                    where.Add("(sp.TenSanPham LIKE @kw OR sp.CauHinh LIKE @kw)");
                if (maDanhMuc.HasValue && maDanhMuc > 0)
                    where.Add("sp.MaDanhMuc = @maDM");
                if (chiConHang)
                    where.Add("sp.TrangThai = 'Con hang' AND sp.SoLuongTon > 0");

                string sql = SelectJoin + (where.Count > 0 ? " WHERE " + string.Join(" AND ", where) : "")
                             + " ORDER BY sp.TenSanPham";
                using var cmd = new SqlCommand(sql, conn);
                if (!string.IsNullOrWhiteSpace(keyword))
                    cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");
                if (maDanhMuc.HasValue && maDanhMuc > 0)
                    cmd.Parameters.AddWithValue("@maDM", maDanhMuc.Value);
                using var r = cmd.ExecuteReader();
                while (r.Read()) list.Add(MapRow(r));
            }
            catch (Exception ex) { ShowError(ex); }
            return list;
        }

        public SanPham? GetById(int maSanPham)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                string sql = SelectJoin + " WHERE sp.MaSanPham = @id";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", maSanPham);
                using var r = cmd.ExecuteReader();
                return r.Read() ? MapRow(r) : null;
            }
            catch (Exception ex) { ShowError(ex); return null; }
        }

        public int Insert(SanPham sp)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, HinhAnh, TrangThai)
                    OUTPUT INSERTED.MaSanPham
                    VALUES (@dm, @ten, @ch, @gia, @sl, @min, @img, @tt)";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@dm",  sp.MaDanhMuc);
                cmd.Parameters.AddWithValue("@ten", sp.TenSanPham);
                cmd.Parameters.AddWithValue("@ch",  (object?)sp.CauHinh ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@gia", sp.GiaBan);
                cmd.Parameters.AddWithValue("@sl",  sp.SoLuongTon);
                cmd.Parameters.AddWithValue("@min", sp.MucTonToiThieu);
                cmd.Parameters.AddWithValue("@img", (object?)sp.HinhAnh ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@tt",  sp.TrangThai);
                return (int)cmd.ExecuteScalar()!;
            }
            catch (Exception ex) { ShowError(ex); return -1; }
        }

        public bool Update(SanPham sp)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    UPDATE SanPham SET
                        MaDanhMuc      = @dm,
                        TenSanPham     = @ten,
                        CauHinh        = @ch,
                        GiaBan         = @gia,
                        SoLuongTon     = @sl,
                        MucTonToiThieu = @min,
                        HinhAnh        = @img,
                        TrangThai      = @tt
                    WHERE MaSanPham = @id";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@dm",  sp.MaDanhMuc);
                cmd.Parameters.AddWithValue("@ten", sp.TenSanPham);
                cmd.Parameters.AddWithValue("@ch",  (object?)sp.CauHinh ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@gia", sp.GiaBan);
                cmd.Parameters.AddWithValue("@sl",  sp.SoLuongTon);
                cmd.Parameters.AddWithValue("@min", sp.MucTonToiThieu);
                cmd.Parameters.AddWithValue("@img", (object?)sp.HinhAnh ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@tt",  sp.TrangThai);
                cmd.Parameters.AddWithValue("@id",  sp.MaSanPham);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex) { ShowError(ex); return false; }
        }

        public bool Delete(int maSanPham)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                // Kiểm tra có trong đơn hàng không
                using var check = new SqlCommand(
                    "SELECT COUNT(1) FROM ChiTietDonHang WHERE MaSanPham = @id", conn);
                check.Parameters.AddWithValue("@id", maSanPham);
                if ((int)check.ExecuteScalar()! > 0)
                {
                    MessageBox.Show("Không thể xóa sản phẩm đã có trong đơn hàng.\nHãy đổi trạng thái sang 'Ngừng kinh doanh'.",
                        "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                using var cmd = new SqlCommand("DELETE FROM SanPham WHERE MaSanPham = @id", conn);
                cmd.Parameters.AddWithValue("@id", maSanPham);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex) { ShowError(ex); return false; }
        }

        /// <summary>Cộng/trừ số lượng tồn kho (delta dương = cộng, âm = trừ)</summary>
        public bool CapNhatSoLuongTon(int maSanPham, int delta)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    UPDATE SanPham SET SoLuongTon = SoLuongTon + @delta
                    WHERE MaSanPham = @id;
                    UPDATE SanPham SET TrangThai = CASE
                        WHEN SoLuongTon <= 0 THEN 'Het hang'
                        ELSE 'Con hang' END
                    WHERE MaSanPham = @id AND TrangThai <> 'Ngung KD'";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@delta", delta);
                cmd.Parameters.AddWithValue("@id", maSanPham);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex) { ShowError(ex); return false; }
        }

        /// <summary>Lấy danh sách sản phẩm cần nhập (tồn dưới mức tối thiểu)</summary>
        public List<SanPham> GetCanNhap()
        {
            var list = new List<SanPham>();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                string sql = SelectJoin +
                    " WHERE sp.SoLuongTon < sp.MucTonToiThieu AND sp.TrangThai <> 'Ngung KD'" +
                    " ORDER BY (sp.MucTonToiThieu - sp.SoLuongTon) DESC";
                using var cmd = new SqlCommand(sql, conn);
                using var r = cmd.ExecuteReader();
                while (r.Read()) list.Add(MapRow(r));
            }
            catch (Exception ex) { ShowError(ex); }
            return list;
        }

        private static void ShowError(Exception ex) =>
            MessageBox.Show($"Lỗi CSDL:\n{ex.Message}", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
