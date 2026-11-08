# 🖥️ Quản Lý Bán Máy Vi Tính

Ứng dụng desktop quản lý cửa hàng bán máy vi tính và linh kiện, phân quyền theo 4 vai trò: **Khách hàng, Nhân viên bán hàng, Kế toán, Quản lý**. Đây là đồ án môn *Phân tích và thiết kế hệ thống*, gồm phần phân tích (sơ đồ trình tự, lược đồ CSDL) và phần cài đặt bằng C#.

## Công nghệ

| Thành phần | Chi tiết |
|---|---|
| Ngôn ngữ | C# 10 |
| Nền tảng | .NET 6, Windows Forms (`net6.0-windows`) |
| Cơ sở dữ liệu | SQL Server (LocalDB) |
| Truy cập dữ liệu | `System.Data.SqlClient`, mô hình DAO |
| Bảo mật | Mật khẩu băm SHA-256 |
| Công cụ | Visual Studio, Git/GitHub |

## Chức năng

**Khách hàng**
- Xem danh sách sản phẩm theo danh mục, tìm kiếm
- Thêm vào giỏ hàng, đặt hàng, chọn phương thức thanh toán
- Nhận thông báo về đơn hàng
- Đề xuất sản phẩm mới cho cửa hàng

**Nhân viên bán hàng**
- Xem hàng hoá, tồn kho
- Xem và xác nhận đơn hàng

**Kế toán**
- Xem hàng hoá, đơn hàng, tồn kho
- Lập và xem hoá đơn

**Quản lý (toàn quyền)**
- Quản lý hàng hoá và danh mục (thêm, sửa, xoá)
- Nhập hàng (lập phiếu nhập)
- Báo cáo: doanh thu, hàng tồn kho, sản phẩm bán chạy
- Quản lý nhân viên
- Kiểm duyệt đề xuất sản phẩm của khách hàng
- Nhận cảnh báo khi tồn kho thấp

### Bảng phân quyền

| Chức năng | NV bán hàng | Kế toán | Quản lý |
|---|:-:|:-:|:-:|
| Xem hàng hoá | ✅ | ✅ | ✅ |
| Thêm / sửa / xoá hàng hoá | ❌ | ❌ | ✅ |
| Nhập hàng | ❌ | ❌ | ✅ |
| Xem đơn hàng | ✅ | ✅ | ✅ |
| Xác nhận đơn hàng | ✅ | ❌ | ✅ |
| Hoá đơn | ❌ | ✅ | ✅ |
| Tồn kho | ✅ | ✅ | ✅ |
| Báo cáo | ❌ | ❌ | ✅ |
| Quản lý nhân viên | ❌ | ❌ | ✅ |

## Cấu trúc dự án

```
QuanLyBanMayVT/
├── Models/        # Lớp dữ liệu: TaiKhoan, SanPham, DonHang, HoaDon, PhieuNhapHang, ...
├── DataAccess/    # Tầng truy cập CSDL (DAO): SanPhamDAO, DonHangDAO, HoaDonDAO, ...
├── Common/        # Tiện ích: UserSession, PasswordHasher, GioHangManager, UIStyleHelper
├── Database/      # Script SQL: dữ liệu mẫu, dữ liệu linh kiện, cập nhật hash mật khẩu
├── frm*.cs        # Các form giao diện (đăng nhập, chính, khách hàng, sản phẩm, đơn hàng, ...)
└── Program.cs     # Điểm khởi chạy
```

Kiến trúc chia 3 lớp: **Giao diện (Forms) → Truy cập dữ liệu (DAO) → CSDL**, dùng `Models` để truyền dữ liệu giữa các lớp.

## Tài liệu phân tích thiết kế

| Tài liệu | Mô tả |
|---|---|
| [Lược đồ quan hệ CSDL](Relational%20Database%20Schema.png) | Các bảng và khoá ngoại |
| [Sơ đồ trình tự: Khách hàng / hệ thống](SoDoTrinhTu_QuanLyMayTinh.svg) | Luồng tổng quát |
| [Sơ đồ trình tự: Nhân viên bán hàng](SoDoTrinhTu_NhanVienBanHang.svg) | Luồng xử lý đơn hàng |
| [Sơ đồ trình tự: Kế toán](SoDoTrinhTu_KeToan.svg) | Luồng lập hoá đơn |
| [Sơ đồ trình tự: Quản lý](SoDoTrinhTu_QuanLy.svg) | Luồng quản trị |

![Lược đồ CSDL](Relational%20Database%20Schema.png)

## Cài đặt và chạy

**Yêu cầu:** Windows, Visual Studio 2022 trở lên (workload *.NET desktop development*), SQL Server LocalDB.

1. Clone repo:
   ```bash
   git clone https://github.com/KhanhKeyy/QuanLyMayTinh.git
   ```
2. Mở SQL Server Management Studio, kết nối `(localdb)\MSSQLLocalDB`, chạy lần lượt các script trong `QuanLyBanMayVT/Database/`:
   - `CreateDatabase.sql`: tạo database, các bảng, tài khoản mẫu (mật khẩu đã băm SHA-256)
   - `InsertSampleData.sql`: dữ liệu mẫu
   - `SeedLinhKienData.sql`: dữ liệu linh kiện
3. Không cần chạy `UpdatePasswordHash.sql` nếu đã chạy `CreateDatabase.sql` (script này chỉ dành cho dữ liệu cũ còn mật khẩu dạng thô).
4. Nếu dùng SQL Server khác, sửa chuỗi kết nối trong `DataAccess/DatabaseHelper.cs`.
5. Mở `QuanLyBanMayVT.slnx` trong Visual Studio và nhấn **F5**.

## Tài khoản dùng thử

Mật khẩu chung: `123456` (chỉ dùng cho môi trường demo)

| Tên đăng nhập | Vai trò |
|---|---|
| `khachhang1` | Khách hàng |
| `nvbanhang1` | Nhân viên bán hàng |
| `ketoan1` | Kế toán |
| `quanly1` | Quản lý |

## Hạn chế và hướng phát triển

- SHA-256 chưa có salt, nên nâng cấp lên bcrypt hoặc PBKDF2 nếu triển khai thực tế.
- Chuỗi kết nối đang ghi cứng trong mã nguồn, nên chuyển sang file cấu hình.
- Chưa có unit test.

## Tác giả

KhanhKeyy, ThaoNoa, Kachien712 · Đại học Kinh tế - Kỹ thuật Công nghiệp (UNETI)
