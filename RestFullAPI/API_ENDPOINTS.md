# 📚 REST API Endpoints - Admin Controllers

## 🔗 Base URL
```
https://localhost:7264/api
http://localhost:5012/api
```

## 📋 Danh sách Controllers

### 1. Users Controller (`/api/users`)

#### GET `/api/users`
Lấy danh sách tất cả người dùng
- **Response**: `200 OK` - Array of UserDto

#### GET `/api/users/{id}`
Lấy thông tin người dùng theo ID
- **Parameters**: `id` (int)
- **Response**: `200 OK` - UserDto hoặc `404 Not Found`

#### POST `/api/users`
Tạo người dùng mới
- **Body**: CreateUserDto
```json
{
  "fullName": "Nguyễn Văn A",
  "email": "user@example.com",
  "password": "123456",
  "phone": "0123456789",
  "address": "Đà Nẵng",
  "role": "Student"
}
```
- **Response**: `201 Created` - UserDto

#### PUT `/api/users/{id}`
Cập nhật thông tin người dùng
- **Parameters**: `id` (int)
- **Body**: UpdateUserDto
```json
{
  "fullName": "Nguyễn Văn B",
  "email": "newemail@example.com",
  "phone": "0987654321",
  "isActive": true
}
```
- **Response**: `204 No Content` hoặc `404 Not Found`

#### DELETE `/api/users/{id}`
Xóa người dùng
- **Parameters**: `id` (int)
- **Response**: `204 No Content` hoặc `404 Not Found`

---

### 2. Categories Controller (`/api/categories`)

#### GET `/api/categories`
Lấy danh sách tất cả danh mục
- **Response**: `200 OK` - Array of CategoryDto

#### GET `/api/categories/{id}`
Lấy thông tin danh mục theo ID
- **Parameters**: `id` (int)
- **Response**: `200 OK` - CategoryDto hoặc `404 Not Found`

#### POST `/api/categories`
Tạo danh mục mới
- **Body**: CreateCategoryDto
```json
{
  "categoryName": "Lập trình Web",
  "slug": "lap-trinh-web",
  "description": "Danh mục về lập trình web",
  "displayOrder": 1,
  "isActive": true
}
```
- **Response**: `201 Created` - CategoryDto

#### PUT `/api/categories/{id}`
Cập nhật danh mục
- **Parameters**: `id` (int)
- **Body**: UpdateCategoryDto
- **Response**: `204 No Content` hoặc `404 Not Found`

#### DELETE `/api/categories/{id}`
Xóa danh mục
- **Parameters**: `id` (int)
- **Response**: `204 No Content` hoặc `404 Not Found`

---

### 3. Permissions Controller (`/api/permissions`)

#### GET `/api/permissions`
Lấy danh sách tất cả quyền
- **Response**: `200 OK` - Array of PermissionDto

#### GET `/api/permissions/{id}`
Lấy thông tin quyền theo ID
- **Parameters**: `id` (int)
- **Response**: `200 OK` - PermissionDto hoặc `404 Not Found`

#### POST `/api/permissions`
Tạo quyền mới
- **Body**: CreatePermissionDto
```json
{
  "permissionName": "ViewUsers",
  "displayName": "Xem danh sách người dùng",
  "description": "Quyền xem danh sách người dùng",
  "module": "User",
  "isActive": true
}
```
- **Response**: `201 Created` - PermissionDto

#### PUT `/api/permissions/{id}`
Cập nhật quyền
- **Parameters**: `id` (int)
- **Body**: UpdatePermissionDto
- **Response**: `204 No Content` hoặc `404 Not Found`

#### DELETE `/api/permissions/{id}`
Xóa quyền
- **Parameters**: `id` (int)
- **Response**: `204 No Content` hoặc `404 Not Found`

---

### 4. Statistics Controller (`/api/statistics`)

#### GET `/api/statistics/dashboard`
Lấy thống kê dashboard
- **Response**: `200 OK` - Dashboard statistics
```json
{
  "totalUsers": 100,
  "activeUsers": 95,
  "inactiveUsers": 5,
  "totalCategories": 20,
  "activeCategories": 18,
  "totalPermissions": 15,
  "newUsersThisMonth": 10,
  "usersByRole": [
    { "role": "Admin", "count": 5 },
    { "role": "Teacher", "count": 20 },
    { "role": "Student", "count": 75 }
  ],
  "lastUpdated": "2024-01-01T00:00:00"
}
```

#### GET `/api/statistics/users`
Lấy thống kê người dùng
- **Response**: `200 OK` - User statistics

---

## 🔧 Cấu hình Database

### 1. Tạo bảng Permission (nếu chưa có)

Chạy script SQL:
```sql
-- Xem file: SQL_CREATE_PERMISSION_TABLE.sql
```

Hoặc chạy trực tiếp:
```sql
USE NenTangHocLieu;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Permission')
BEGIN
    CREATE TABLE Permission (
        PermissionId INT IDENTITY(1,1) PRIMARY KEY,
        PermissionName NVARCHAR(100) NOT NULL UNIQUE,
        DisplayName NVARCHAR(200),
        Description NVARCHAR(500),
        Module NVARCHAR(50),
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        UpdatedDate DATETIME
    );
END
GO
```

### 2. Connection String

Đã được cấu hình trong `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=CHAODAIKA\\THAITHANHTU2340;Database=NenTangHocLieu;User Id=sa;Password=12345;TrustServerCertificate=true;"
  }
}
```

---

## 🧪 Test API với Swagger

1. **Chạy API:**
   ```bash
   cd LTWNC/RestFullAPI
   dotnet run
   ```

2. **Mở Swagger UI:**
   - URL: `https://localhost:7264/swagger`
   - Hoặc: `http://localhost:5012/swagger`

3. **Test các endpoints:**
   - Click vào endpoint
   - Click "Try it out"
   - Điền thông tin
   - Click "Execute"

---

## 📝 DTOs (Data Transfer Objects)

### UserDto
```csharp
{
  "userId": 1,
  "fullName": "Nguyễn Văn A",
  "email": "user@example.com",
  "phone": "0123456789",
  "address": "Đà Nẵng",
  "dateOfBirth": "1990-01-01",
  "gender": "Nam",
  "role": "Student",
  "roleId": 3,
  "createdDate": "2024-01-01T00:00:00",
  "isActive": true
}
```

### CategoryDto
```csharp
{
  "categoryId": 1,
  "categoryName": "Lập trình Web",
  "slug": "lap-trinh-web",
  "description": "Danh mục về lập trình web",
  "parentCategoryId": null,
  "displayOrder": 1,
  "isActive": true
}
```

### PermissionDto
```csharp
{
  "permissionId": 1,
  "permissionName": "ViewUsers",
  "displayName": "Xem danh sách người dùng",
  "description": "Quyền xem danh sách người dùng",
  "module": "User",
  "isActive": true,
  "createdDate": "2024-01-01T00:00:00",
  "updatedDate": null
}
```

---

## ⚠️ Lưu ý

1. **Password Hashing**: Hiện tại dùng SHA256 đơn giản, nên dùng BCrypt trong production
2. **Error Handling**: Tất cả endpoints đều có error handling
3. **CORS**: Đã cấu hình CORS cho MVC_ADMIN
4. **Validation**: Cần thêm validation attributes cho DTOs

---

## 🚀 Sử dụng từ MVC_ADMIN

Các Controllers trong MVC_ADMIN đã được cấu hình để gọi các API endpoints này:
- `UserController` → `/api/users`
- `CategoryController` → `/api/categories`
- `PermissionController` → `/api/permissions`
- `StatisticalController` → `/api/statistics/dashboard`

