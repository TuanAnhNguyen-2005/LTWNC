# ✅ Đã sửa toàn bộ các project MVC

## 📋 Tóm tắt các thay đổi

Đã áp dụng các sửa đổi sau cho **tất cả 3 project MVC**:
- ✅ MVC_ADMIN
- ✅ MVC_STUDENT  
- ✅ MVC_Teacher

## 🔧 Các sửa đổi đã thực hiện

### 1. Thêm Error Handling trong Global.asax.cs

**Đã thêm:**
- ✅ Try-catch trong `Application_Start()` để bắt và log lỗi khi khởi động
- ✅ `Application_Error()` handler để log mọi exception xảy ra
- ✅ Log chi tiết: Message, InnerException, StackTrace

**File đã sửa:**
- `MVC_ADMIN/Global.asax.cs`
- `MVC_STUDENT/Global.asax.cs`
- `MVC_Teacher/Global.asax.cs`

### 2. Bật CustomErrors để xem lỗi chi tiết

**Đã thêm:**
- ✅ `<customErrors mode="Off" />` vào Web.config của tất cả project

**File đã sửa:**
- `MVC_ADMIN/Web.config`
- `MVC_STUDENT/Web.config`
- `MVC_Teacher/Web.config`

### 3. Tắt code tạo User mẫu khi khởi động

**Đã comment out:**
- ✅ Code tạo user mẫu trong `Application_Start()` để tránh lỗi database khi khởi động

**File đã sửa:**
- `MVC_ADMIN/Global.asax.cs` - đã comment từ trước
- `MVC_Teacher/Global.asax.cs` - đã comment mới

**Lưu ý:** MVC_STUDENT không có code tạo user mẫu nên không cần sửa.

## 🚀 Cách chạy lại các project

### Cách 1: Từ Visual Studio (Khuyến nghị)

1. **Mở Visual Studio**
2. **Mở solution** `LTWNC/demo_HocLieu.sln`
3. **Clean và Rebuild:**
   - Right-click Solution → **Clean Solution**
   - Right-click Solution → **Rebuild Solution**
4. **Chạy project:**
   - Right-click project cần chạy (MVC_ADMIN/MVC_STUDENT/MVC_Teacher)
   - Chọn **Set as Startup Project**
   - Nhấn **F5** hoặc click **Start**

### Cách 2: Build từng project riêng

```powershell
# Build MVC_ADMIN
cd LTWNC\MVC_ADMIN
# Chạy từ Visual Studio: F5

# Build MVC_STUDENT  
cd LTWNC\MVC_STUDENT
# Chạy từ Visual Studio: F5

# Build MVC_Teacher
cd LTWNC\MVC_Teacher
# Chạy từ Visual Studio: F5
```

## 🔍 Xem lỗi chi tiết

### Trong Visual Studio:
1. Mở **View → Output** (Ctrl+Alt+O)
2. Chọn **Show output from: Debug**
3. Xem các dòng log bắt đầu bằng:
   - "Error in Application_Start: ..."
   - "Application Error: ..."

### Trong Browser:
Vì đã bật `customErrors mode="Off"`, nếu có lỗi sẽ hiển thị:
- ✅ **Yellow Screen of Death (YSOD)** với chi tiết lỗi đầy đủ
- ✅ Stack trace đầy đủ
- ✅ Inner exception (nếu có)

## ✅ Kiểm tra kết quả

Sau khi build và chạy, kiểm tra:

1. ✅ **Build thành công** - Không có lỗi trong Error List
2. ✅ **IIS Express khởi động** - Xem Output window
3. ✅ **Browser mở được** - Không còn lỗi ERR_CONNECTION_RESET
4. ✅ **Trang web hiển thị** - Hoặc hiển thị lỗi chi tiết (nếu có)

## 📝 Các URL mặc định

| Project | HTTP | HTTPS |
|---------|------|-------|
| MVC_ADMIN | `http://localhost:64761` | `https://localhost:44320` hoặc `44319` |
| MVC_STUDENT | `http://localhost:[port]` | `https://localhost:[port]` |
| MVC_Teacher | `http://localhost:[port]` | `https://localhost:[port]` |

*Port có thể khác tùy cấu hình trong project*

## 🔄 Sau khi ứng dụng chạy được

Nếu muốn bật lại tính năng tạo user mẫu:
1. Uncomment phần code trong `Global.asax.cs`
2. Đảm bảo database connection string đúng
3. Đảm bảo SQL Server đang chạy
4. Test kết nối database trước

## ❓ Vẫn gặp lỗi?

1. **Kiểm tra Output window** trong Visual Studio để xem lỗi chi tiết
2. **Kiểm tra Error List** (View → Error List) để xem lỗi compilation
3. **Kiểm tra Event Viewer** (Windows + R → `eventvwr`) → Windows Logs → Application
4. **Clean và Rebuild** lại solution

