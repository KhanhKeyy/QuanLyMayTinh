using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT.DataAccess
{
    public class YeuCauThemSanPhamDAO
    {
        private static bool _tableChecked = false;

        public static void EnsureTableExists()
        {
            if (_tableChecked) return;
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'YeuCauThemSanPham')
                    BEGIN
                        CREATE TABLE YeuCauThemSanPham (
                            MaYeuCau INT IDENTITY(1,1) PRIMARY KEY,
                            MaNhanVienDeXuat INT NOT NULL FOREIGN KEY REFERENCES NhanVien(MaNhanVien),
                            MaDanhMuc INT NOT NULL FOREIGN KEY REFERENCES DanhMucSanPham(MaDanhMuc),
                            TenSanPham NVARCHAR(200) NOT NULL,
                            CauHinh NVARCHAR(MAX),
                            GiaBan DECIMAL(18,2) NOT NULL,
                            SoLuongTon INT NOT NULL DEFAULT 0,
                            MucTonToiThieu INT NOT NULL DEFAULT 1,
                            LyDoDeXuat NVARCHAR(500),
                            TrangThai NVARCHAR(50) NOT NULL DEFAULT 'Cho duyet',
                            NgayDeXuat DATETIME NOT NULL DEFAULT GETDATE(),
                            NgayDuyet DATETIME NULL,
                            MaNhanVienDuyet INT NULL FOREIGN KEY REFERENCES NhanVien(MaNhanVien),
                            GhiChuDuyet NVARCHAR(500) NULL
                        );
                    END";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                const string sqlDropCheck = @"
                    DECLARE @chkName NVARCHAR(256);
                    SELECT @chkName = name FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID('ThongBao');
                    IF @chkName IS NOT NULL
                    BEGIN
                        EXEC('ALTER TABLE ThongBao DROP CONSTRAINT [' + @chkName + '];');
                    END";
                using (var cmdChk = new SqlCommand(sqlDropCheck, conn))
                {
                    cmdChk.ExecuteNonQuery();
                }

                _tableChecked = true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Lỗi khởi tạo bảng YeuCauThemSanPham:\n{ex.Message}", "Lỗi CSDL", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private static YeuCauThemSanPham MapRow(SqlDataReader r) => new YeuCauThemSanPham
        {
            MaYeuCau = (int)r["MaYeuCau"],
            MaNhanVienDeXuat = (int)r["MaNhanVienDeXuat"],
            MaDanhMuc = (int)r["MaDanhMuc"],
            TenSanPham = (string)r["TenSanPham"],
            CauHinh = r["CauHinh"] as string ?? "",
            GiaBan = (decimal)r["GiaBan"],
            SoLuongTon = (int)r["SoLuongTon"],
            MucTonToiThieu = (int)r["MucTonToiThieu"],
            LyDoDeXuat = r["LyDoDeXuat"] as string ?? "",
            TrangThai = (string)r["TrangThai"],
            NgayDeXuat = (DateTime)r["NgayDeXuat"],
            NgayDuyet = r["NgayDuyet"] as DateTime?,
            MaNhanVienDuyet = r["MaNhanVienDuyet"] as int?,
            GhiChuDuyet = r["GhiChuDuyet"] as string ?? "",
            TenNhanVienDeXuat = r.GetOrdinal("TenNhanVienDeXuat") >= 0 && !r.IsDBNull(r.GetOrdinal("TenNhanVienDeXuat")) ? (string)r["TenNhanVienDeXuat"] : "",
            TenNhanVienDuyet = r.GetOrdinal("TenNhanVienDuyet") >= 0 && !r.IsDBNull(r.GetOrdinal("TenNhanVienDuyet")) ? (string)r["TenNhanVienDuyet"] : "",
            TenDanhMuc = r.GetOrdinal("TenDanhMuc") >= 0 && !r.IsDBNull(r.GetOrdinal("TenDanhMuc")) ? (string)r["TenDanhMuc"] : ""
        };

        private const string SelectJoin = @"
            SELECT yc.*,
                   nv1.HoTen AS TenNhanVienDeXuat,
                   nv2.HoTen AS TenNhanVienDuyet,
                   dm.TenDanhMuc
            FROM YeuCauThemSanPham yc
            LEFT JOIN NhanVien nv1 ON yc.MaNhanVienDeXuat = nv1.MaNhanVien
            LEFT JOIN NhanVien nv2 ON yc.MaNhanVienDuyet = nv2.MaNhanVien
            LEFT JOIN DanhMucSanPham dm ON yc.MaDanhMuc = dm.MaDanhMuc";

        public int InsertDeXuat(YeuCauThemSanPham yc)
        {
            EnsureTableExists();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    INSERT INTO YeuCauThemSanPham (MaNhanVienDeXuat, MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, LyDoDeXuat, TrangThai, NgayDeXuat)
                    OUTPUT INSERTED.MaYeuCau
                    VALUES (@mnv, @mdm, @tsp, @ch, @gb, @slt, @mtt, @ld, 'Cho duyet', GETDATE())";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@mnv", yc.MaNhanVienDeXuat);
                cmd.Parameters.AddWithValue("@mdm", yc.MaDanhMuc);
                cmd.Parameters.AddWithValue("@tsp", yc.TenSanPham);
                cmd.Parameters.AddWithValue("@ch",  (object?)yc.CauHinh ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@gb",  yc.GiaBan);
                cmd.Parameters.AddWithValue("@slt", yc.SoLuongTon);
                cmd.Parameters.AddWithValue("@mtt", yc.MucTonToiThieu);
                cmd.Parameters.AddWithValue("@ld",  (object?)yc.LyDoDeXuat ?? DBNull.Value);

                int maYC = (int)cmd.ExecuteScalar()!;
                if (maYC > 0)
                {
                    ThongBaoDAO.GuiThongBaoChoBanQuanLy("De xuat sp",
                        $"💡 Đề xuất sản phẩm mới #{maYC}: '{yc.TenSanPham}' vừa được gửi từ Nhân viên. Vui lòng kiểm duyệt.");
                }
                return maYC;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Lỗi gửi đề xuất sản phẩm:\n{ex.Message}", "Lỗi", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return -1;
            }
        }

        public List<YeuCauThemSanPham> GetAll(string? trangThai = null)
        {
            EnsureTableExists();
            var list = new List<YeuCauThemSanPham>();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                string sql = SelectJoin;
                if (!string.IsNullOrEmpty(trangThai)) sql += " WHERE yc.TrangThai = @tt";
                sql += " ORDER BY yc.MaYeuCau DESC";

                using var cmd = new SqlCommand(sql, conn);
                if (!string.IsNullOrEmpty(trangThai)) cmd.Parameters.AddWithValue("@tt", trangThai);
                using var r = cmd.ExecuteReader();
                while (r.Read()) list.Add(MapRow(r));
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Lỗi tải danh sách đề xuất:\n{ex.Message}", "Lỗi", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            return list;
        }

        public int DemChoDuyet()
        {
            EnsureTableExists();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = "SELECT COUNT(1) FROM YeuCauThemSanPham WHERE TrangThai = 'Cho duyet'";
                using var cmd = new SqlCommand(sql, conn);
                return (int)cmd.ExecuteScalar()!;
            }
            catch { return 0; }
        }

        public bool DuyetYeuCau(int maYeuCau, int maNhanVienDuyet)
        {
            EnsureTableExists();
            using var conn = DatabaseHelper.GetConnection();
            using var tran = conn.BeginTransaction();
            try
            {
                // Lấy thông tin đề xuất
                const string sqlGet = "SELECT * FROM YeuCauThemSanPham WHERE MaYeuCau = @id";
                using var cmdGet = new SqlCommand(sqlGet, conn, tran);
                cmdGet.Parameters.AddWithValue("@id", maYeuCau);
                int mnvDeXuat = 0, mdm = 0, slt = 0, mtt = 0;
                string tsp = "", ch = "";
                decimal gb = 0;

                using (var r = cmdGet.ExecuteReader())
                {
                    if (!r.Read()) return false;
                    mnvDeXuat = (int)r["MaNhanVienDeXuat"];
                    mdm = (int)r["MaDanhMuc"];
                    tsp = (string)r["TenSanPham"];
                    ch = r["CauHinh"] as string ?? "";
                    gb = (decimal)r["GiaBan"];
                    slt = (int)r["SoLuongTon"];
                    mtt = (int)r["MucTonToiThieu"];
                }

                // 1. Thêm vào bảng SanPham
                const string sqlInsertSP = @"
                    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, HinhAnh, TrangThai)
                    VALUES (@mdm, @tsp, @ch, @gb, @slt, @mtt, NULL, 'Con hang')";
                using var cmdInsertSP = new SqlCommand(sqlInsertSP, conn, tran);
                cmdInsertSP.Parameters.AddWithValue("@mdm", mdm);
                cmdInsertSP.Parameters.AddWithValue("@tsp", tsp);
                cmdInsertSP.Parameters.AddWithValue("@ch",  ch);
                cmdInsertSP.Parameters.AddWithValue("@gb",  gb);
                cmdInsertSP.Parameters.AddWithValue("@slt", slt);
                cmdInsertSP.Parameters.AddWithValue("@mtt", mtt);
                cmdInsertSP.ExecuteNonQuery();

                // 2. Cập nhật YeuCauThemSanPham -> Da duyet
                const string sqlUpdateYC = @"
                    UPDATE YeuCauThemSanPham
                    SET TrangThai = 'Da duyet', NgayDuyet = GETDATE(), MaNhanVienDuyet = @mnd
                    WHERE MaYeuCau = @id";
                using var cmdUpdateYC = new SqlCommand(sqlUpdateYC, conn, tran);
                cmdUpdateYC.Parameters.AddWithValue("@mnd", maNhanVienDuyet);
                cmdUpdateYC.Parameters.AddWithValue("@id",  maYeuCau);
                cmdUpdateYC.ExecuteNonQuery();

                tran.Commit();

                // 3. Thông báo cho Nhân viên đề xuất
                var cmdTK = new SqlCommand("SELECT MaTaiKhoan FROM NhanVien WHERE MaNhanVien = @mnv", conn);
                cmdTK.Parameters.AddWithValue("@mnv", mnvDeXuat);
                var tkObj = cmdTK.ExecuteScalar();
                if (tkObj != null)
                {
                    int maTK = Convert.ToInt32(tkObj);
                    var cmdTB = new SqlCommand(@"
                        INSERT INTO ThongBao (LoaiThongBao, NoiDung, NgayTao, MaTaiKhoanNhan, DaDoc)
                        VALUES ('De xuat sp', N'🎉 Đề xuất sản phẩm ''' + @tsp + N''' của bạn đã được Quản lý PHÊ DUYỆT và chính thức thêm vào Hệ thống!', GETDATE(), @mtk, 0)", conn);
                    cmdTB.Parameters.AddWithValue("@tsp", tsp);
                    cmdTB.Parameters.AddWithValue("@mtk", maTK);
                    cmdTB.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                System.Windows.Forms.MessageBox.Show($"Lỗi duyệt đề xuất sản phẩm:\n{ex.Message}", "Lỗi", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }

        public bool TuChoiYeuCau(int maYeuCau, int maNhanVienDuyet, string lyDoTuChoi)
        {
            EnsureTableExists();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sqlUpdateYC = @"
                    UPDATE YeuCauThemSanPham
                    SET TrangThai = 'Tu choi', NgayDuyet = GETDATE(), MaNhanVienDuyet = @mnd, GhiChuDuyet = @gc
                    WHERE MaYeuCau = @id";
                using var cmdUpdateYC = new SqlCommand(sqlUpdateYC, conn);
                cmdUpdateYC.Parameters.AddWithValue("@mnd", maNhanVienDuyet);
                cmdUpdateYC.Parameters.AddWithValue("@gc",  (object?)lyDoTuChoi ?? DBNull.Value);
                cmdUpdateYC.Parameters.AddWithValue("@id",  maYeuCau);
                cmdUpdateYC.ExecuteNonQuery();

                // Lấy thông tin người đề xuất
                var cmdGet = new SqlCommand("SELECT MaNhanVienDeXuat, TenSanPham FROM YeuCauThemSanPham WHERE MaYeuCau = @id", conn);
                cmdGet.Parameters.AddWithValue("@id", maYeuCau);
                using var r = cmdGet.ExecuteReader();
                if (r.Read())
                {
                    int mnvDeXuat = (int)r["MaNhanVienDeXuat"];
                    string tsp = (string)r["TenSanPham"];
                    r.Close();

                    var cmdTK = new SqlCommand("SELECT MaTaiKhoan FROM NhanVien WHERE MaNhanVien = @mnv", conn);
                    cmdTK.Parameters.AddWithValue("@mnv", mnvDeXuat);
                    var tkObj = cmdTK.ExecuteScalar();
                    if (tkObj != null)
                    {
                        int maTK = Convert.ToInt32(tkObj);
                        var cmdTB = new SqlCommand(@"
                            INSERT INTO ThongBao (LoaiThongBao, NoiDung, NgayTao, MaTaiKhoanNhan, DaDoc)
                            VALUES ('De xuat sp', N'❌ Đề xuất sản phẩm ''' + @tsp + N''' của bạn đã bị Từ chối. Lý do: ' + @lydo, GETDATE(), @mtk, 0)", conn);
                        cmdTB.Parameters.AddWithValue("@tsp", tsp);
                        cmdTB.Parameters.AddWithValue("@lydo", string.IsNullOrEmpty(lyDoTuChoi) ? "Không đạt yêu cầu" : lyDoTuChoi);
                        cmdTB.Parameters.AddWithValue("@mtk", maTK);
                        cmdTB.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Lỗi từ chối đề xuất:\n{ex.Message}", "Lỗi", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
