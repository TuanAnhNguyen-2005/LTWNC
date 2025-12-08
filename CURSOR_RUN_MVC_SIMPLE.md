# 🚀 Cách chạy MVC từ Cursor - Đơn giản nhất

## ✅ Cách 1: Chạy từ Run and Debug (F5)

### Bước 1: Nhấn F5
- Nhấn **F5** hoặc **Ctrl+Shift+D**
- Hoặc click icon **Run and Debug** ở sidebar

### Bước 2: Chọn configuration
Trong dropdown, chọn:
- **🚀 Launch All (API + MVC + Chrome)** - Chạy cả API và MVC
- **🚀 Launch API Only** - Chỉ chạy API

### Bước 3: Chờ và mở Chrome
- Sau 5-6 giây, Chrome sẽ tự động mở
- URL: `http://localhost:64761`

## ✅ Cách 2: Chạy Task trực tiếp (Khuyến nghị cho MVC)

### Bước 1: Mở Command Palette
- Nhấn **Ctrl+Shift+P** (hoặc **F1**)

### Bước 2: Chọn Task
- Gõ: `Tasks: Run Task`
- Chọn: **`start-mvc-admin`**

### Bước 3: Chờ và mở Chrome
- IIS Express sẽ khởi động
- Chrome tự động mở sau 5-6 giây
- URL: `http://localhost:64761`

## ✅ Cách 3: Chạy từ Terminal

### Mở Terminal
- Nhấn **Ctrl+`** (backtick)
- Hoặc **Terminal** → **New Terminal**

### Chạy script:
```powershell
cd LTWNC
.\start-mvc-admin.ps1
```

## 📋 Danh sách Tasks có sẵn

Để xem tất cả tasks:
1. Nhấn **Ctrl+Shift+P**
2. Gõ: `Tasks: Run Task`
3. Xem danh sách:
   - `start-mvc-admin` - Chạy MVC Admin với IIS Express
   - `open-chrome-mvc` - Mở Chrome với MVC URL
   - `run-mvc-admin` - Chạy cả 2 tasks trên (khuyến nghị)

## 🎯 Cách nhanh nhất

**Nhấn Ctrl+Shift+P** → Gõ `task` → Chọn **`run-mvc-admin`**

Hoặc:

**Nhấn F5** → Chọn **🚀 Launch All (API + MVC + Chrome)**

## 📝 URLs

| Service | URL |
|---------|-----|
| MVC Admin (HTTP) | `http://localhost:64761` |
| MVC Admin (HTTPS) | `https://localhost:44319` |
| API Swagger | `https://localhost:7264/swagger` |

## ⚠️ Lưu ý

1. **IIS Express cần được cài đặt**
   - Cài Visual Studio (bao gồm IIS Express)
   - Hoặc cài IIS Express riêng

2. **Port đã được sử dụng**
   - Nếu port 64761 đã được dùng, script sẽ báo lỗi
   - Giải pháp: Đóng ứng dụng đang dùng port

3. **Chrome không tự mở**
   - Mở thủ công: `http://localhost:64761`

## 🔧 Troubleshooting

### Lỗi: "IIS Express not found"
**Giải pháp:**
- Cài Visual Studio hoặc IIS Express

### Lỗi: "Port already in use"
```powershell
# Tìm process
netstat -ano | findstr :64761

# Kill process (thay <PID>)
taskkill /PID <PID> /F
```

## 💡 Mẹo

- **Ctrl+Shift+P** → `task` → Chọn task là cách nhanh nhất
- **F5** → Chọn configuration để chạy cả API + MVC
- Chrome tự động mở sau 5-6 giây
- Để dừng, đóng terminal hoặc nhấn **Ctrl+C**


