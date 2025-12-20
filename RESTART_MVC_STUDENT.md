# Hướng dẫn khởi động lại MVC_STUDENT (Port 44326)

## Tình trạng hiện tại
- Port 44326 đang bị chiếm bởi System process (PID 4)
- IIS Express có thể đang crash khi khởi động
- Đã sửa code để thêm error handling

## Cách khắc phục

### Bước 1: Build lại project trong Visual Studio
1. Mở **Visual Studio**
2. Mở solution `LTWNC/demo_HocLieu.sln`
3. **Clean Solution:**
   - Right-click Solution → **Clean Solution**
4. **Rebuild Solution:**
   - Right-click Solution → **Rebuild Solution**
5. Kiểm tra **Error List** (View → Error List) - đảm bảo không có lỗi

### Bước 2: Set MVC_STUDENT làm Startup Project
1. Right-click project **MVC_STUDENT** → **Set as Startup Project**
2. Kiểm tra port trong Properties:
   - Right-click MVC_STUDENT → Properties
   - Web tab → Project URL: `https://localhost:44326/`

### Bước 3: Chạy ứng dụng
1. Nhấn **F5** hoặc click **Start** (▶️)
2. Xem **Output window** (View → Output, Ctrl+Alt+O) để kiểm tra lỗi
3. Chọn "Show output from: Debug"

### Bước 4: Nếu vẫn lỗi
1. Xem **Output window** để tìm lỗi chi tiết
2. Hoặc xem trang lỗi trong browser (vì đã bật customErrors mode="Off")
3. Kiểm tra lỗi và sửa tương ứng

## Đã sửa trong code
✅ Thêm error handling trong `Global.asax.cs`
✅ Bật `customErrors mode="Off"` trong `Web.config`
✅ Connection string đã được cập nhật thành CUATUI

## Nếu port vẫn bị chiếm
```powershell
# Tìm process sử dụng port 44326
netstat -ano | findstr :44326

# Kill process (thay <PID> bằng Process ID)
taskkill /PID <PID> /F

# Hoặc restart IIS Express
taskkill /F /IM iisexpress.exe
```

## Kiểm tra kết quả
Sau khi chạy thành công:
- ✅ Browser sẽ mở với URL `https://localhost:44326`
- ✅ Hoặc hiển thị trang lỗi chi tiết nếu có lỗi
- ✅ Không còn lỗi ERR_CONNECTION_RESET

