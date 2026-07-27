-- ============================================================
-- SCRIPT THÊM DỮ LIỆU SẢN PHẨM LINH KIỆN MÁY TÍNH DÙNG CHO CỬA HÀNG VÀ BUILD PC
-- Chạy script này trong SQL Server Management Studio (SSMS) kết nối tới DB QuanLyMayTinh
-- ============================================================

USE QuanLyMayTinh;
GO

DECLARE @dmLinhKien INT = (SELECT TOP 1 MaDanhMuc FROM DanhMucSanPham WHERE TenDanhMuc = N'Linh kiện máy tính');

IF @dmLinhKien IS NULL
BEGIN
    INSERT INTO DanhMucSanPham (TenDanhMuc, MoTa) VALUES (N'Linh kiện máy tính', N'CPU, RAM, GPU, Mainboard, SSD, HDD, Nguồn, Case');
    SET @dmLinhKien = SCOPE_IDENTITY();
END

-- ── 1. VI XỬ LÝ (CPU) ──────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'CPU Intel Core i5-13400F')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'CPU Intel Core i5-13400F', N'10 Nhân 16 Luồng / Up to 4.6GHz / 20MB Cache / Socket LGA 1700', 4850000, 25, 5, N'Con hang');

IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'CPU Intel Core i7-13700K')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'CPU Intel Core i7-13700K', N'16 Nhân 24 Luồng / Up to 5.4GHz / 30MB Cache / Socket LGA 1700', 9990000, 15, 3, N'Con hang');

IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'CPU AMD Ryzen 5 7600X')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'CPU AMD Ryzen 5 7600X', N'6 Nhân 12 Luồng / Up to 5.3GHz / 38MB Cache / Socket AM5', 5690000, 18, 4, N'Con hang');

-- ── 2. BO MẠCH CHỦ (MAINBOARD) ──────────────────────────────
IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'Mainboard MSI PRO B660M-A DDR4')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'Mainboard MSI PRO B660M-A DDR4', N'Micro-ATX / Socket LGA 1700 / 4 Slot RAM DDR4 / 2x M.2 NVMe', 3190000, 20, 5, N'Con hang');

IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'Mainboard ASUS TUF GAMING B760-PLUS WIFI')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'Mainboard ASUS TUF GAMING B760-PLUS WIFI', N'ATX / Socket LGA 1700 / RAM DDR5 / PCIe 5.0 / WiFi 6', 4950000, 12, 3, N'Con hang');

IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'Mainboard GIGABYTE Z790 AORUS ELITE')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'Mainboard GIGABYTE Z790 AORUS ELITE', N'ATX / Socket LGA 1700 / DDR5 7600MHz / 4x M.2 NVMe / PCIe 5.0', 7490000, 8, 2, N'Con hang');

-- ── 3. BỘ NHỚ RAM ──────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'RAM Kingston FURY Beast 16GB DDR4 3200MHz')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'RAM Kingston FURY Beast 16GB DDR4 3200MHz', N'1x16GB / DDR4 / Bus 3200MHz / Tản nhiệt nhôm đen', 1050000, 40, 10, N'Con hang');

IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'RAM Corsair Vengeance RGB 32GB DDR5 6000MHz')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'RAM Corsair Vengeance RGB 32GB DDR5 6000MHz', N'Kit 32GB (2x16GB) / DDR5 / Bus 6000MHz / RGB Dynamic Sync', 3450000, 25, 5, N'Con hang');

IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'RAM G.SKILL Trident Z5 RGB 32GB DDR5 5600MHz')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'RAM G.SKILL Trident Z5 RGB 32GB DDR5 5600MHz', N'Kit 32GB (2x16GB) / DDR5 / Bus 5600MHz / Tản nhôm cao cấp', 3290000, 15, 4, N'Con hang');

-- ── 4. CARD ĐỒ HỌA (VGA) ───────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'Card VGA MSI RTX 4060 VENTUS 2X 8G OC')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'Card VGA MSI RTX 4060 VENTUS 2X 8G OC', N'8GB GDDR6 / 128-bit / PCIe 4.0 / Dual Fan Tản Nhiệt', 8290000, 15, 3, N'Con hang');

IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'Card VGA GIGABYTE RTX 4070 SUPER WINDFORCE OC 12G')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'Card VGA GIGABYTE RTX 4070 SUPER WINDFORCE OC 12G', N'12GB GDDR6X / 192-bit / PCIe 4.0 / 3 Fan Windforce', 17990000, 10, 2, N'Con hang');

IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'Card VGA ASUS ROG Strix RTX 4080 SUPER 16GB')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'Card VGA ASUS ROG Strix RTX 4080 SUPER 16GB', N'16GB GDDR6X / 256-bit / PCIe 4.0 / Aura Sync RGB', 31500000, 5, 1, N'Con hang');

-- ── 5. Ổ CỨNG (SSD / HDD) ──────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'SSD Kingston NV2 500GB PCIe 4.0 NVMe M.2')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'SSD Kingston NV2 500GB PCIe 4.0 NVMe M.2', N'500GB M.2 NVMe / Đọc 3500MB/s / Ghi 2100MB/s', 1150000, 30, 8, N'Con hang');

IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'SSD Samsung 980 PRO 1TB PCIe NVMe M.2')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'SSD Samsung 980 PRO 1TB PCIe NVMe M.2', N'1TB M.2 NVMe PCIe 4.0 / Đọc 7000MB/s / Ghi 5000MB/s', 2650000, 20, 5, N'Con hang');

IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'Ổ cứng HDD Western Digital Blue 2TB 3.5 inch')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'Ổ cứng HDD Western Digital Blue 2TB 3.5 inch', N'2TB / 3.5 inch / SATA3 / 7200RPM / 256MB Cache', 1690000, 15, 5, N'Con hang');

-- ── 6. NGUỒN MÁY TÍNH (PSU) ────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'Nguồn PSU Corsair CV650 650W 80 Plus Bronze')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'Nguồn PSU Corsair CV650 650W 80 Plus Bronze', N'650W / 80 Plus Bronze / Quạt 120mm / Dây cáp bọc lưới', 1450000, 25, 5, N'Con hang');

IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'Nguồn PSU MSI MAG A750GL PCIE5 750W 80 Plus Gold')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'Nguồn PSU MSI MAG A750GL PCIE5 750W 80 Plus Gold', N'750W / Full Modular / 80 Plus Gold / Hỗ trợ ATX 3.0 PCIe 5.0', 2790000, 18, 4, N'Con hang');

IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'Nguồn PSU ASUS ROG Strix 850W 80 Plus Gold')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'Nguồn PSU ASUS ROG Strix 850W 80 Plus Gold', N'850W / Full Modular / 80 Plus Gold / Tản nhiệt ROG cao cấp', 3990000, 10, 2, N'Con hang');

-- ── 7. VỎ CASE ─────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'Vỏ Case Xigmatek Gaming X 3FX')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'Vỏ Case Xigmatek Gaming X 3FX', N'Mid Tower / Mặt kính cường lực / Đã kèm 3 Fan RGB', 890000, 20, 5, N'Con hang');

IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'Vỏ Case NZXT H5 Flow Black')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'Vỏ Case NZXT H5 Flow Black', N'Mid Tower / Kính cường lực / Tối ưu luồng khí / Thiết kế tối giản', 2390000, 12, 3, N'Con hang');

IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'Vỏ Case Lian Li O11 Dynamic EVO Black')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'Vỏ Case Lian Li O11 Dynamic EVO Black', N'Mid Tower / Kính đôi Panorama / Thiết kế modular cao cấp', 4250000, 8, 2, N'Con hang');
GO

PRINT N'✅ Đã thêm đầy đủ 21 mẫu linh kiện PC (CPU, Main, RAM, VGA, SSD, Nguồn, Case) vào database!';
GO
