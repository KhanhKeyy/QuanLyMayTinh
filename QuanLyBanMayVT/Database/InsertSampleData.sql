-- ============================================================
-- SCRIPT THÊM DỮ LIỆU MẪU VÀO DB QuanLyMayTinh
-- VaiTro dùng NVARCHAR: 'KhachHang', 'NhanVienBanHang', 'KeToan', 'QuanLy'
-- Chạy script này trong SSMS khi đã kết nối đến QuanLyMayTinh
-- ============================================================

USE QuanLyMayTinh;
GO

-- Xem constraint hiện tại (để xác nhận)
SELECT cc.name AS TenConstraint, cc.definition AS DinhNghia
FROM sys.check_constraints cc
JOIN sys.tables t ON cc.parent_object_id = t.object_id
WHERE t.name = 'TaiKhoan';
GO

-- ============================================================
-- XEM CẤU TRÚC BẢNG TaiKhoan TRƯỚC KHI THÊM DỮ LIỆU
-- ============================================================
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'TaiKhoan'
ORDER BY ORDINAL_POSITION;
GO

-- ============================================================
-- THÊM TÀI KHOẢN MẪU (VaiTro dùng chuỗi đúng constraint)
-- Mật khẩu: 123456 (plain text, chỉ dùng để test)
-- ============================================================

-- ⚠️ Nếu đã có dữ liệu, chạy phần này để xóa trước (bỏ comment nếu cần):
-- DELETE FROM NhanVien;
-- DELETE FROM KhachHang;
-- DELETE FROM TaiKhoan WHERE TenDangNhap IN ('khachhang1','nvbanhang1','ketoan1','quanly1');

-- Thêm tài khoản mẫu cho từng vai trò
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
-- KIỂM TRA KẾT QUẢ
-- ============================================================
SELECT MaTaiKhoan, TenDangNhap, VaiTro
FROM TaiKhoan
WHERE TenDangNhap IN (N'khachhang1', N'nvbanhang1', N'ketoan1', N'quanly1')
ORDER BY MaTaiKhoan;
GO

PRINT '✅ Thêm dữ liệu mẫu thành công!';
PRINT 'Tài khoản test (mật khẩu: 123456):';
PRINT '  khachhang1  → KhachHang';
PRINT '  nvbanhang1  → NhanVienBanHang';
PRINT '  ketoan1     → KeToan';
PRINT '  quanly1     → QuanLy';
GO
