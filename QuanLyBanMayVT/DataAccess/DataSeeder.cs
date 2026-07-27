using System.Data.SqlClient;

namespace QuanLyBanMayVT.DataAccess
{
    public static class DataSeeder
    {
        public static void FixFontAndSeedData()
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();

                // 1. Sửa lại tên các DanhMucSanPham bị lỗi font
                using (var cmdFixDM = new SqlCommand(@"
                    UPDATE DanhMucSanPham SET TenDanhMuc = N'Máy tính để bàn (PC)', MoTa = N'Các bộ máy tính để bàn đồng bộ và lắp ráp' WHERE TenDanhMuc LIKE N'%để bàn%' OR TenDanhMuc LIKE N'%dá»%';
                    UPDATE DanhMucSanPham SET TenDanhMuc = N'Máy tính xách tay (Laptop)', MoTa = N'Laptop văn phòng, đồ họa, gaming' WHERE TenDanhMuc LIKE N'%xách tay%' OR TenDanhMuc LIKE N'%xÃ¡ch%';
                    UPDATE DanhMucSanPham SET TenDanhMuc = N'Linh kiện máy tính', MoTa = N'CPU, RAM, GPU, Mainboard, SSD, HDD, Nguồn' WHERE TenDanhMuc LIKE N'%Linh%' OR TenDanhMuc LIKE N'%Linh ki%';
                    UPDATE DanhMucSanPham SET TenDanhMuc = N'Thiết bị ngoại vi', MoTa = N'Màn hình, Bàn phím, Chuột, Tai nghe, Loa' WHERE TenDanhMuc LIKE N'%ngoại vi%' OR TenDanhMuc LIKE N'%ngoáº¡i%';
                ", conn))
                {
                    cmdFixDM.ExecuteNonQuery();
                }

                // Lấy ID danh mục Linh Kiện
                int dmLinhKien = 0;
                using (var cmdGetID = new SqlCommand("SELECT TOP 1 MaDanhMuc FROM DanhMucSanPham WHERE TenDanhMuc = N'Linh kiện máy tính'", conn))
                {
                    var res = cmdGetID.ExecuteScalar();
                    if (res != null && res != DBNull.Value)
                        dmLinhKien = Convert.ToInt32(res);
                }

                if (dmLinhKien == 0) return;

                // 2. Xóa các sản phẩm linh kiện bị lỗi mã hóa font cũ
                using (var cmdClean = new SqlCommand(@"
                    DELETE FROM ChiTietDonHang WHERE MaSanPham IN (SELECT MaSanPham FROM SanPham WHERE TenSanPham LIKE N'%á%' OR TenSanPham LIKE N'%Ã%' OR CauHinh LIKE N'%Luá%');
                    DELETE FROM SanPham WHERE TenSanPham LIKE N'%á%' OR TenSanPham LIKE N'%Ã%' OR CauHinh LIKE N'%Luá%';
                ", conn))
                {
                    cmdClean.ExecuteNonQuery();
                }

                // 3. Danh sách 21 linh kiện chuẩn Unicode UTF-16
                var items = new (string TenSP, string CauHinh, decimal GiaBan, int SoLuong)[]
                {
                    // CPU
                    ("CPU Intel Core i5-13400F", "10 Nhân 16 Luồng / Up to 4.6GHz / 20MB Cache / Socket LGA 1700", 4850000, 25),
                    ("CPU Intel Core i7-13700K", "16 Nhân 24 Luồng / Up to 5.4GHz / 30MB Cache / Socket LGA 1700", 9990000, 15),
                    ("CPU AMD Ryzen 5 7600X", "6 Nhân 12 Luồng / Up to 5.3GHz / 38MB Cache / Socket AM5", 5690000, 18),

                    // Mainboard
                    ("Mainboard MSI PRO B660M-A DDR4", "Micro-ATX / Socket LGA 1700 / 4 Slot RAM DDR4 / 2x M.2 NVMe", 3190000, 20),
                    ("Mainboard ASUS TUF GAMING B760-PLUS WIFI", "ATX / Socket LGA 1700 / RAM DDR5 / PCIe 5.0 / WiFi 6", 4950000, 12),
                    ("Mainboard GIGABYTE Z790 AORUS ELITE", "ATX / Socket LGA 1700 / DDR5 7600MHz / 4x M.2 NVMe / PCIe 5.0", 7490000, 8),

                    // RAM
                    ("RAM Kingston FURY Beast 16GB DDR4 3200MHz", "1x16GB / DDR4 / Bus 3200MHz / Tản nhiệt nhôm đen", 1050000, 40),
                    ("RAM Corsair Vengeance RGB 32GB DDR5 6000MHz", "Kit 32GB (2x16GB) / DDR5 / Bus 6000MHz / RGB Dynamic Sync", 3450000, 25),
                    ("RAM G.SKILL Trident Z5 RGB 32GB DDR5 5600MHz", "Kit 32GB (2x16GB) / DDR5 / Bus 5600MHz / Tản nhôm cao cấp", 3290000, 15),

                    // VGA
                    ("Card VGA MSI RTX 4060 VENTUS 2X 8G OC", "8GB GDDR6 / 128-bit / PCIe 4.0 / Dual Fan Tản Nhiệt", 8290000, 15),
                    ("Card VGA GIGABYTE RTX 4070 SUPER WINDFORCE OC 12G", "12GB GDDR6X / 192-bit / PCIe 4.0 / 3 Fan Windforce", 17990000, 10),
                    ("Card VGA ASUS ROG Strix RTX 4080 SUPER 16GB", "16GB GDDR6X / 256-bit / PCIe 4.0 / Aura Sync RGB", 31500000, 5),

                    // SSD / HDD
                    ("SSD Kingston NV2 500GB PCIe 4.0 NVMe M.2", "500GB M.2 NVMe / Đọc 3500MB/s / Ghi 2100MB/s", 1150000, 30),
                    ("SSD Samsung 980 PRO 1TB PCIe NVMe M.2", "1TB M.2 NVMe PCIe 4.0 / Đọc 7000MB/s / Ghi 5000MB/s", 2650000, 20),
                    ("Ổ cứng HDD Western Digital Blue 2TB 3.5 inch", "2TB / 3.5 inch / SATA3 / 7200RPM / 256MB Cache", 1690000, 15),

                    // PSU
                    ("Nguồn PSU Corsair CV650 650W 80 Plus Bronze", "650W / 80 Plus Bronze / Quạt 120mm / Dây cáp bọc lưới", 1450000, 25),
                    ("Nguồn PSU MSI MAG A750GL PCIE5 750W 80 Plus Gold", "750W / Full Modular / 80 Plus Gold / Hỗ trợ ATX 3.0 PCIe 5.0", 2790000, 18),
                    ("Nguồn PSU ASUS ROG Strix 850W 80 Plus Gold", "850W / Full Modular / 80 Plus Gold / Tản nhiệt ROG cao cấp", 3990000, 10),

                    // Case
                    ("Vỏ Case Xigmatek Gaming X 3FX", "Mid Tower / Mặt kính cường lực / Đã kèm 3 Fan RGB", 890000, 20),
                    ("Vỏ Case NZXT H5 Flow Black", "Mid Tower / Kính cường lực / Tối ưu luồng khí / Thiết kế tối giản", 2390000, 12),
                    ("Vỏ Case Lian Li O11 Dynamic EVO Black", "Mid Tower / Kính đôi Panorama / Thiết kế modular cao cấp", 4250000, 8),
                };

                foreach (var item in items)
                {
                    using var cmdInsert = new SqlCommand(@"
                        IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = @TenSP)
                        BEGIN
                            INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
                            VALUES (@MaDM, @TenSP, @CauHinh, @GiaBan, @SoLuong, 3, N'Con hang');
                        END
                        ELSE
                        BEGIN
                            UPDATE SanPham SET CauHinh = @CauHinh, GiaBan = @GiaBan WHERE TenSanPham = @TenSP;
                        END
                    ", conn);

                    cmdInsert.Parameters.AddWithValue("@MaDM", dmLinhKien);
                    cmdInsert.Parameters.AddWithValue("@TenSP", item.TenSP);
                    cmdInsert.Parameters.AddWithValue("@CauHinh", item.CauHinh);
                    cmdInsert.Parameters.AddWithValue("@GiaBan", item.GiaBan);
                    cmdInsert.Parameters.AddWithValue("@SoLuong", item.SoLuong);

                    cmdInsert.ExecuteNonQuery();
                }
            }
            catch
            {
                // Bỏ qua lỗi nếu chưa có DB
            }
        }
    }
}
