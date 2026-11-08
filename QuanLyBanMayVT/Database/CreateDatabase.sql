-- ============================================================
-- CreateDatabase.sql — Tạo lại CSDL QuanLyMayTinh
-- Dựng lại từ lược đồ quan hệ + câu truy vấn trong các DAO.
-- Chạy trong SSMS, kết nối (localdb)\MSSQLLocalDB, bấm Execute (F5).
-- Sau đó chạy tiếp: InsertSampleData.sql, SeedLinhKienData.sql
-- ============================================================

USE master;
GO

-- Xoá database cũ (đang lỗi) nếu còn tồn tại để tạo lại từ đầu
IF DB_ID(N'QuanLyMayTinh') IS NOT NULL
BEGIN
    ALTER DATABASE QuanLyMayTinh SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyMayTinh;
END
GO

CREATE DATABASE QuanLyMayTinh;
GO

USE QuanLyMayTinh;
GO

-- ============================================================
-- 1. TÀI KHOẢN, KHÁCH HÀNG, NHÂN VIÊN
-- VaiTro / ChucVu: 'KhachHang', 'NhanVienBanHang', 'KeToan', 'QuanLy'
-- ============================================================
CREATE TABLE TaiKhoan (
    MaTaiKhoan  INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap NVARCHAR(50)  NOT NULL UNIQUE,
    MatKhau     NVARCHAR(255) NOT NULL,           -- SHA-256 (64 ký tự hex)
    VaiTro      NVARCHAR(30)  NOT NULL,
    TrangThai   BIT           NOT NULL DEFAULT 1,
    NgayTao     DATETIME      NOT NULL DEFAULT GETDATE()
);

CREATE TABLE KhachHang (
    MaKhachHang INT IDENTITY(1,1) PRIMARY KEY,
    MaTaiKhoan  INT           NOT NULL UNIQUE
                FOREIGN KEY REFERENCES TaiKhoan(MaTaiKhoan),
    HoTen       NVARCHAR(100) NOT NULL,
    SoDienThoai NVARCHAR(15)  NULL,
    Email       NVARCHAR(100) NULL,
    DiaChi      NVARCHAR(255) NULL
);

CREATE TABLE NhanVien (
    MaNhanVien  INT IDENTITY(1,1) PRIMARY KEY,
    MaTaiKhoan  INT           NOT NULL UNIQUE
                FOREIGN KEY REFERENCES TaiKhoan(MaTaiKhoan),
    HoTen       NVARCHAR(100) NOT NULL,
    ChucVu      NVARCHAR(30)  NOT NULL,
    SoDienThoai NVARCHAR(15)  NULL,
    Email       NVARCHAR(100) NULL,
    NgayVaoLam  DATETIME      NULL
);

-- ============================================================
-- 2. DANH MỤC, SẢN PHẨM, PHƯƠNG THỨC THANH TOÁN
-- SanPham.TrangThai: 'Con hang' / 'Het hang' / 'Ngung KD'
-- ============================================================
CREATE TABLE DanhMucSanPham (
    MaDanhMuc  INT IDENTITY(1,1) PRIMARY KEY,
    TenDanhMuc NVARCHAR(100) NOT NULL,
    MoTa       NVARCHAR(255) NULL
);

CREATE TABLE SanPham (
    MaSanPham      INT IDENTITY(1,1) PRIMARY KEY,
    MaDanhMuc      INT            NOT NULL
                   FOREIGN KEY REFERENCES DanhMucSanPham(MaDanhMuc),
    TenSanPham     NVARCHAR(200)  NOT NULL,
    CauHinh        NVARCHAR(MAX)  NULL,
    GiaBan         DECIMAL(18,2)  NOT NULL,
    SoLuongTon     INT            NOT NULL DEFAULT 0,
    MucTonToiThieu INT            NOT NULL DEFAULT 1,
    HinhAnh        NVARCHAR(500)  NULL,
    TrangThai      NVARCHAR(30)   NOT NULL DEFAULT 'Con hang'
);

CREATE TABLE PhuongThucThanhToan (
    MaPhuongThucTT INT IDENTITY(1,1) PRIMARY KEY,
    TenPhuongThuc  NVARCHAR(100) NOT NULL
);

-- ============================================================
-- 3. ĐƠN HÀNG, CHI TIẾT ĐƠN HÀNG, HOÁ ĐƠN
-- DonHang.TrangThaiDonHang: 'Cho xac nhan' / 'Da xac nhan' / 'Hoan tat' / 'Da huy'
-- HoaDon.TrangThaiThanhToan: 'Chua thanh toan' / 'Da thanh toan' / 'That bai'
-- ============================================================
CREATE TABLE DonHang (
    MaDonHang         INT IDENTITY(1,1) PRIMARY KEY,
    MaKhachHang       INT           NOT NULL
                      FOREIGN KEY REFERENCES KhachHang(MaKhachHang),
    NgayDatHang       DATETIME      NOT NULL DEFAULT GETDATE(),
    MaPhuongThucTT    INT           NULL
                      FOREIGN KEY REFERENCES PhuongThucThanhToan(MaPhuongThucTT),
    MaNhanVienXacNhan INT           NULL
                      FOREIGN KEY REFERENCES NhanVien(MaNhanVien),
    TrangThaiDonHang  NVARCHAR(30)  NOT NULL DEFAULT 'Cho xac nhan',
    GhiChu            NVARCHAR(500) NULL
);

CREATE TABLE ChiTietDonHang (
    MaChiTiet INT IDENTITY(1,1) PRIMARY KEY,
    MaDonHang INT           NOT NULL
              FOREIGN KEY REFERENCES DonHang(MaDonHang),
    MaSanPham INT           NOT NULL
              FOREIGN KEY REFERENCES SanPham(MaSanPham),
    SoLuong   INT           NOT NULL,
    DonGia    DECIMAL(18,2) NOT NULL,
    ThanhTien AS (SoLuong * DonGia) PERSISTED     -- DonHangDAO dùng SUM(ThanhTien)
);

CREATE TABLE HoaDon (
    MaHoaDon           INT IDENTITY(1,1) PRIMARY KEY,
    MaDonHang          INT           NOT NULL
                       FOREIGN KEY REFERENCES DonHang(MaDonHang),
    MaKeToan           INT           NOT NULL
                       FOREIGN KEY REFERENCES NhanVien(MaNhanVien),
    NgayLapHoaDon      DATETIME      NOT NULL DEFAULT GETDATE(),
    TongTien           DECIMAL(18,2) NOT NULL,
    TrangThaiThanhToan NVARCHAR(30)  NOT NULL DEFAULT 'Chua thanh toan',
    NgayThanhToan      DATETIME      NULL
);

-- ============================================================
-- 4. NHẬP HÀNG
-- PhieuNhapHang.TrangThai: 'Cho kiem tra' / 'Da nhap kho'
-- ============================================================
CREATE TABLE PhieuNhapHang (
    MaPhieuNhap INT IDENTITY(1,1) PRIMARY KEY,
    MaQuanLy    INT          NOT NULL
                FOREIGN KEY REFERENCES NhanVien(MaNhanVien),
    NgayNhap    DATETIME     NOT NULL DEFAULT GETDATE(),
    TrangThai   NVARCHAR(30) NOT NULL DEFAULT 'Cho kiem tra'
);

CREATE TABLE ChiTietPhieuNhap (
    MaChiTiet   INT IDENTITY(1,1) PRIMARY KEY,
    MaPhieuNhap INT           NOT NULL
                FOREIGN KEY REFERENCES PhieuNhapHang(MaPhieuNhap),
    MaSanPham   INT           NOT NULL
                FOREIGN KEY REFERENCES SanPham(MaSanPham),
    SoLuongNhap INT           NOT NULL,
    DonGiaNhap  DECIMAL(18,2) NOT NULL
);

-- ============================================================
-- 5. THÔNG BÁO, ĐỀ XUẤT SẢN PHẨM
-- ThongBao.LoaiThongBao: 'Ton kho thap' / 'Ket qua don hang' / 'De xuat sp'
-- (Không đặt CHECK constraint: code tự gỡ constraint này khi khởi động)
-- ============================================================
CREATE TABLE ThongBao (
    MaThongBao     INT IDENTITY(1,1) PRIMARY KEY,
    LoaiThongBao   NVARCHAR(50)  NOT NULL,
    NoiDung        NVARCHAR(500) NOT NULL,
    NgayTao        DATETIME      NOT NULL DEFAULT GETDATE(),
    MaTaiKhoanNhan INT           NOT NULL
                   FOREIGN KEY REFERENCES TaiKhoan(MaTaiKhoan),
    MaSanPham      INT           NULL
                   FOREIGN KEY REFERENCES SanPham(MaSanPham),
    MaDonHang      INT           NULL
                   FOREIGN KEY REFERENCES DonHang(MaDonHang),
    DaDoc          BIT           NOT NULL DEFAULT 0
);

CREATE TABLE YeuCauThemSanPham (
    MaYeuCau         INT IDENTITY(1,1) PRIMARY KEY,
    MaNhanVienDeXuat INT NOT NULL FOREIGN KEY REFERENCES NhanVien(MaNhanVien),
    MaDanhMuc        INT NOT NULL FOREIGN KEY REFERENCES DanhMucSanPham(MaDanhMuc),
    TenSanPham       NVARCHAR(200) NOT NULL,
    CauHinh          NVARCHAR(MAX) NULL,
    GiaBan           DECIMAL(18,2) NOT NULL,
    SoLuongTon       INT NOT NULL DEFAULT 0,
    MucTonToiThieu   INT NOT NULL DEFAULT 1,
    LyDoDeXuat       NVARCHAR(500) NULL,
    TrangThai        NVARCHAR(50)  NOT NULL DEFAULT 'Cho duyet',
    NgayDeXuat       DATETIME      NOT NULL DEFAULT GETDATE(),
    NgayDuyet        DATETIME      NULL,
    MaNhanVienDuyet  INT NULL FOREIGN KEY REFERENCES NhanVien(MaNhanVien),
    GhiChuDuyet      NVARCHAR(500) NULL
);
GO

-- ============================================================
-- 6. DỮ LIỆU CƠ BẢN ĐỂ ĐĂNG NHẬP ĐƯỢC NGAY
-- Mật khẩu mẫu 123456 đã băm SHA-256
-- ============================================================
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro) VALUES
 (N'khachhang1', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'KhachHang'),
 (N'nvbanhang1', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'NhanVienBanHang'),
 (N'ketoan1',    N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'KeToan'),
 (N'quanly1',    N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'QuanLy');

INSERT INTO KhachHang (MaTaiKhoan, HoTen, SoDienThoai, Email, DiaChi)
SELECT MaTaiKhoan, N'Khách hàng mẫu', N'0900000001', N'khachhang1@example.com', N'Hà Nội'
FROM TaiKhoan WHERE TenDangNhap = N'khachhang1';

INSERT INTO NhanVien (MaTaiKhoan, HoTen, ChucVu, SoDienThoai, Email, NgayVaoLam)
SELECT MaTaiKhoan, N'Nhân viên bán hàng mẫu', N'NhanVienBanHang', N'0900000002', N'nvbanhang1@example.com', GETDATE()
FROM TaiKhoan WHERE TenDangNhap = N'nvbanhang1';

INSERT INTO NhanVien (MaTaiKhoan, HoTen, ChucVu, SoDienThoai, Email, NgayVaoLam)
SELECT MaTaiKhoan, N'Kế toán mẫu', N'KeToan', N'0900000003', N'ketoan1@example.com', GETDATE()
FROM TaiKhoan WHERE TenDangNhap = N'ketoan1';

INSERT INTO NhanVien (MaTaiKhoan, HoTen, ChucVu, SoDienThoai, Email, NgayVaoLam)
SELECT MaTaiKhoan, N'Quản lý mẫu', N'QuanLy', N'0900000004', N'quanly1@example.com', GETDATE()
FROM TaiKhoan WHERE TenDangNhap = N'quanly1';

INSERT INTO PhuongThucThanhToan (TenPhuongThuc) VALUES
 (N'Tiền mặt'),
 (N'Chuyển khoản ngân hàng'),
 (N'Thẻ tín dụng / Ghi nợ'),
 (N'Ví điện tử (Momo / ZaloPay)');

INSERT INTO DanhMucSanPham (TenDanhMuc, MoTa) VALUES
 (N'Máy tính để bàn (PC)',      N'Các bộ máy tính để bàn đồng bộ và lắp ráp'),
 (N'Máy tính xách tay (Laptop)', N'Laptop văn phòng, đồ họa, gaming'),
 (N'Linh kiện máy tính',        N'CPU, RAM, GPU, Mainboard, SSD, HDD, Nguồn'),
 (N'Thiết bị ngoại vi',         N'Màn hình, Bàn phím, Chuột, Tai nghe, Loa');
GO

PRINT N'Tạo CSDL QuanLyMayTinh thành công. Chạy tiếp InsertSampleData.sql và SeedLinhKienData.sql.';
GO
