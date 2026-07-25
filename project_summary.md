# 📋 Tổng hợp dự án: QuanLyBanMayVT

## 1. Thông tin kỹ thuật tổng quan

| Mục | Chi tiết |
|-----|----------|
| **Loại ứng dụng** | Windows Forms App (.NET 6) |
| **Target Framework** | `net6.0-windows` |
| **Ngôn ngữ** | C# 10 |
| **NuGet Package** | `System.Data.SqlClient` v4.9.1 |
| **Database** | SQL Server (LocalDB `\MSSQLLocalDB`) |
| **Tên DB** | `QuanLyMayTinh` |
| **IDE** | Visual Studio (file `.slnx`) |

---

## 2. Cấu trúc thư mục

```
QuanLyBanMayVT/
│
├── Program.cs                       ← Entry point
│
├── Models/                          ← Lớp dữ liệu (POCO)
│   ├── TaiKhoan.cs                  ← Model tài khoản + enum VaiTro
│   ├── NhanVien.cs                  ← Model nhân viên
│   └── KhachHang.cs                 ← Model khách hàng
│
├── DataAccess/                      ← Tầng truy cập DB (DAO pattern)
│   ├── DatabaseHelper.cs            ← Quản lý connection string
│   ├── TaiKhoanDAO.cs               ← Xác thực đăng nhập, kiểm tra tên
│   ├── NhanVienDAO.cs               ← Lấy thông tin nhân viên
│   └── KhachHangDAO.cs              ← Lấy thông tin khách hàng
│
├── Common/
│   └── UserSession.cs               ← Lưu thông tin người đang đăng nhập (static)
│
├── Database/
│   ├── CreateDatabase.sql           ← Script tạo bảng
│   └── InsertSampleData.sql         ← Script thêm dữ liệu mẫu
│
├── frmDangNhap.cs/.Designer.cs      ← Form đăng nhập (form đầu tiên)
├── frmMain.cs/.Designer.cs          ← Form chính cho Nhân viên
├── frmKhachHang.cs/.Designer.cs     ← Form dành cho Khách hàng
├── frmSanPham.cs                    ← Form sản phẩm (stub)
└── frmDonHang_HoaDon_BaoCao.cs     ← Form đơn hàng/hóa đơn/báo cáo (stub)
```

---

## 3. Models (Lớp dữ liệu)

### 3.1 Enum `VaiTro` — trong `TaiKhoan.cs`

| Giá trị | Số | Ý nghĩa |
|---------|-----|---------|
| `KhachHang` | 0 | Khách mua hàng |
| `NhanVienBanHang` | 1 | Nhân viên bán hàng |
| `KeToan` | 2 | Kế toán |
| `QuanLy` | 3 | Quản lý (full quyền) |

> **Lưu ý quan trọng:** Trong DB thực tế (`InsertSampleData.sql`), cột `VaiTro` lưu dạng **chuỗi** (`'KhachHang'`, `'NhanVienBanHang'`, `'KeToan'`, `'QuanLy'`). Code C# dùng `TaiKhoan.ParseVaiTro(string)` để chuyển đổi.

### 3.2 Model `TaiKhoan`

| Property | Kiểu | Mô tả |
|----------|------|-------|
| `MaTaiKhoan` | `int` | Khóa chính (IDENTITY) |
| `TenDangNhap` | `string` | Tên đăng nhập (UNIQUE) |
| `MatKhau` | `string` | Mật khẩu (plain text trong test) |
| `VaiTro` | `VaiTro` (enum) | Vai trò người dùng |
| `TrangThai` | `bool` | `true` = hoạt động |

**Phương thức tĩnh:**
- `ParseVaiTro(string)` — Chuỗi DB → enum
- `ToDbString(VaiTro)` — enum → chuỗi DB
- `VaiTroDisplayName` — Property tên tiếng Việt

### 3.3 Model `NhanVien`

| Property | Kiểu | Mô tả |
|----------|------|-------|
| `MaNhanVien` | `int` | Khóa chính |
| `HoTen` | `string` | Họ và tên |
| `Email` | `string` | Email |
| `SoDienThoai` | `string` | Số điện thoại |
| `ChucVu` | `string` | Chức vụ trong công ty |
| `MaTaiKhoan` | `int` | Khóa ngoại → TaiKhoan |

### 3.4 Model `KhachHang`

| Property | Kiểu | Mô tả |
|----------|------|-------|
| `MaKhachHang` | `int` | Khóa chính |
| `HoTen` | `string` | Họ và tên |
| `Email` | `string` | Email |
| `SoDienThoai` | `string` | Số điện thoại |
| `DiaChi` | `string` | Địa chỉ |
| `MaTaiKhoan` | `int` | Khóa ngoại → TaiKhoan |

---

## 4. DataAccess (Tầng DAO)

### 4.1 `DatabaseHelper` (static)

```
Connection String:
  Data Source=(localdb)\MSSQLLocalDB;
  Initial Catalog=QuanLyMayTinh;
  Integrated Security=True;
  TrustServerCertificate=True
```

| Phương thức | Trả về | Mô tả |
|-------------|--------|-------|
| `GetConnection()` | `SqlConnection` | Tạo và mở kết nối DB |
| `TestConnection(out string)` | `bool` | Kiểm tra kết nối, lấy lỗi nếu thất bại |
| `TestConnection()` | `bool` | Overload không cần out param |

### 4.2 `TaiKhoanDAO`

| Phương thức | Trả về | SQL thực hiện |
|-------------|--------|---------------|
| `XacThucDangNhap(tenDangNhap, matKhau)` | `TaiKhoan?` | SELECT WHERE TenDangNhap + MatKhau + TrangThai=1 |
| `TonTaiTenDangNhap(tenDangNhap)` | `bool` | SELECT COUNT(1) WHERE TenDangNhap |

### 4.3 `NhanVienDAO`

| Phương thức | Trả về | Mô tả |
|-------------|--------|-------|
| `GetByMaTaiKhoan(int)` | `NhanVien?` | Lấy 1 nhân viên theo khóa ngoại |
| `GetAll()` | `List<NhanVien>` | Lấy tất cả (ORDER BY HoTen) |

### 4.4 `KhachHangDAO`

| Phương thức | Trả về | Mô tả |
|-------------|--------|-------|
| `GetByMaTaiKhoan(int)` | `KhachHang?` | Lấy 1 khách hàng theo khóa ngoại |
| `GetAll()` | `List<KhachHang>` | Lấy tất cả (ORDER BY HoTen) |

---

## 5. Common — `UserSession` (static, toàn cục)

| Property / Method | Kiểu | Mô tả |
|-------------------|------|-------|
| `CurrentAccount` | `TaiKhoan?` | Tài khoản đang đăng nhập |
| `CurrentNhanVien` | `NhanVien?` | Thông tin nhân viên (null nếu là KH) |
| `CurrentKhachHang` | `KhachHang?` | Thông tin khách hàng (null nếu là NV) |
| `IsLoggedIn` | `bool` | Đã đăng nhập chưa |
| `IsKhachHang` | `bool` | Vai trò = KhachHang |
| `IsNVBanHang` | `bool` | Vai trò = NhanVienBanHang |
| `IsKeToan` | `bool` | Vai trò = KeToan |
| `IsQuanLy` | `bool` | Vai trò = QuanLy |
| `IsNhanVien` | `bool` | Là bất kỳ loại nhân viên nào |
| `DisplayName` | `string` | HoTen ưu tiên, fallback TenDangNhap |
| `Clear()` | `void` | Xóa phiên → đăng xuất |

---

## 6. Forms (Giao diện)

### 6.1 `frmDangNhap` — Form Đăng nhập

> Là form đầu tiên, `Program.cs` chạy `Application.Run(new frmDangNhap())`

**Giao diện:** Màu nền `#0F172A`, có panel card viền gradient

#### Controls:

| Control | Tên | Chức năng |
|---------|-----|-----------|
| `TextBox` | `txtTenDangNhap` | Nhập tên đăng nhập |
| `TextBox` | `txtMatKhau` | Nhập mật khẩu (`PasswordChar = '*'`) |
| `Button` | `btnDangNhap` | Đăng nhập (xanh `#3B82F6`, hover `#2563EB`) |
| `Button` | `btnTogglePassword` | Ẩn/hiện mật khẩu (`👁` / `🙈`) |
| `Button` | `btnThoat` | Thoát ứng dụng |
| `Panel` | `PanelCard` | Khung card (vẽ viền gradient trong sự kiện Paint) |

#### Logic `btnDangNhap_Click`:
1. Validate: không để trống
2. `TaiKhoanDAO.XacThucDangNhap()` → truy vấn DB
3. Lưu `UserSession.CurrentAccount`
4. Nếu là NV/KT/QL → load thêm `NhanVienDAO.GetByMaTaiKhoan()`
5. Nếu là KH → load thêm `KhachHangDAO.GetByMaTaiKhoan()`
6. `ChuyenHuongTheoQuyen()` → ẩn form này, mở form phù hợp

#### Điều hướng theo vai trò:

| Vai trò | Form mở |
|---------|---------|
| KhachHang | `frmKhachHang` |
| NhanVienBanHang / KeToan / QuanLy | `frmMain` |

> Khi đóng form chính: `UserSession.Clear()` → hiện lại `frmDangNhap`

---

### 6.2 `frmMain` — Form Nhân viên

> Mở ở `WindowState = Maximized`. Dùng kiến trúc **MDI-lite**: nhúng form con vào `panelContent`.

#### Layout:
```
┌──────────────────────────────────────────────┐
│  MenuStrip (#1E293B)                         │
├──────────────────────────────────────────────┤
│  panelTop (50px) — Tên NV + Badge vai trò    │
├──────────────────────────────────────────────┤
│                                              │
│         panelContent  (Dock=Fill)            │
│    [Trang chủ / Form con nhúng vào đây]      │
│                                              │
├──────────────────────────────────────────────┤
│  StatusStrip — "© 2025 Quản Lý Bán Máy..."  │
└──────────────────────────────────────────────┘
```

#### Controls cố định:

| Control | Tên | Mô tả |
|---------|-----|-------|
| `MenuStrip` | `menuStrip` | Thanh menu chính |
| `Panel` | `panelTop` | Thanh thông tin người dùng |
| `Label` | `lblTenNguoiDung` | "👤 [Tên]" (font 11pt Bold) |
| `Label` | `lblVaiTro` | Badge màu theo vai trò |
| `Panel` | `panelContent` | Vùng nội dung MDI-lite |
| `StatusStrip` | `statusStrip` | Thanh trạng thái dưới cùng |

#### Badge màu theo vai trò:

| Vai trò | Màu hex | Màu mô tả |
|---------|---------|-----------|
| Quản lý | `#7C3AED` | Tím |
| Kế toán | `#059669` | Xanh lá |
| Nhân viên bán hàng | `#3B82F6` | Xanh dương |

#### Bảng phân quyền menu:

| Menu | NV Bán hàng | Kế toán | Quản lý |
|------|:-----------:|:-------:|:-------:|
| 📦 Hàng hoá (xem) | ✅ | ✅ | ✅ |
| 📦 Hàng hoá (Thêm/Sửa/Xóa) | ❌ | ❌ | ✅ |
| 🚚 Nhập hàng | ❌ | ❌ | ✅ |
| 📋 Đơn hàng (xem) | ✅ | ✅ | ✅ |
| 📋 Đơn hàng (xác nhận) | ✅ | ❌ | ✅ |
| 🧾 Hóa đơn | ❌ | ✅ | ✅ |
| 📊 Tồn kho | ✅ | ✅ | ✅ |
| 📈 Báo cáo | ❌ | ❌ | ✅ |
| 👥 Nhân viên | ❌ | ❌ | ✅ |
| ⚙️ Cài đặt | ❌ | ❌ | ✅ |
| 🚪 Đăng xuất | ✅ | ✅ | ✅ |

#### Sub-items menu:
- **Hàng hoá:** Xem danh sách · Thêm mới · Sửa · Xóa
- **Nhập hàng:** Lập phiếu nhập · Danh sách phiếu
- **Đơn hàng:** Xem tất cả · Xác nhận đơn
- **Hóa đơn:** Lập mới · Danh sách
- **Tồn kho:** Xem tình trạng · Cập nhật
- **Báo cáo:** Doanh thu · Hàng tồn kho · Sản phẩm bán chạy
- **Nhân viên:** Danh sách nhân viên

#### Trang chủ (tạo động, căn giữa tự động):
- `lblWelcome` — "Chào mừng, [Tên]!" (20pt Bold, màu `#63B3ED`)
- `lblHuongDan` — Hướng dẫn theo vai trò (11pt, màu xám `#94A3B8`)
- Cả hai căn giữa qua `PanelContent_Resize` + `CenterTrangChu()`

---

### 6.3 `frmKhachHang` — Form Khách hàng

> Mở ở `WindowState = Maximized`. Là form riêng cho khách (không MDI).

#### Layout:
```
┌──────────────────────────────────────────────┐
│ panelTop (80px, #1E293B)                     │
│  [Chào mừng, Tên! 👋] [🔍 Tìm kiếm] [🚪 DX]│
├──────────────────────────────────────────────┤
│                                              │
│   🖥️ Danh sách sản phẩm sẽ hiển thị ở đây  │
│   Bạn có thể xem thông tin sản phẩm...      │
│                                              │
│      [ 🛒 Đặt mua sản phẩm đã chọn ]        │
│                                              │
└──────────────────────────────────────────────┘
```

#### Controls:

| Control | Tên | Mô tả |
|---------|-----|-------|
| `Panel` | `panelTop` | Header cao 80px |
| `Label` | `lblChaoMung` | "Chào mừng, [Tên]! 👋" (14pt Bold, `#63B3ED`) |
| `TextBox` | `txtTimKiem` | Tìm kiếm (placeholder "🔍 Tìm kiếm sản phẩm...") |
| `Button` | `btnDangXuat` | Đăng xuất (đỏ `#7F1D1D`, luôn cách phải 20px) |
| `Panel` | `panelMain` | Vùng nội dung chính (Fill) |
| `Label` | `lblPlaceholder` | Text placeholder chờ dữ liệu |
| `Button` | `btnDatHang` | "🛒 Đặt mua sản phẩm đã chọn" (xanh `#3B82F6`) |

#### Responsive (không dùng Anchor vì form mở Maximized):
- `PanelTop_Resize` → `btnDangXuat.Left = panelTop.Width - 110 - 20`
- `PanelMain_Resize` → `lblPlaceholder` và `btnDatHang` căn giữa theo công thức `(width - controlWidth) / 2`

---

### 6.4 `frmSanPham` (Stub)

- Param: `cheBoDuyet = false` (xem) / `true` (CRUD)
- Hiện tại: 1 Label `Dock=Fill` căn giữa
- TODO: DataGridView + `SanPhamDAO`

### 6.5 `frmDonHang` (Stub)

- Param: `cheBoDuyet = false` (xem) / `true` (xác nhận)
- Hiện tại: 1 Label `Dock=Fill` căn giữa
- TODO: Hiển thị danh sách đơn hàng + `DonHangDAO`

### 6.6 `frmHoaDon` (Stub)

- Hiện tại: 1 Label `Dock=Fill`
- TODO: Form lập hóa đơn + `HoaDonDAO`

### 6.7 `frmBaoCao` (Stub)

- Param: `loaiBaoCao` = `"DoanhThu"` / `"TonKho"` / `"BanChay"`
- Hiện tại: 1 Label `Dock=Fill`
- TODO: Biểu đồ + query thống kê

---

## 7. Database Schema

### Bảng `TaiKhoan`

| Cột | Kiểu | Ghi chú |
|-----|------|---------|
| `MaTaiKhoan` | `INT IDENTITY(1,1)` | PK |
| `TenDangNhap` | `NVARCHAR(50)` | UNIQUE NOT NULL |
| `MatKhau` | `NVARCHAR(255)` | NOT NULL |
| `VaiTro` | `NVARCHAR` | `'KhachHang'`/`'NhanVienBanHang'`/`'KeToan'`/`'QuanLy'` |
| `TrangThai` | `BIT` | DEFAULT 1 |

### Bảng `KhachHang`

| Cột | Kiểu | Ghi chú |
|-----|------|---------|
| `MaKhachHang` | `INT IDENTITY(1,1)` | PK |
| `HoTen` | `NVARCHAR(100)` | NOT NULL |
| `Email` | `NVARCHAR(100)` | Nullable |
| `SoDienThoai` | `NVARCHAR(15)` | Nullable |
| `DiaChi` | `NVARCHAR(255)` | Nullable |
| `MaTaiKhoan` | `INT UNIQUE` | FK → TaiKhoan |

### Bảng `NhanVien`

| Cột | Kiểu | Ghi chú |
|-----|------|---------|
| `MaNhanVien` | `INT IDENTITY(1,1)` | PK |
| `HoTen` | `NVARCHAR(100)` | NOT NULL |
| `Email` | `NVARCHAR(100)` | Nullable |
| `SoDienThoai` | `NVARCHAR(15)` | Nullable |
| `ChucVu` | `NVARCHAR` | Chức vụ cụ thể |
| `MaTaiKhoan` | `INT UNIQUE` | FK → TaiKhoan |

---

## 8. Tài khoản mẫu (mật khẩu: `123456`)

| Tên đăng nhập | Vai trò | Form vào |
|---------------|---------|---------|
| `khachhang1` | KhachHang | frmKhachHang |
| `nvbanhang1` | NhanVienBanHang | frmMain |
| `ketoan1` | KeToan | frmMain |
| `quanly1` | QuanLy | frmMain (full menu) |

---

## 9. TODO — Chưa implement

| Tính năng | Form liên quan | Ghi chú |
|-----------|---------------|---------|
| Hiển thị danh sách sản phẩm | `frmKhachHang`, `frmSanPham` | Cần `SanPhamDAO` + DataGridView |
| Đặt hàng | `frmKhachHang` | `btnDatHang` chỉ show MessageBox |
| Tìm kiếm sản phẩm | `frmKhachHang` | `txtTimKiem` chưa có event |
| Xác nhận đơn hàng | `frmDonHang` | Cần `DonHangDAO` |
| Lập hóa đơn | `frmHoaDon` | Cần `HoaDonDAO` |
| Báo cáo thống kê | `frmBaoCao` | Cần biểu đồ + query |
| Quản lý nhân viên | Menu Nhân viên | Chưa có form |
| Nhập hàng | Menu Nhập hàng | Chưa có form |
| Hash mật khẩu | `TaiKhoanDAO` | Đang plain text |
| Kết nối DB thực | `DatabaseHelper.cs` | Cần attach file `.mdf`/`.ldf` |
