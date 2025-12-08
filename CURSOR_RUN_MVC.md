# 🚀 Hướng dẫn chạy MVC từ Cursor (VS Code)

## ✅ Cách 1: Nhấn F5 và chọn configuration

### Bước 1: Mở Run and Debug
- Nhấn **F5** hoặc **Ctrl+Shift+D**
- Hoặc click vào icon **Run and Debug** ở sidebar bên trái

### Bước 2: Chọn configuration
Trong dropdown ở trên cùng, chọn một trong các options:

1. **🌐 Launch MVC Admin (IIS Express + Chrome)**
   - Chỉ chạy MVC Admin
   - Tự động mở Chrome
   - URL: `http://localhost:64761`

2. **🚀 Launch All (API + MVC + Chrome)**
   - Chạy cả API và MVC Admin
   - Tự động mở Chrome
   - API: `https://localhost:7264`
   - MVC: `http://localhost:64761`

3. **🚀 Launch API Only (MVC chạy từ Visual Studio)**
   - Chỉ chạy API
   - MVC cần chạy từ Visual Studio riêng

### Bước 3: Nhấn F5
- Nhấn **F5** hoặc click nút **▶️ Start Debugging**
- Chrome sẽ tự động mở sau vài giây

## ✅ Cách 2: Chạy từ Terminal

### Chạy MVC Admin:
```powershell
cd LTWNC
.\start-mvc-admin.ps1
```

Script sẽ:
- ✅ Khởi động IIS Express
- ✅ Tự động mở Chrome với URL `http://localhost:64761`
- ✅ Hiển thị thông tin ports

## 📝 Cấu hình Ports

| Service | HTTP | HTTPS |
|---------|------|-------|
| MVC_ADMIN | `http://localhost:64761` | `https://localhost:44319` |
| RestFullAPI | `http://localhost:5012` | `https://localhost:7264` |

## 🎯 Các Configuration có sẵn

### 1. Launch MVC Admin
- **Tên**: `🌐 Launch MVC Admin (IIS Express + Chrome)`
- **Mô tả**: Chạy MVC Admin với IIS Express và tự động mở Chrome
- **URL**: `http://localhost:64761`

### 2. Launch All
- **Tên**: `🚀 Launch All (API + MVC + Chrome)`
- **Mô tả**: Chạy cả API và MVC Admin cùng lúc
- **URLs**: 
  - API: `https://localhost:7264/swagger`
  - MVC: `http://localhost:64761`

### 3. Launch API Only
- **Tên**: `🚀 Launch API Only (MVC chạy từ Visual Studio)`
- **Mô tả**: Chỉ chạy API, MVC chạy từ Visual Studio riêng

## ⚠️ Lưu ý

1. **IIS Express cần được cài đặt**
   - Cài đặt Visual Studio (bao gồm IIS Express)
   - Hoặc cài đặt IIS Express riêng

2. **Port đã được sử dụng**
   - Nếu port 64761 đã được sử dụng, script sẽ báo lỗi
   - Giải pháp: Đóng ứng dụng đang dùng port đó

3. **Chrome không tự mở**
   - Mở thủ công Chrome và truy cập: `http://localhost:64761`

## 🔧 Troubleshooting

### Lỗi: "IIS Express not found"
**Giải pháp:**
- Cài đặt Visual Studio (bao gồm IIS Express)
- Hoặc cài đặt IIS Express riêng từ Microsoft

### Lỗi: "Port already in use"
**Giải pháp:**
```powershell
# Tìm process đang dùng port
netstat -ano | findstr :64761

# Kill process (thay <PID> bằng process ID)
taskkill /PID <PID> /F
```

### Lỗi: "Cannot find project file"
**Giải pháp:**
- Đảm bảo bạn đang ở đúng workspace root
- Kiểm tra đường dẫn: `LTWNC/MVC_ADMIN/MVC_ADMIN.csproj`

## 💡 Mẹo

- **Nhấn F5** là cách nhanh nhất để chạy
- Chọn configuration từ dropdown ở trên cùng
- Chrome sẽ tự động mở sau 5-6 giây
- Để dừng, nhấn **Shift+F5** hoặc click nút **Stop**

## 🎉 Kết quả

Sau khi chạy thành công:
- ✅ IIS Express đang chạy
- ✅ Chrome tự động mở với MVC Admin
- ✅ Có thể truy cập: `http://localhost:64761`
- ✅ Có thể truy cập: `https://localhost:44319` (HTTPS)

