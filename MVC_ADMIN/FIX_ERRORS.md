# 🔧 Hướng dẫn sửa lỗi Build trong Visual Studio

## ❌ Các lỗi thường gặp:
- `The type or namespace name 'Filters' does not exist`
- `The type or namespace name 'BaseController' could not be found`
- `The type or namespace name 'AuthorizeRoleAttribute' could not be found`

## ✅ Giải pháp (Thực hiện theo thứ tự):

### Bước 1: Unload và Reload Project
1. Right-click `MVC_ADMIN` project trong Solution Explorer
2. Chọn **Unload Project**
3. Right-click lại → **Reload Project**

### Bước 2: Clean Solution
1. Menu **Build** → **Clean Solution**
2. Đợi quá trình clean hoàn tất

### Bước 3: Rebuild Solution
1. Menu **Build** → **Rebuild Solution**
2. Đợi quá trình build hoàn tất

### Bước 4: Kiểm tra các file đã được include
1. Right-click `MVC_ADMIN` project → **Properties**
2. Tab **Application** → Kiểm tra **Default namespace** = `MVC_ADMIN`
3. Trong Solution Explorer, kiểm tra các folder:
   - ✅ `Controllers/BaseController.cs`
   - ✅ `Helpers/ConfigurationHelper.cs`
   - ✅ `Services/ApiService.cs`
   - ✅ `Filters/AuthorizeRoleAttribute.cs`

### Bước 5: Nếu vẫn lỗi - Thêm file thủ công
1. Right-click `MVC_ADMIN` project → **Add** → **Existing Item**
2. Thêm các file:
   - `Controllers/BaseController.cs`
   - `Helpers/ConfigurationHelper.cs`
   - `Services/ApiService.cs`
   - `Filters/AuthorizeRoleAttribute.cs`
3. Đảm bảo **Build Action** = **Compile**

### Bước 6: Kiểm tra References
1. Right-click `MVC_ADMIN` project → **Properties**
2. Tab **References** → Kiểm tra có:
   - ✅ `System.Web`
   - ✅ `System.Web.Mvc`
   - ✅ `System.Configuration`
   - ✅ `System.Net.Http`
   - ✅ `Newtonsoft.Json`

### Bước 7: Restart Visual Studio
Nếu vẫn lỗi, đóng và mở lại Visual Studio.

## 📝 Lưu ý:
- Đảm bảo tất cả file `.cs` đều có namespace đúng: `MVC_ADMIN.Controllers`, `MVC_ADMIN.Helpers`, `MVC_ADMIN.Services`, `MVC_ADMIN.Filters`
- Kiểm tra file `.csproj` đã có các dòng:
  ```xml
  <Compile Include="Controllers\BaseController.cs" />
  <Compile Include="Helpers\ConfigurationHelper.cs" />
  <Compile Include="Services\ApiService.cs" />
  <Compile Include="Filters\AuthorizeRoleAttribute.cs" />
  ```

