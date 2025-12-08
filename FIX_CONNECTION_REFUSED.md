# 🔧 Sửa lỗi ERR_CONNECTION_REFUSED

## ❌ Vấn đề
Khi truy cập `http://localhost:64761`, gặp lỗi:
```
ERR_CONNECTION_REFUSED
localhost đã từ chối kết nối
```

## ✅ Nguyên nhân
IIS Express **chưa chạy** hoặc **không khởi động được**.

## 🔧 Giải pháp

### Cách 1: Chạy script thủ công (Khuyến nghị)

1. **Mở PowerShell** (Run as Administrator nếu cần)
2. **Chạy script:**
   ```powershell
   cd C:\Users\thanh\source\repos\Web_CC\LTWNC
   .\start-mvc-admin.ps1
   ```
3. **Đợi thông báo:**
   ```
   ✅ IIS Express đã khởi động
   ✅ Port 64761 đã sẵn sàng
   ✅ Đã mở Chrome với MVC Admin
   ```
4. **Kiểm tra:**
   - Chrome tự động mở với URL `http://localhost:64761`
   - Nếu không tự mở, mở thủ công: `http://localhost:64761`

### Cách 2: Chạy từ Cursor/VS Code

1. **Nhấn Ctrl+Shift+P**
2. **Gõ:** `Tasks: Run Task`
3. **Chọn:** `start-mvc-admin`
4. **Xem terminal** để kiểm tra IIS Express đã chạy chưa

### Cách 3: Chạy IIS Express thủ công

1. **Tìm IIS Express:**
   ```powershell
   # Thường ở đây:
   C:\Program Files\IIS Express\iisexpress.exe
   # hoặc
   C:\Program Files (x86)\IIS Express\iisexpress.exe
   ```

2. **Chạy lệnh:**
   ```powershell
   cd C:\Users\thanh\source\repos\Web_CC\LTWNC\MVC_ADMIN
   & "C:\Program Files\IIS Express\iisexpress.exe" /path:"C:\Users\thanh\source\repos\Web_CC\LTWNC\MVC_ADMIN" /port:64761 /clr:v4.0
   ```

3. **Mở Chrome:** `http://localhost:64761`

## 🔍 Kiểm tra IIS Express đang chạy

### Kiểm tra process:
```powershell
Get-Process -Name "iisexpress" -ErrorAction SilentlyContinue
```

### Kiểm tra port:
```powershell
netstat -ano | findstr ":64761"
```

Nếu có kết quả → IIS Express đang chạy ✅
Nếu không có → IIS Express chưa chạy ❌

## ⚠️ Lỗi thường gặp

### 1. "IIS Express not found"
**Giải pháp:**
- Cài đặt Visual Studio (bao gồm IIS Express)
- Hoặc cài IIS Express riêng từ Microsoft

### 2. "Port already in use"
**Giải pháp:**
```powershell
# Tìm process đang dùng port
netstat -ano | findstr ":64761"

# Kill process (thay <PID> bằng process ID)
taskkill /PID <PID> /F
```

### 3. "Access denied"
**Giải pháp:**
- Chạy PowerShell **as Administrator**
- Hoặc chạy Cursor/VS Code **as Administrator**

### 4. Script chạy nhưng IIS Express không khởi động
**Giải pháp:**
1. Kiểm tra IIS Express có tồn tại:
   ```powershell
   Test-Path "C:\Program Files\IIS Express\iisexpress.exe"
   ```
2. Chạy script với verbose:
   ```powershell
   cd LTWNC
   .\start-mvc-admin.ps1 -Verbose
   ```
3. Xem lỗi trong terminal

## ✅ Kiểm tra thành công

Sau khi chạy script, bạn sẽ thấy:
- ✅ IIS Express process đang chạy
- ✅ Port 64761 đang lắng nghe
- ✅ Chrome mở với URL `http://localhost:64761`
- ✅ Trang MVC Admin hiển thị

## 💡 Mẹo

1. **Luôn chạy script từ thư mục LTWNC:**
   ```powershell
   cd LTWNC
   .\start-mvc-admin.ps1
   ```

2. **Nếu script không chạy, thử:**
   ```powershell
   Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
   ```

3. **Để dừng IIS Express:**
   - Nhấn **Ctrl+C** trong terminal đang chạy script
   - Hoặc:
     ```powershell
     Get-Process -Name "iisexpress" | Stop-Process -Force
     ```

## 🎯 Quick Fix

**Cách nhanh nhất:**
```powershell
cd C:\Users\thanh\source\repos\Web_CC\LTWNC
.\start-mvc-admin.ps1
```

Sau đó mở Chrome: `http://localhost:64761`


