-- ============================================================
-- SCRIPT THÊM DỮ LIỆU MẪU VÀO DB QuanLyMayTinh
-- VaiTro dùng NVARCHAR: 'KhachHang', 'NhanVienBanHang', 'KeToan', 'QuanLy'
-- Chạy script này trong SSMS khi đã kết nối đến QuanLyMayTinh
-- ============================================================

USE QuanLyMayTinh;
GO

-- ============================================================
-- 1. THÊM TÀI KHOẢN MẪU (Mật khẩu: 123456 -> SHA-256 hash)
-- ============================================================
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro)
SELECT N'khachhang1', N'123456', N'KhachHang'
WHERE NOT EXISTS (SELECT 1 FROM TaiKhoan WHERE TenDangNhap = N'khachhang1');

INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro)
SELECT N'nvbanhang1', N'123456', N'NhanVienBanHang'
WHERE NOT EXISTS (SELECT 1 FROM TaiKhoan WHERE TenDangNhap = N'nvbanhang1');

INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro)
SELECT N'ketoan1', N'123456', N'KeToan'
WHERE NOT EXISTS (SELECT 1 FROM TaiKhoan WHERE TenDangNhap = N'ketoan1');

INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro)
SELECT N'quanly1', N'123456', N'QuanLy'
WHERE NOT EXISTS (SELECT 1 FROM TaiKhoan WHERE TenDangNhap = N'quanly1');
GO

-- ============================================================
-- 2. THÊM PHƯƠNG THỨC THANH TOÁN
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM PhuongThucThanhToan WHERE TenPhuongThuc = N'Tiền mặt')
    INSERT INTO PhuongThucThanhToan (TenPhuongThuc) VALUES (N'Tiền mặt');

IF NOT EXISTS (SELECT 1 FROM PhuongThucThanhToan WHERE TenPhuongThuc = N'Chuyển khoản ngân hàng')
    INSERT INTO PhuongThucThanhToan (TenPhuongThuc) VALUES (N'Chuyển khoản ngân hàng');

IF NOT EXISTS (SELECT 1 FROM PhuongThucThanhToan WHERE TenPhuongThuc = N'Thẻ tín dụng / Ghi nợ')
    INSERT INTO PhuongThucThanhToan (TenPhuongThuc) VALUES (N'Thẻ tín dụng / Ghi nợ');

IF NOT EXISTS (SELECT 1 FROM PhuongThucThanhToan WHERE TenPhuongThuc = N'Ví điện tử (Momo / ZaloPay)')
    INSERT INTO PhuongThucThanhToan (TenPhuongThuc) VALUES (N'Ví điện tử (Momo / ZaloPay)');
GO

-- ============================================================
-- 3. THÊM DANH MỤC SẢN PHẨM MẪU
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM DanhMucSanPham WHERE TenDanhMuc = N'Máy tính để bàn (PC)')
    INSERT INTO DanhMucSanPham (TenDanhMuc, MoTa) VALUES (N'Máy tính để bàn (PC)', N'Các bộ máy tính để bàn đồng bộ và lắp ráp');

IF NOT EXISTS (SELECT 1 FROM DanhMucSanPham WHERE TenDanhMuc = N'Máy tính xách tay (Laptop)')
    INSERT INTO DanhMucSanPham (TenDanhMuc, MoTa) VALUES (N'Máy tính xách tay (Laptop)', N'Laptop văn phòng, đồ họa, gaming');

IF NOT EXISTS (SELECT 1 FROM DanhMucSanPham WHERE TenDanhMuc = N'Linh kiện máy tính')
    INSERT INTO DanhMucSanPham (TenDanhMuc, MoTa) VALUES (N'Linh kiện máy tính', N'CPU, RAM, GPU, Mainboard, SSD, HDD, Nguồn');

IF NOT EXISTS (SELECT 1 FROM DanhMucSanPham WHERE TenDanhMuc = N'Thiết bị ngoại vi')
    INSERT INTO DanhMucSanPham (TenDanhMuc, MoTa) VALUES (N'Thiết bị ngoại vi', N'Màn hình, Bàn phím, Chuột, Tai nghe, Loa');
GO

-- ============================================================
-- 4. THÊM SẢN PHẨM MẪU
-- ============================================================
DECLARE @dmPC INT = (SELECT TOP 1 MaDanhMuc FROM DanhMucSanPham WHERE TenDanhMuc = N'Máy tính để bàn (PC)');
DECLARE @dmLaptop INT = (SELECT TOP 1 MaDanhMuc FROM DanhMucSanPham WHERE TenDanhMuc = N'Máy tính xách tay (Laptop)');
DECLARE @dmLinhKien INT = (SELECT TOP 1 MaDanhMuc FROM DanhMucSanPham WHERE TenDanhMuc = N'Linh kiện máy tính');
DECLARE @dmNgoaiVi INT = (SELECT TOP 1 MaDanhMuc FROM DanhMucSanPham WHERE TenDanhMuc = N'Thiết bị ngoại vi');

IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'PC Gaming Ultra Core i7 RTX 4070')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmPC, N'PC Gaming Ultra Core i7 RTX 4070', N'Intel Core i7-13700K / 32GB RAM DDR5 / 1TB NVMe SSD / RTX 4070 12GB', 38500000, 15, 3, N'Con hang');

IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'PC Văn Phòng HP ProDesk 400 G9')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmPC, N'PC Văn Phòng HP ProDesk 400 G9', N'Intel Core i5-12500 / 8GB RAM / 512GB SSD / Intel UHD 770', 13200000, 20, 5, N'Con hang');

IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'Laptop ASUS ROG Strix G16')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLaptop, N'Laptop ASUS ROG Strix G16', N'Core i7-13650HX / 16GB RAM / 512GB SSD / RTX 4060 8GB / 16 inch 165Hz', 32900000, 8, 2, N'Con hang');

IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'Laptop Dell XPS 13 Plus')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLaptop, N'Laptop Dell XPS 13 Plus', N'Core i7-1360P / 16GB RAM / 1TB SSD / 13.4 inch 3.5K OLED Touch', 42500000, 5, 2, N'Con hang');

IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'VGA ASUS TUF Gaming RTX 4060 Ti 8GB')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmLinhKien, N'VGA ASUS TUF Gaming RTX 4060 Ti 8GB', N'8GB GDDR6 / 128-bit / PCIe 4.0 / 3 Fan Tản Nhiệt', 11800000, 2, 5, N'Con hang');

IF NOT EXISTS (SELECT 1 FROM SanPham WHERE TenSanPham = N'Màn hình LG UltraGear 27GR75Q 27 inch 2K 165Hz')
    INSERT INTO SanPham (MaDanhMuc, TenSanPham, CauHinh, GiaBan, SoLuongTon, MucTonToiThieu, TrangThai)
    VALUES (@dmNgoaiVi, N'Màn hình LG UltraGear 27GR75Q 27 inch 2K 165Hz', N'27 inch 2K QHD (2560x1440) / IPS / 1ms GtG / 165Hz / HDR10', 64900000, 12, 4, N'Con hang');
GO

PRINT '✅ Thêm dữ liệu mẫu (Tài khoản, Danh mục, PTTT, Sản phẩm) thành công!';
GO
