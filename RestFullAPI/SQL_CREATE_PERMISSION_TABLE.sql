-- Script tạo bảng Permission nếu chưa có
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
    
    PRINT 'Bảng Permission đã được tạo thành công!';
END
ELSE
BEGIN
    PRINT 'Bảng Permission đã tồn tại.';
END
GO

