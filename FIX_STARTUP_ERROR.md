# Hướng dẫn sửa lỗi "ERR_CONNECTION_RESET"

## Nguyên nhân
Lỗi này xảy ra khi ứng dụng MVC_ADMIN crash khi khởi động, thường do:
- Lỗi kết nối database trong `Application_Start`
- Exception không được handle đúng cách

## Đã sửa
Đã tạm thời comment out phần code tạo user mẫu trong `Global.asax.cs` để ứng dụng có thể khởi động mà không cần kết nối database ngay từ đầu.

## Cách khởi động lại ứng dụng

### Cách 1: Từ Visual Studio (Khuyến nghị)
1. Mở Visual Studio
2. Mở solution `demo_HocLieu.sln`
3. Right-click project `MVC_ADMIN` → **Set as Startup Project**
4. Nhấn **F5** hoặc click **Start**

### Cách 2: Từ PowerShell
```powershell
cd LTWNC
.\start-mvc-admin.ps1
```

### Cách 3: Restart IIS Express thủ công
1. Đóng tất cả cửa sổ IIS Express
2. Trong Visual Studio, nhấn **Ctrl+Shift+B** để build
3. Sau đó nhấn **F5** để chạy

## Kiểm tra kết quả
Sau khi khởi động, mở browser và truy cập:
- HTTP: `http://localhost:64761`
- HTTPS: `https://localhost:44320` hoặc `https://localhost:44319`

## Sau khi ứng dụng chạy được
Nếu muốn bật lại tính năng tạo user mẫu, uncomment phần code trong `Global.asax.cs` sau khi đã đảm bảo database connection hoạt động đúng.


