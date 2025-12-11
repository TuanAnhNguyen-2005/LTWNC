# Hướng dẫn Debug Project

## Cấu hình Ports

### RestFullAPI (.NET Core)
- **HTTPS**: `https://localhost:7264`
- **HTTP**: `http://localhost:5012`
- **Swagger**: `https://localhost:7264/swagger`

### MVC_ADMIN (ASP.NET Framework)
- **HTTPS**: `https://localhost:44319`
- **HTTP**: `http://localhost:44319` (nếu có)

## Cách chạy Debug

### Option 1: Chạy từ Visual Studio Code

1. **Chạy API trước:**
   - Mở file `LTWNC/RestFullAPI/Program.cs`
   - Nhấn F5 hoặc chọn "Run and Debug" → ".NET Core Launch (RestFullAPI)"
   - API sẽ chạy trên `https://localhost:7264`

2. **Chạy MVC Admin:**
   - Mở Visual Studio (hoặc IIS Express)
   - Set `MVC_ADMIN` làm Startup Project
   - Nhấn F5 để chạy
   - MVC sẽ chạy trên `https://localhost:44319`

### Option 2: Chạy Compound (Cả 2 cùng lúc)

- Chọn "Launch All (API + MVC)" trong Run and Debug
- Cả API và MVC sẽ chạy cùng lúc

### Option 3: Chạy từ Visual Studio

1. **Chạy RestFullAPI:**
   - Right-click `RestFullAPI` project → Set as Startup Project
   - Nhấn F5
   - API chạy trên `https://localhost:7264`

2. **Chạy MVC_ADMIN:**
   - Right-click `MVC_ADMIN` project → Set as Startup Project  
   - Nhấn F5
   - MVC chạy trên `https://localhost:44319`

## Kiểm tra kết nối

1. Mở browser và truy cập:
   - API Swagger: `https://localhost:7264/swagger`
   - MVC Admin: `https://localhost:44319`

2. Nếu có lỗi SSL certificate:
   - Click "Advanced" → "Proceed to localhost (unsafe)"
   - Hoặc chạy lệnh: `dotnet dev-certs https --trust`

## Cấu hình API URL

API URL được cấu hình trong:
- `MVC_ADMIN/Web.config` → `appSettings` → `ApiBaseUrl`
- Hiện tại: `https://localhost:7264/api`

Nếu thay đổi port của API, cần cập nhật:
1. `MVC_ADMIN/Web.config`
2. `MVC_ADMIN/Controllers/BaseController.cs` (fallback URL)

## Troubleshooting

### Lỗi "Connection refused"
- Đảm bảo API đã chạy trước khi chạy MVC
- Kiểm tra port có bị conflict không

### Lỗi CORS
- Đã cấu hình CORS trong `Program.cs`
- Kiểm tra origin trong CORS policy

### Lỗi SSL Certificate
```bash
dotnet dev-certs https --trust
```

### Port đã được sử dụng
- Đổi port trong `launchSettings.json` (RestFullAPI)
- Hoặc đổi port trong `MVC_ADMIN.csproj.user` (MVC_ADMIN)

