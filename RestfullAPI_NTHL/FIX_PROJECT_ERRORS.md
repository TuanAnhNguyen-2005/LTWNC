# 🔧 Sửa lỗi Project RestfullAPI_NTHL

## ❌ Lỗi gặp phải

1. **Source file không tìm thấy:**
   - `obj\Release\net8.0\RestfullAPI_NTHL.GlobalUsings.g.cs`
   - `obj\Debug\netcoreapp3.1\RestfullAPI_NTHL.AssemblyInfo.cs`
   - `obj\Debug\net9.0\RestfullAPI_NTHL.MvcApplicationPartsAssemblyInfo.cs`
   - Và nhiều file khác trong obj/bin

2. **Duplicate Compile items:**
   - Các file trong obj/bin được include trong project file
   - .NET SDK tự động include các file này

3. **Duplicate TargetFrameworkAttribute:**
   - Lỗi do có file AssemblyInfo.cs với `.NETFramework,Version=v4.7.2` trong khi project dùng `net8.0`
   - ✅ Đã sửa bằng cách thêm `<GenerateAssemblyInfo>true</GenerateAssemblyInfo>` và `<GenerateTargetFrameworkAttribute>true</GenerateTargetFrameworkAttribute>`

## ✅ Giải pháp đã áp dụng

### 1. Làm sạch Project File
- ✅ Xóa tất cả `<Compile Include="obj\...">` 
- ✅ Xóa tất cả `<Content Include="bin\...">`
- ✅ Xóa tất cả `<None Include="bin\...">` và `<None Include="obj\...">`
- ✅ Chỉ giữ lại các PackageReference cần thiết
- ✅ Đảm bảo chỉ có `TargetFramework` là `net8.0`

### 2. Xóa obj và bin folders
- ✅ Xóa folder `obj` để loại bỏ các file build cũ
- ✅ Xóa folder `bin` để loại bỏ các file output cũ

## 🔄 Các bước tiếp theo

### 1. Clean và Rebuild Project

Trong Visual Studio:
1. **Build** → **Clean Solution**
2. **Build** → **Rebuild Solution**

Hoặc từ command line:
```powershell
cd LTWNC\RestfullAPI_NTHL
dotnet clean
dotnet build
```

### 2. Kiểm tra lỗi

Sau khi rebuild, kiểm tra:
- ✅ Không còn lỗi "Source file could not be found"
- ✅ Không còn lỗi "Duplicate Compile items"
- ✅ Project build thành công

## 📝 Lưu ý

1. **Không bao giờ include obj/bin vào project file**
   - Các file trong obj/bin được tự động generate
   - .NET SDK tự động quản lý chúng

2. **Chỉ có một TargetFramework**
   - Project này dùng `net8.0`
   - Không nên có net9.0, netcoreapp3.1 trong cùng project

3. **Nếu vẫn còn lỗi:**
   - Xóa `.vs` folder
   - Xóa `obj` và `bin` folders
   - Rebuild project

## ✅ Kết quả mong đợi

Sau khi sửa:
- ✅ Project file sạch sẽ, chỉ có PackageReference
- ✅ Build thành công không lỗi
- ✅ Có thể chạy và debug bình thường

