# Hướng dẫn sửa lỗi ERR_CONNECTION_RESET - Chi tiết

## Đã thực hiện

1. ✅ Thêm error handling trong `Global.asax.cs` để catch và log lỗi
2. ✅ Bật `customErrors mode="Off"` trong `Web.config` để xem lỗi chi tiết
3. ✅ Comment out code tạo user mẫu để tránh lỗi database khi khởi động

## Các bước khắc phục

### Bước 1: Build lại project
1. Mở Visual Studio
2. Mở solution `LTWNC/demo_HocLieu.sln`
3. Right-click solution → **Clean Solution**
4. Right-click solution → **Rebuild Solution**
5. Kiểm tra xem có lỗi compilation không

### Bước 2: Kiểm tra Output window
1. Trong Visual Studio, mở **View → Output** (Ctrl+Alt+O)
2. Chọn **Show output from: Debug**
3. Chạy ứng dụng (F5)
4. Xem lỗi chi tiết trong Output window

### Bước 3: Chạy ứng dụng từ Visual Studio
1. Right-click project `MVC_ADMIN` → **Set as Startup Project**
2. Nhấn **F5** hoặc click **Start**
3. Nếu có lỗi, sẽ hiển thị trang lỗi chi tiết (vì đã bật customErrors mode="Off")

### Bước 4: Kiểm tra Event Viewer (nếu vẫn lỗi)
1. Mở **Event Viewer** (Windows + R → `eventvwr`)
2. Vào **Windows Logs → Application**
3. Tìm lỗi liên quan đến IIS Express hoặc ASP.NET

## Các lỗi thường gặp và cách sửa

### Lỗi: "Could not load file or assembly"
- **Nguyên nhân**: Thiếu DLL hoặc version không khớp
- **Giải pháp**: 
  - Clean và Rebuild solution
  - Kiểm tra packages.config và restore packages (NuGet)

### Lỗi: "Database connection failed"
- **Nguyên nhân**: Connection string không đúng hoặc SQL Server không chạy
- **Giải pháp**: 
  - Kiểm tra SQL Server đang chạy: `services.msc` → SQL Server (MSSQLSERVER)
  - Test connection bằng SQL Server Management Studio
  - Kiểm tra connection string trong Web.config

### Lỗi: "Port already in use"
- **Nguyên nhân**: Port 44320 đang được sử dụng
- **Giải pháp**:
  ```powershell
  # Tìm process sử dụng port
  netstat -ano | findstr :44320
  
  # Kill process (thay <PID> bằng Process ID)
  taskkill /PID <PID> /F
  ```

### Lỗi: "The system cannot find the file specified"
- **Nguyên nhân**: Thiếu file trong bin folder
- **Giải pháp**: Clean và Rebuild solution

## Kiểm tra nhanh

Sau khi build, kiểm tra:
- ✅ Thư mục `bin` có chứa file DLL không
- ✅ Không có lỗi trong Error List (View → Error List)
- ✅ IIS Express khởi động được (xem Output window)

## Nếu vẫn không được

1. Xóa thư mục `bin` và `obj`:
   ```powershell
   cd LTWNC\MVC_ADMIN
   Remove-Item -Recurse -Force bin, obj
   ```
2. Build lại từ Visual Studio
3. Xem chi tiết lỗi trong Output window hoặc trang lỗi khi chạy

