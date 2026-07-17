-- ============================================================
-- SCRIPT TẠO CSDL: QuanLyBanMayVT
-- Chạy script này trong SQL Server Management Studio (SSMS)
-- hoặc Azure Data Studio
-- ============================================================

USE master;
GO

-- Tạo database nếu chưa có
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'QuanLyBanMayVT')
BEGIN
    CREATE DATABASE QuanLyBanMayVT;
    PRINT 'Database QuanLyBanMayVT đã được tạo.';
END
GO

USE QuanLyBanMayVT;
GO

-- ============================================================
-- BẢNG: TaiKhoan
-- VaiTro: 0 = KhachHang, 1 = NhanVien, 2 = Admin
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TaiKhoan' AND xtype='U')
BEGIN
    CREATE TABLE TaiKhoan (
        MaTaiKhoan  INT           IDENTITY(1,1) PRIMARY KEY,
        TenDangNhap NVARCHAR(50)  NOT NULL UNIQUE,
        MatKhau     NVARCHAR(255) NOT NULL,   -- Nên lưu dạng hash
        VaiTro      INT           NOT NULL DEFAULT 0,  
        TrangThai   BIT           NOT NULL DEFAULT 1   -- 1 = Hoạt động
    );
    PRINT 'Bảng TaiKhoan đã được tạo.';
END
GO

-- ============================================================
-- BẢNG: KhachHang
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='KhachHang' AND xtype='U')
BEGIN
    CREATE TABLE KhachHang (
        MaKhachHang INT          IDENTITY(1,1) PRIMARY KEY,
        HoTen       NVARCHAR(100) NOT NULL,
        Email       NVARCHAR(100),
        SoDienThoai NVARCHAR(15),
        DiaChi      NVARCHAR(255),
        MaTaiKhoan  INT          NOT NULL UNIQUE,
        FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
    );
    PRINT 'Bảng KhachHang đã được tạo.';
END
GO

-- ============================================================
-- BẢNG: NhanVien
-- ChucVu: 0 = NhanVienBanHang, 1 = KeToan, 2 = QuanLy
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='NhanVien' AND xtype='U')
BEGIN
    CREATE TABLE NhanVien (
        MaNhanVien  INT           IDENTITY(1,1) PRIMARY KEY,
        HoTen       NVARCHAR(100) NOT NULL,
        Email       NVARCHAR(100),
        SoDienThoai NVARCHAR(15),
        ChucVu      INT           NOT NULL DEFAULT 0,
        MaTaiKhoan  INT           NOT NULL UNIQUE,
        FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
    );
    PRINT 'Bảng NhanVien đã được tạo.';
END
GO

-- ============================================================
-- DỮ LIỆU MẪU (TEST DATA)
-- ============================================================

-- Xóa dữ liệu cũ nếu có (để chạy lại script)
DELETE FROM NhanVien;
DELETE FROM KhachHang;
DELETE FROM TaiKhoan;
DBCC CHECKIDENT ('TaiKhoan', RESEED, 0);
DBCC CHECKIDENT ('NhanVien',  RESEED, 0);
DBCC CHECKIDENT ('KhachHang', RESEED, 0);

-- ── Tài khoản mẫu ─────────────────────────────────────────
-- (Mật khẩu lưu dạng plain text để test; thực tế phải hash)
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai) VALUES
('khachhang1',   '123456', 0, 1),   -- Khách hàng
('nvbanhang1',   '123456', 1, 1),   -- Nhân viên bán hàng
('ketoan1',      '123456', 1, 1),   -- Kế toán
('quanly1',      '123456', 1, 1),   -- Quản lý
('admin',        'admin',  2, 1);   -- Admin
GO

-- ── Khách hàng ─────────────────────────────────────────────
INSERT INTO KhachHang (HoTen, Email, SoDienThoai, DiaChi, MaTaiKhoan)
SELECT 'Nguyễn Văn An', 'an.nguyen@email.com', '0901234567',
       '123 Đường Lê Lợi, TP.HCM', MaTaiKhoan
FROM TaiKhoan WHERE TenDangNhap = 'khachhang1';
GO

-- ── Nhân viên bán hàng ─────────────────────────────────────
INSERT INTO NhanVien (HoTen, Email, SoDienThoai, ChucVu, MaTaiKhoan)
SELECT 'Trần Thị Bình', 'binh.tran@shop.com', '0912345678',
       0, MaTaiKhoan   -- ChucVu = 0: NhanVienBanHang
FROM TaiKhoan WHERE TenDangNhap = 'nvbanhang1';
GO

-- ── Kế toán ────────────────────────────────────────────────
INSERT INTO NhanVien (HoTen, Email, SoDienThoai, ChucVu, MaTaiKhoan)
SELECT 'Lê Minh Cường', 'cuong.le@shop.com', '0923456789',
       1, MaTaiKhoan   -- ChucVu = 1: KeToan
FROM TaiKhoan WHERE TenDangNhap = 'ketoan1';
GO

-- ── Quản lý ────────────────────────────────────────────────
INSERT INTO NhanVien (HoTen, Email, SoDienThoai, ChucVu, MaTaiKhoan)
SELECT 'Phạm Quốc Dũng', 'dung.pham@shop.com', '0934567890',
       2, MaTaiKhoan   -- ChucVu = 2: QuanLy
FROM TaiKhoan WHERE TenDangNhap = 'quanly1';
GO

-- ── Kiểm tra dữ liệu ───────────────────────────────────────
SELECT 
    tk.MaTaiKhoan,
    tk.TenDangNhap,
    CASE tk.VaiTro 
        WHEN 0 THEN 'Khách hàng'
        WHEN 1 THEN 'Nhân viên'
        WHEN 2 THEN 'Admin'
    END AS VaiTro,
    COALESCE(nv.HoTen, kh.HoTen, 'Admin') AS HoTen,
    CASE 
        WHEN nv.ChucVu = 0 THEN 'Nhân viên bán hàng'
        WHEN nv.ChucVu = 1 THEN 'Kế toán'
        WHEN nv.ChucVu = 2 THEN 'Quản lý'
        ELSE N'—'
    END AS ChucVu
FROM TaiKhoan tk
LEFT JOIN NhanVien nv ON tk.MaTaiKhoan = nv.MaTaiKhoan
LEFT JOIN KhachHang kh ON tk.MaTaiKhoan = kh.MaTaiKhoan
ORDER BY tk.MaTaiKhoan;
GO

PRINT '✅ CSDL QuanLyBanMayVT đã sẵn sàng!';
PRINT 'Tài khoản test (mật khẩu: 123456):';
PRINT '  - khachhang1  → Khách hàng';
PRINT '  - nvbanhang1  → Nhân viên bán hàng';
PRINT '  - ketoan1     → Kế toán';
PRINT '  - quanly1     → Quản lý';
PRINT '  - admin / admin → Admin';
GO
