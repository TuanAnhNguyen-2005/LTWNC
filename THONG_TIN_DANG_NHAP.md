# 🔐 Thông tin đăng nhập - Tài khoản mẫu

## 📋 Danh sách tài khoản

Tất cả tài khoản đều có **mật khẩu: `123456`**

### 👨‍💼 Admin (Quản trị viên)
- **Email:** `vana@example.com`
- **Mật khẩu:** `123456`
- **Họ tên:** Nguyễn Văn A
- **Vai trò:** Admin
- **Quyền:** Toàn quyền quản lý hệ thống

### 👩‍🏫 Giảng viên (Teacher)
**Tài khoản 1:**
- **Email:** `thib@example.com`
- **Mật khẩu:** `123456`
- **Họ tên:** Trần Thị B
- **Vai trò:** Giảng viên

**Tài khoản 2:**
- **Email:** `vanc@example.com`
- **Mật khẩu:** `123456`
- **Họ tên:** Lê Văn C
- **Vai trò:** Giảng viên

### 👨‍🎓 Sinh viên (Student)
**Tài khoản 1:**
- **Email:** `thid@example.com`
- **Mật khẩu:** `123456`
- **Họ tên:** Phạm Thị D
- **Vai trò:** Sinh viên

**Tài khoản 2:**
- **Email:** `vane@example.com`
- **Mật khẩu:** `123456`
- **Họ tên:** Hoàng Văn E
- **Vai trò:** Sinh viên

## 🚀 Cách sử dụng

1. Truy cập trang đăng nhập: `http://localhost:64761/Login` hoặc `https://localhost:44319/Login`
2. Nhập **Email** và **Mật khẩu** từ danh sách trên
3. Sau khi đăng nhập, hệ thống sẽ tự động chuyển hướng theo vai trò:
   - **Admin** → `/Admin` (Dashboard Admin)
   - **Giảng viên** → `/Teacher/Dashboard`
   - **Sinh viên** → `/Student/Dashboard`

## ⚠️ Lưu ý

- Đây là **tài khoản mẫu** cho môi trường development
- **KHÔNG** sử dụng các mật khẩu này trong production
- Nếu chưa có dữ liệu, chạy file `NenTangHocLieu.sql` để tạo database và dữ liệu mẫu

## 📝 Tạo tài khoản mới

Bạn có thể tạo tài khoản mới bằng cách:
1. Truy cập trang đăng ký: `/SignUp`
2. Điền thông tin và chọn vai trò (Admin/Teacher/Student)
3. Hoặc thêm trực tiếp vào database bằng SQL

