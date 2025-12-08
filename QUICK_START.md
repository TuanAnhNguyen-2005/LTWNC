# 🚀 Hướng dẫn nhanh - Chạy Project

## ⚠️ LƯU Ý QUAN TRỌNG

**MVC_ADMIN (ASP.NET Framework) cần chạy từ Visual Studio**, không thể chạy trực tiếp từ VS Code với IIS Express.

## ✅ Cách chạy đúng (Khuyến nghị)

### Bước 1: Chạy RestFullAPI từ VS Code
1. Mở VS Code
2. Nhấn **F5**
3. Chọn **".NET Core Launch (RestFullAPI)"**
4. API sẽ chạy trên `https://localhost:7264`

### Bước 2: Chạy MVC_ADMIN từ Visual Studio
1. Mở **Visual Studio**
2. Mở solution `LTWNC/demo_HocLieu.sln`
3. Right-click `MVC_ADMIN` project → **Set as Startup Project**
4. Nhấn **F5**
5. MVC sẽ chạy trên `https://localhost:44319` hoặc `http://localhost:64761`

### Bước 3: Mở Chrome
- Tự động mở hoặc mở thủ công:
  - MVC Admin: `http://localhost:64761` hoặc `https://localhost:44319`
  - API Swagger: `https://localhost:7264/swagger`

## 🔄 Cách chạy cả 2 cùng lúc (Visual Studio)

1. Mở Visual Studio
2. Right-click **Solution** → **Properties**
3. Chọn **Multiple startup projects**
4. Set:
   - `RestFullAPI` → **Start**
   - `MVC_ADMIN` → **Start**
5. Nhấn **OK** và **F5**

## 📝 Ports

| Service | HTTP | HTTPS |
|---------|------|-------|
| RestFullAPI | `http://localhost:5012` | `https://localhost:7264` |
| MVC_ADMIN | `http://localhost:64761` | `https://localhost:44319` |

## ❌ Nếu gặp lỗi

### Lỗi: "Connection refused" hoặc "ERR_CONNECTION_REFUSED"
- **Nguyên nhân**: IIS Express chưa chạy
- **Giải pháp**: Chạy MVC_ADMIN từ Visual Studio (không phải VS Code)

### Lỗi: "SSL Protocol Error"
- **Giải pháp**: Dùng HTTP thay vì HTTPS (`http://localhost:64761`)

### Lỗi: "Port already in use"
- **Giải pháp**: 
  ```powershell
  # Tìm process
  netstat -ano | findstr :64761
  # Kill process
  taskkill /PID <process_id> /F
  ```

## 💡 Tại sao không chạy được từ VS Code?

- MVC_ADMIN là **ASP.NET Framework** (không phải .NET Core)
- IIS Express cần được quản lý bởi Visual Studio
- VS Code không có đầy đủ tool để chạy ASP.NET Framework với IIS Express

## ✅ Giải pháp tốt nhất

**Chạy từ Visual Studio** - Đây là cách đúng và ổn định nhất cho ASP.NET Framework projects.

