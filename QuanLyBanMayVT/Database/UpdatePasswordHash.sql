-- ============================================================
-- CẬP NHẬT MẬT KHẨU SANG SHA-256 CHO CÁC TÀI KHOẢN MẪU ĐÃ CÓ
-- SHA-256('123456') = 8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92
-- Chạy script này nếu DB đã có dữ liệu plain text '123456'
-- ============================================================

USE QuanLyMayTinh;
GO

UPDATE TaiKhoan
SET MatKhau = N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92'
WHERE TenDangNhap IN (N'khachhang1', N'nvbanhang1', N'ketoan1', N'quanly1')
  AND MatKhau = N'123456';   -- Chỉ cập nhật nếu còn plain text

PRINT CAST(@@ROWCOUNT AS VARCHAR) + ' tài khoản đã được cập nhật sang hash SHA-256.';
GO

-- Kiểm tra kết quả
SELECT MaTaiKhoan, TenDangNhap, LEFT(MatKhau, 16) + '...' AS MatKhau_Preview, VaiTro
FROM TaiKhoan
WHERE TenDangNhap IN (N'khachhang1', N'nvbanhang1', N'ketoan1', N'quanly1');
GO
