# Sửa lỗi "Cannot create a file when that file already exists" - Port 44326

## Nguyên nhân
Lỗi này xảy ra khi IIS Express không thể đăng ký URL `https://localhost:44326/` vì:
- Port đã được đăng ký bởi process khác
- ApplicationHost.config bị conflict
- HTTP.sys có reservation cũ

## ✅ Đã thực hiện
1. ✅ Dừng tất cả IIS Express processes
2. ✅ Xóa thư mục `.vs` để Visual Studio tạo lại cấu hình mới

## 🔧 Cách khắc phục tiếp theo

### Cách 1: Restart Visual Studio (Khuyến nghị)
1. **Đóng Visual Studio hoàn toàn**
2. **Mở lại Visual Studio**
3. Mở solution `LTWNC/demo_HocLieu.sln`
4. Right-click **MVC_STUDENT** → **Set as Startup Project**
5. Nhấn **F5**

### Cách 2: Xóa reservation thủ công (nếu Cách 1 không được)
1. Mở **Command Prompt as Administrator**
2. Chạy các lệnh sau:
   ```cmd
   netsh http show urlacl | findstr 44326
   ```
3. Nếu có kết quả, xóa reservation:
   ```cmd
   netsh http delete urlacl url=https://localhost:44326/
   ```
4. Hoặc xóa tất cả reservations của IIS Express:
   ```cmd
   netsh http delete urlacl url=https://+:44326/
   ```

### Cách 3: Thay đổi port
1. Trong Visual Studio, Right-click project **MVC_STUDENT**
2. Chọn **Properties**
3. Tab **Web**
4. Thay đổi **Project Url** thành port khác, ví dụ: `https://localhost:44327/`
5. Nhấn **Create Virtual Directory**
6. Save và thử chạy lại

### Cách 4: Reset IIS Express configuration
1. Đóng Visual Studio
2. Xóa thư mục `.vs` trong solution folder (đã làm)
3. Xóa thư mục `%USERPROFILE%\Documents\IISExpress\config` (nếu có)
4. Mở lại Visual Studio

## 🔍 Kiểm tra sau khi sửa
1. Build solution (Ctrl+Shift+B)
2. Set MVC_STUDENT làm Startup Project
3. Nhấn F5
4. Kiểm tra Output window xem có lỗi gì không

## ⚠️ Lưu ý
- Đảm bảo đã đóng tất cả cửa sổ Visual Studio trước khi xóa `.vs` folder
- Nếu vẫn lỗi, thử restart máy tính
- Port 44326 có thể bị sử dụng bởi ứng dụng khác - kiểm tra bằng `netstat -ano | findstr :44326`



