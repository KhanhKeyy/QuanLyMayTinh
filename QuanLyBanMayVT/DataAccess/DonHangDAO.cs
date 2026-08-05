using System.Data.SqlClient;
using QuanLyBanMayVT.Common;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT.DataAccess
{
    public class DonHangDAO
    {
        private static DonHang MapRow(SqlDataReader r) => new DonHang
        {
            MaDonHang           = (int)r["MaDonHang"],
            MaKhachHang         = (int)r["MaKhachHang"],
            NgayDatHang         = (DateTime)r["NgayDatHang"],
            MaPhuongThucTT      = (int)r["MaPhuongThucTT"],
            MaNhanVienXacNhan   = r["MaNhanVienXacNhan"] as int?,
            TrangThaiDonHang    = (string)r["TrangThaiDonHang"],
            GhiChu              = r["GhiChu"] as string,
            TenKhachHang        = r.GetOrdinal("TenKhachHang") >= 0 && !r.IsDBNull(r.GetOrdinal("TenKhachHang"))
                                  ? (string)r["TenKhachHang"] : "",
            TenPhuongThuc       = r.GetOrdinal("TenPhuongThuc") >= 0 && !r.IsDBNull(r.GetOrdinal("TenPhuongThuc"))
                                  ? (string)r["TenPhuongThuc"] : "",
            TenNhanVienXacNhan  = r.GetOrdinal("TenNhanVienXacNhan") >= 0 && !r.IsDBNull(r.GetOrdinal("TenNhanVienXacNhan"))
                                  ? (string)r["TenNhanVienXacNhan"] : "",
            TongTien            = r.GetOrdinal("TongTien") >= 0 && !r.IsDBNull(r.GetOrdinal("TongTien"))
                                  ? (decimal)r["TongTien"] : 0
        };

        private const string SelectJoin = @"
            SELECT dh.*,
                   kh.HoTen AS TenKhachHang,
                   pt.TenPhuongThuc,
                   nv.HoTen AS TenNhanVienXacNhan,
                   ISNULL((SELECT SUM(ctdh.ThanhTien) FROM ChiTietDonHang ctdh WHERE ctdh.MaDonHang = dh.MaDonHang), 0) AS TongTien
            FROM DonHang dh
            LEFT JOIN KhachHang kh ON dh.MaKhachHang = kh.MaKhachHang
            LEFT JOIN PhuongThucThanhToan pt ON dh.MaPhuongThucTT = pt.MaPhuongThucTT
            LEFT JOIN NhanVien nv ON dh.MaNhanVienXacNhan = nv.MaNhanVien";

        public List<DonHang> GetAll(string? trangThai = null)
        {
            var list = new List<DonHang>();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                string sql = SelectJoin;
                if (!string.IsNullOrEmpty(trangThai)) sql += " WHERE dh.TrangThaiDonHang = @tt";
                sql += " ORDER BY dh.MaDonHang DESC";
                using var cmd = new SqlCommand(sql, conn);
                if (!string.IsNullOrEmpty(trangThai))
                    cmd.Parameters.AddWithValue("@tt", trangThai);
                using var r = cmd.ExecuteReader();
                while (r.Read()) list.Add(MapRow(r));
            }
            catch (Exception ex) { ShowError(ex); }
            return list;
        }

        public List<DonHang> GetByMaKhachHang(int maKhachHang)
        {
            var list = new List<DonHang>();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                string sql = SelectJoin + " WHERE dh.MaKhachHang = @mkh ORDER BY dh.MaDonHang DESC";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@mkh", maKhachHang);
                using var r = cmd.ExecuteReader();
                while (r.Read()) list.Add(MapRow(r));
            }
            catch (Exception ex) { ShowError(ex); }
            return list;
        }

        public DonHang? GetById(int maDonHang)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                string sql = SelectJoin + " WHERE dh.MaDonHang = @id";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", maDonHang);
                using var r = cmd.ExecuteReader();
                return r.Read() ? MapRow(r) : null;
            }
            catch (Exception ex) { ShowError(ex); return null; }
        }

        /// <summary>Tạo đơn hàng mới kèm chi tiết. Trả về MaDonHang mới.</summary>
        public int Insert(DonHang dh, List<ChiTietDonHang> chiTiet)
        {
            using var conn = DatabaseHelper.GetConnection();
            using var tran = conn.BeginTransaction();
            try
            {
                // Kiểm tra số lượng tồn kho trước khi đặt hàng
                foreach (var ct in chiTiet)
                {
                    var cmdCheck = new SqlCommand("SELECT TenSanPham, SoLuongTon FROM SanPham WHERE MaSanPham = @msp", conn, tran);
                    cmdCheck.Parameters.AddWithValue("@msp", ct.MaSanPham);
                    using var rCheck = cmdCheck.ExecuteReader();
                    if (rCheck.Read())
                    {
                        string tenSP = rCheck["TenSanPham"]?.ToString() ?? "";
                        int ton = Convert.ToInt32(rCheck["SoLuongTon"]);
                        rCheck.Close();

                        if (ct.SoLuong > ton)
                        {
                            tran.Rollback();
                            MessageBox.Show($"⚠️ Rất tiếc, sản phẩm '{tenSP}' chỉ còn {ton} cái trong kho, không đủ để đặt {ct.SoLuong} cái!\nVui lòng giảm số lượng mua.",
                                "Cảnh báo tồn kho", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return -1;
                        }
                    }
                    else
                    {
                        rCheck.Close();
                    }
                }

                // Tạo đơn hàng
                var cmdDH = new SqlCommand(@"
                    INSERT INTO DonHang (MaKhachHang, NgayDatHang, MaPhuongThucTT, TrangThaiDonHang, GhiChu)
                    OUTPUT INSERTED.MaDonHang
                    VALUES (@mkh, GETDATE(), @pttt, 'Cho xac nhan', @gc)", conn, tran);
                cmdDH.Parameters.AddWithValue("@mkh",  dh.MaKhachHang);
                cmdDH.Parameters.AddWithValue("@pttt", dh.MaPhuongThucTT);
                cmdDH.Parameters.AddWithValue("@gc",   (object?)dh.GhiChu ?? DBNull.Value);
                int maDH = (int)cmdDH.ExecuteScalar()!;

                // Thêm chi tiết
                foreach (var ct in chiTiet)
                {
                    var cmdCT = new SqlCommand(@"
                        INSERT INTO ChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
                        VALUES (@mdh, @msp, @sl, @dg)", conn, tran);
                    cmdCT.Parameters.AddWithValue("@mdh", maDH);
                    cmdCT.Parameters.AddWithValue("@msp", ct.MaSanPham);
                    cmdCT.Parameters.AddWithValue("@sl",  ct.SoLuong);
                    cmdCT.Parameters.AddWithValue("@dg",  ct.DonGia);
                    cmdCT.ExecuteNonQuery();
                }
                tran.Commit();

                // ── Tự động tạo thông báo ────────────────────────────
                ThongBaoDAO.GuiThongBaoChoKhachHang(dh.MaKhachHang, "Ket qua don hang",
                    $"📦 Đơn hàng #{maDH} của bạn đã được tiếp nhận thành công và đang chờ xác nhận.", maDH);

                ThongBaoDAO.GuiThongBaoChoBanQuanLy("Ket qua don hang",
                    $"📦 Đơn hàng mới #{maDH} vừa được tạo! Vui lòng xác nhận.", maDH);

                ThongBaoDAO.KiemTraTietTonKhoThapVaGuiThongBao();

                return maDH;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                ShowError(ex);
                return -1;
            }
        }

        /// <summary>Nhân viên xác nhận đơn hàng.</summary>
        public bool XacNhanDonHang(int maDonHang, int maNhanVien)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                using var cmd = new SqlCommand(@"
                    UPDATE DonHang SET
                        TrangThaiDonHang    = 'Da xac nhan',
                        MaNhanVienXacNhan   = @mnv
                    WHERE MaDonHang = @id AND TrangThaiDonHang = 'Cho xac nhan'", conn);
                cmd.Parameters.AddWithValue("@mnv", maNhanVien);
                cmd.Parameters.AddWithValue("@id",  maDonHang);
                bool ok = cmd.ExecuteNonQuery() > 0;

                if (ok)
                {
                    var dh = GetById(maDonHang);
                    if (dh != null)
                    {
                        ThongBaoDAO.GuiThongBaoChoKhachHang(dh.MaKhachHang, "Ket qua don hang",
                            $"✅ Đơn hàng #{maDonHang} của bạn đã được nhân viên bán hàng xác nhận thành công!", maDonHang);
                    }
                }
                return ok;
            }
            catch (Exception ex) { ShowError(ex); return false; }
        }

        /// <summary>Huỷ đơn hàng.</summary>
        public bool HuyDonHang(int maDonHang)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                using var cmd = new SqlCommand(@"
                    UPDATE DonHang SET TrangThaiDonHang = 'Da huy'
                    WHERE MaDonHang = @id AND TrangThaiDonHang = 'Cho xac nhan'", conn);
                cmd.Parameters.AddWithValue("@id", maDonHang);
                bool ok = cmd.ExecuteNonQuery() > 0;

                if (ok)
                {
                    var dh = GetById(maDonHang);
                    if (dh != null)
                    {
                        ThongBaoDAO.GuiThongBaoChoKhachHang(dh.MaKhachHang, "Ket qua don hang",
                            $"❌ Đơn hàng #{maDonHang} của bạn đã bị hủy.", maDonHang);
                    }
                }
                return ok;
            }
            catch (Exception ex) { ShowError(ex); return false; }
        }

        public List<ChiTietDonHang> GetChiTiet(int maDonHang)
        {
            var list = new List<ChiTietDonHang>();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                const string sql = @"
                    SELECT ct.*, sp.TenSanPham
                    FROM ChiTietDonHang ct
                    LEFT JOIN SanPham sp ON ct.MaSanPham = sp.MaSanPham
                    WHERE ct.MaDonHang = @id";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", maDonHang);
                using var r = cmd.ExecuteReader();
                while (r.Read())
                    list.Add(new ChiTietDonHang
                    {
                        MaChiTiet  = (int)r["MaChiTiet"],
                        MaDonHang  = (int)r["MaDonHang"],
                        MaSanPham  = (int)r["MaSanPham"],
                        SoLuong    = (int)r["SoLuong"],
                        DonGia     = (decimal)r["DonGia"],
                        TenSanPham = r["TenSanPham"] as string ?? ""
                    });
            }
            catch (Exception ex) { ShowError(ex); }
            return list;
        }

        /// <summary>Đặt lại đơn hàng cũ (sao chép sản phẩm vào Giỏ hàng)</summary>
        public bool DatLaiDonHang(int maDonHang)
        {
            try
            {
                var ctList = GetChiTiet(maDonHang);
                if (ctList == null || ctList.Count == 0) return false;

                var spDao = new SanPhamDAO();
                foreach (var ct in ctList)
                {
                    var sp = spDao.GetById(ct.MaSanPham);
                    if (sp != null && sp.SoLuongTon > 0)
                    {
                        GioHangManager.ThemVaoGio(sp, ct.SoLuong);
                    }
                }
                return true;
            }
            catch { return false; }
        }

        private static void ShowError(Exception ex) =>
            MessageBox.Show($"Lỗi CSDL:\n{ex.Message}", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
