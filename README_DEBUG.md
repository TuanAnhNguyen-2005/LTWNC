# 🚀 Hướng dẫn Debug - Chạy API + MVC Admin cùng lúc

## Cách 1: Sử dụng VS Code (Khuyến nghị)

### Bước 1: Mở VS Code
1. Mở thư mục project trong VS Code
2. Nhấn `F5` hoặc vào **Run and Debug** (Ctrl+Shift+D)
3. Chọn **"🚀 Launch All (API + MVC + Chrome)"**
4. Nhấn F5

### Kết quả:
- ✅ RestFullAPI sẽ chạy trên `https://localhost:7264`
- ✅ MVC_ADMIN sẽ chạy trên `https://localhost:44319` (qua IIS Express)
- ✅ Chrome sẽ tự động mở 2 tab:
  - Tab 1: MVC Admin (`https://localhost:44319`)
  - Tab 2: API Swagger (`https://localhost:7264/swagger`)

## Cách 2: Sử dụng PowerShell Script

### Chạy script tự động:
```powershell
cd LTWNC
.\start-all.ps1
```

Script này sẽ:
1. Start RestFullAPI
2. Start MVC_ADMIN với IIS Express
3. Tự động mở Chrome với cả 2 URL

### Dừng tất cả:
Nhấn `Ctrl+C` trong PowerShell window

## Cách 3: Chạy thủ công từ Visual Studio

### Bước 1: Chạy RestFullAPI
1. Right-click `RestFullAPI` project → **Set as Startup Project**
2. Nhấn `F5`
3. API chạy trên `https://localhost:7264`

### Bước 2: Chạy MVC_ADMIN (trong Visual Studio mới)
1. Right-click `MVC_ADMIN` project → **Set as Startup Project**
2. Nhấn `F5`
3. MVC chạy trên `https://localhost:44319`

### Bước 3: Mở Chrome
- Mở Chrome và truy cập:
  - `https://localhost:44319` (MVC Admin)
  - `https://localhost:7264/swagger` (API)

## Cấu hình Ports

| Service | URL | Port |
|---------|-----|------|
| RestFullAPI (HTTPS) | `https://localhost:7264` | 7264 |
| RestFullAPI (HTTP) | `http://localhost:5012` | 5012 |
| MVC_ADMIN | `https://localhost:44319` | 44319 |

## Troubleshooting

### ❌ Lỗi: "IIS Express not found"
**Giải pháp:**
- Cài đặt Visual Studio (bao gồm IIS Express)
- Hoặc cài đặt IIS Express riêng
- Hoặc chạy MVC_ADMIN từ Visual Studio thay vì VS Code

### ❌ Lỗi: "Port already in use"
**Giải pháp:**
1. Tìm process đang dùng port:
   ```powershell
   netstat -ano | findstr :7264
   netstat -ano | findstr :44319
   ```
2. Kill process:
   ```powershell
   taskkill /PID <process_id> /F
   ```

### ❌ Lỗi: SSL Certificate
**Giải pháp:**
```bash
dotnet dev-certs https --trust
```

### ❌ Chrome không tự mở
**Giải pháp:**
- Mở thủ công Chrome và truy cập:
  - `https://localhost:44319`
  - `https://localhost:7264/swagger`

### ❌ MVC_ADMIN không chạy từ VS Code
**Giải pháp:**
- Chạy MVC_ADMIN từ Visual Studio riêng
- Hoặc dùng PowerShell script: `.\start-all.ps1`

## Lưu ý

1. **Luôn chạy API trước** khi chạy MVC Admin (để tránh lỗi kết nối)
2. **Chờ 5-10 giây** sau khi start để cả 2 service sẵn sàng
3. Nếu dùng **VS Code**, compound configuration sẽ tự động chạy cả 2
4. Nếu dùng **Visual Studio**, cần mở 2 instance riêng hoặc dùng Multiple Startup Projects

## Multiple Startup Projects trong Visual Studio

1. Right-click Solution → **Properties**
2. Chọn **Multiple startup projects**
3. Set cả 2 projects:
   - `RestFullAPI` → **Start**
   - `MVC_ADMIN` → **Start**
4. Nhấn OK và F5

