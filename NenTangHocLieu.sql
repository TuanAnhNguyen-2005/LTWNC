USE master;
GO
-- Xóa DB nếu tồn tại
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'NenTangHocLieu')
BEGIN
    ALTER DATABASE NenTangHocLieu SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE NenTangHocLieu;
END
GO

-- Tạo lại DB mới
CREATE DATABASE NenTangHocLieu;
GO
USE NenTangHocLieu;
GO

-- ==============================
-- BẢNG VAI TRÒ, MÔN HỌC, LỚP HỌC, CHỦ ĐỀ
-- ==============================
CREATE TABLE VaiTro (
    MaVaiTro INT IDENTITY(1,1) PRIMARY KEY,
    TenVaiTro NVARCHAR(50) NOT NULL
);
CREATE TABLE MonHoc (
    MaMonHoc INT IDENTITY(1,1) PRIMARY KEY,
    TenMonHoc NVARCHAR(100) NOT NULL
);
CREATE TABLE LopHoc (
    MaLopHoc INT IDENTITY(1,1) PRIMARY KEY,
    TenLopHoc NVARCHAR(50) NOT NULL
);
CREATE TABLE ChuDe (
    MaChuDe INT IDENTITY(1,1) PRIMARY KEY,
    TenChuDe NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255)
);
GO

-- ==============================
-- BẢNG NGƯỜI DÙNG
-- ==============================
CREATE TABLE NguoiDung (
    MaNguoiDung INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100),
    Email NVARCHAR(100) UNIQUE NOT NULL,
    MatKhau NVARCHAR(255) NOT NULL,
    AnhDaiDien NVARCHAR(255),
    SoDienThoai NVARCHAR(15),
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    DiaChi NVARCHAR(255),
    MaVaiTro INT,
    NgayTao DATETIME DEFAULT GETDATE(),
    LanDangNhapCuoi DATETIME,
    TrangThai BIT DEFAULT 1,
    CONSTRAINT FK_NguoiDung_VaiTro FOREIGN KEY (MaVaiTro) REFERENCES VaiTro(MaVaiTro)
);
GO
USE NenTangHocLieu;
GO


CREATE TABLE Category (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(150) NOT NULL,
    Slug NVARCHAR(200) NOT NULL UNIQUE,
    Description NVARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
INSERT INTO Category (CategoryName, Slug, Description, IsActive)
VALUES
(N'Lập trình Web',       'lap-trinh-web',       N'HTML, CSS, JavaScript, ASP.NET', 1),
(N'Lập trình Mobile',    'lap-trinh-mobile',    N'Android, iOS, Flutter', 1),
(N'Cơ sở dữ liệu',       'co-so-du-lieu',       N'SQL Server, MySQL, MongoDB', 1),
(N'Trí tuệ nhân tạo',    'tri-tue-nhan-tao',    N'Machine Learning, AI', 1),
(N'Thiết kế đồ họa',     'thiet-ke-do-hoa',     N'Photoshop, Illustrator', 0);
CREATE TABLE KhoaHoc (
    MaKhoaHoc INT IDENTITY(1,1) PRIMARY KEY,
    TenKhoaHoc NVARCHAR(200) NOT NULL,
    Slug NVARCHAR(200) NULL,
    MoTa NVARCHAR(1000) NULL,

    -- Teacher tạo khóa học
    MaGiaoVien INT NOT NULL,

    -- Workflow duyệt
    TrangThaiDuyet NVARCHAR(20) NOT NULL DEFAULT N'Draft',  -- Draft | ChoDuyet | DaDuyet | TuChoi
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayGuiDuyet DATETIME NULL,
    NgayDuyet DATETIME NULL,
    NguoiDuyetId INT NULL,
    LyDoTuChoi NVARCHAR(500) NULL,

    IsActive BIT NOT NULL DEFAULT 1
);

USE [NenTangHocLieu];
GO
SELECT MaKhoaHoc, TenKhoaHoc, AnhBia 
FROM dbo.KhoaHoc 
WHERE MaKhoaHoc IN (9,10,11,12)

-- Kiểm tra nếu cột chưa tồn tại thì mới thêm (tránh lỗi nếu chạy lại nhiều lần)
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.KhoaHoc') 
      AND name = 'AnhBia'
)
BEGIN
    ALTER TABLE dbo.KhoaHoc
    ADD AnhBia NVARCHAR(500) NULL;  -- Lưu đường dẫn ảnh, ví dụ: /images/courses/abc123.jpg
    
    PRINT 'Đã thêm cột AnhBia thành công!';
END
ELSE
BEGIN
    PRINT 'Cột AnhBia đã tồn tại rồi, không cần thêm lại.';
END
GO
-- FK: giáo viên (NguoiDung)
ALTER TABLE KhoaHoc
ADD CONSTRAINT FK_KhoaHoc_GiaoVien
FOREIGN KEY (MaGiaoVien) REFERENCES NguoiDung(MaNguoiDung);


-- FK: người duyệt (Admin - cũng là NguoiDung)
ALTER TABLE KhoaHoc
ADD CONSTRAINT FK_KhoaHoc_NguoiDuyet
FOREIGN KEY (NguoiDuyetId) REFERENCES NguoiDung(MaNguoiDung);
CREATE TABLE DangKyKhoaHoc (
    MaDangKy INT IDENTITY(1,1) PRIMARY KEY,
    MaKhoaHoc INT NOT NULL,
    MaHocSinh INT NOT NULL,
    NgayDangKy DATETIME NOT NULL DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'DangHoc' -- DangHoc | Huy
);

ALTER TABLE DangKyKhoaHoc
ADD CONSTRAINT FK_DangKy_KhoaHoc
FOREIGN KEY (MaKhoaHoc) REFERENCES KhoaHoc(MaKhoaHoc);

ALTER TABLE DangKyKhoaHoc
ADD CONSTRAINT FK_DangKy_HocSinh
FOREIGN KEY (MaHocSinh) REFERENCES NguoiDung(MaNguoiDung);

-- 1 học sinh không đăng ký trùng 1 khóa
CREATE UNIQUE INDEX UX_DangKy_Unique
ON DangKyKhoaHoc(MaKhoaHoc, MaHocSinh);
-- Ví dụ: Teacher tạo 2 khóa, 1 cái đang chờ duyệt, 1 cái draft



-- Admin duyệt khóa học 1 (ví dụ admin id = 1)
UPDATE KhoaHoc
SET TrangThaiDuyet = N'DaDuyet',
    NgayDuyet = GETDATE(),
    NguoiDuyetId = 1,
    LyDoTuChoi = NULL
WHERE MaKhoaHoc = 1;


SELECT TOP 50 MaNguoiDung, Email, MaVaiTro, TrangThai
FROM NguoiDung
ORDER BY MaNguoiDung;


UPDATE NguoiDung
SET TrangThai = '1'
WHERE Email = 'thanhtu98912@gmail.com';
USE NenTangHocLieu;
GO
UPDATE NguoiDung
SET NgayTao = '2025-12-08 14:39:30.133'
WHERE Email = 'thanhtu98912@gmail.com';
-- ==============================
-- BẢNG HỌC LIỆU
-- ==============================
CREATE TABLE HocLieu (
    MaHocLieu INT IDENTITY(1,1) PRIMARY KEY,
    TieuDe NVARCHAR(200) NOT NULL,
    MoTa NVARCHAR(500),
    DuongDanTep NVARCHAR(255),
    LoaiTep NVARCHAR(50),
    KichThuocTep BIGINT,
    DoKho NVARCHAR(20) CHECK (DoKho IN (N'Dễ', N'Trung bình', N'Khó')),
    NgayDang DATETIME DEFAULT GETDATE(),
    MaChuDe INT,
    MaNguoiDung INT,
    MaMonHoc INT,
    MaLopHoc INT,
    DaDuyet BIT DEFAULT 0,
    TrangThaiDuyet NVARCHAR(20) DEFAULT N'Chờ duyệt'
        CHECK (TrangThaiDuyet IN (N'Chờ duyệt', N'Đã duyệt', N'Từ chối')),
    NguoiDuyet INT,
    NgayDuyet DATETIME,
    LyDoTuChoi NVARCHAR(500),
    LuotXem INT DEFAULT 0,
    SoLuotTai INT DEFAULT 0,
    DiemTrungBinh FLOAT DEFAULT 0,
    CONSTRAINT FK_HocLieu_ChuDe FOREIGN KEY (MaChuDe) REFERENCES ChuDe(MaChuDe) ON DELETE SET NULL,
    CONSTRAINT FK_HocLieu_NguoiDung FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung) ON DELETE SET NULL,
    CONSTRAINT FK_HocLieu_MonHoc FOREIGN KEY (MaMonHoc) REFERENCES MonHoc(MaMonHoc) ON DELETE SET NULL,
    CONSTRAINT FK_HocLieu_LopHoc FOREIGN KEY (MaLopHoc) REFERENCES LopHoc(MaLopHoc) ON DELETE SET NULL
);
GO

-- ==============================
-- BẢNG BÌNH LUẬN
-- ==============================
CREATE TABLE BinhLuan (
    MaBinhLuan INT IDENTITY(1,1) PRIMARY KEY,
    MaHocLieu INT,
    MaNguoiDung INT,
    NoiDung NVARCHAR(500),
    DanhGia INT CHECK (DanhGia BETWEEN 1 AND 5),
    MaBinhLuanCha INT,
    NgayDang DATETIME DEFAULT GETDATE(),
    TrangThai BIT DEFAULT 1,
    CONSTRAINT FK_BinhLuan_HocLieu FOREIGN KEY (MaHocLieu) REFERENCES HocLieu(MaHocLieu) ON DELETE CASCADE,
    CONSTRAINT FK_BinhLuan_NguoiDung FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung) ON DELETE SET NULL,
    CONSTRAINT FK_BinhLuan_Cha FOREIGN KEY (MaBinhLuanCha) REFERENCES BinhLuan(MaBinhLuan)
);
GO
-- ==============================
-- DỮ LIỆU MẪU (CHẠY SAU KHI ĐÃ TẠO XONG TẤT CẢ BẢNG)
-- ==============================
SET IDENTITY_INSERT VaiTro ON;
INSERT INTO VaiTro (MaVaiTro, TenVaiTro) VALUES (1, N'Admin'), (2, N'Giảng viên'), (3, N'Sinh viên');
SET IDENTITY_INSERT VaiTro OFF;

SET IDENTITY_INSERT MonHoc ON;
INSERT INTO MonHoc (MaMonHoc, TenMonHoc) VALUES 
(1, N'Lập trình Java'), (2, N'Cơ sở dữ liệu'), (3, N'Mạng máy tính');
SET IDENTITY_INSERT MonHoc OFF;

SET IDENTITY_INSERT LopHoc ON;
INSERT INTO LopHoc (MaLopHoc, TenLopHoc) VALUES (1, N'CNTT2A'), (2, N'CNTT2B'), (3, N'CNTT2C');
SET IDENTITY_INSERT LopHoc OFF;

SET IDENTITY_INSERT ChuDe ON;
INSERT INTO ChuDe (MaChuDe, TenChuDe, MoTa) VALUES
(1, N'Lập trình hướng đối tượng', NULL),
(2, N'Cấu trúc dữ liệu', NULL),
(3, N'Cơ sở dữ liệu quan hệ', NULL),
(4, N'Quản lý file', NULL),
(5, N'Lập trình web', NULL);
SET IDENTITY_INSERT ChuDe OFF;

SET IDENTITY_INSERT NguoiDung ON;
INSERT INTO NguoiDung (MaNguoiDung, HoTen, Email, MatKhau, MaVaiTro, GioiTinh, DiaChi) VALUES
(1, N'Admin', N'admin@example.com', N'123456', 1, N'Nam', N'Đà Nẵng'),
(2, N'GV Trần B', N'gv@example.com', N'123456', 2, N'Nữ', N'Hà Nội'),
(3, N'GV Lê C', N'gv2@example.com', N'123456', 2, N'Nam', N'HCM'),
(4, N'SV Phạm D', N'sv@example.com', N'123456', 3, N'Nữ', N'Đà Nẵng'),
(5, N'SV Hoàng E', N'sv2@example.com', N'123456', 3, N'Nam', N'Huế');
SET IDENTITY_INSERT NguoiDung OFF;

-- Insert KhoaHoc SAU NguoiDung (để MaGiaoVien=2 tồn tại)
INSERT INTO KhoaHoc (TenKhoaHoc, Slug, MoTa, MaGiaoVien, TrangThaiDuyet, NgayGuiDuyet)
VALUES
(N'Lập trình C# cơ bản', N'lap-trinh-csharp-co-ban', N'Khóa học cho người mới bắt đầu', 2, N'ChoDuyet', GETDATE()),
(N'SQL Server nền tảng', N'sql-server-nen-tang', N'Học từ cơ bản đến thực hành', 2, N'Draft', NULL);

-- Duyệt khóa học MaKhoaHoc=1
UPDATE KhoaHoc
SET TrangThaiDuyet = N'DaDuyet',
    NgayDuyet = GETDATE(),
    NguoiDuyetId = 1,
    LyDoTuChoi = NULL
WHERE MaKhoaHoc = 1;

-- Insert đăng ký SAU KHI KhoaHoc đã tồn tại
INSERT INTO DangKyKhoaHoc (MaKhoaHoc, MaHocSinh)
VALUES (1, 3);

-- Insert HocLieu (sau các bảng cha)
SET IDENTITY_INSERT HocLieu ON;
INSERT INTO HocLieu (MaHocLieu, TieuDe, MoTa, DuongDanTep, LoaiTep, KichThuocTep, DoKho, MaChuDe, MaNguoiDung, MaMonHoc, MaLopHoc, DaDuyet, TrangThaiDuyet) VALUES
(1, N'Bài giảng OOP Cơ bản', N'Nội dung OOP', N'/files/oop.pdf', N'PDF', 1024000, N'Dễ', 1, 2, 1, 1, 1, N'Đã duyệt'),
(2, N'Video SQL Join', N'Các loại join', N'/files/join.mp4', N'Video', 52428800, N'Trung bình', 3, 2, 2, 1, 1, N'Đã duyệt'),
(3, N'Bài tập Collections', NULL, N'/files/collections.docx', N'Word', 51200, N'Khó', 2, 3, 1, 1, 0, N'Chờ duyệt'),
(4, N'Slide Mạng máy tính', NULL, N'/files/mang.pptx', N'PPT', 8192000, N'Trung bình', 2, 2, 3, 1, 1, N'Đã duyệt'),
(5, N'HTML cơ bản', NULL, N'/files/html.pdf', N'PDF', 204800, N'Dễ', 5, 3, 3, 1, 1, N'Đã duyệt');
SET IDENTITY_INSERT HocLieu OFF;

-- Insert BinhLuan
INSERT INTO BinhLuan (MaHocLieu, MaNguoiDung, NoiDung, DanhGia, MaBinhLuanCha) VALUES
(1, 4, N'Rất dễ hiểu ạ!', 5, NULL),
(1, 5, N'Giảng hay quá thầy ơi', 5, NULL),
(2, 4, N'Video chất lượng cao', 4, NULL),
(1, 2, N'Cảm ơn các em đã góp ý', NULL, 1); -- reply của giáo viên

PRINT N'=== NHẬP DỮ LIỆU MẪU THÀNH CÔNG 100% ===';

-- Cập nhật điểm trung bình
UPDATE HocLieu 
SET DiemTrungBinh = (
    SELECT AVG(CAST(DanhGia AS FLOAT)) 
    FROM BinhLuan 
    WHERE BinhLuan.MaHocLieu = HocLieu.MaHocLieu AND DanhGia IS NOT NULL
)
WHERE EXISTS (SELECT 1 FROM BinhLuan WHERE BinhLuan.MaHocLieu = HocLieu.MaHocLieu AND DanhGia IS NOT NULL);
GO
SELECT * FROM NguoiDung;
PRINT N'=== NHẬP DỮ LIỆU MẪU THÀNH CÔNG 100% ===';
-- Cập nhật điểm trung bình
UPDATE HocLieu 
SET DiemTrungBinh = (
    SELECT AVG(CAST(DanhGia AS FLOAT)) 
    FROM BinhLuan 
    WHERE BinhLuan.MaHocLieu = HocLieu.MaHocLieu AND DanhGia IS NOT NULL
)
WHERE EXISTS (SELECT 1 FROM BinhLuan WHERE BinhLuan.MaHocLieu = HocLieu.MaHocLieu AND DanhGia IS NOT NULL);
GO

PRINT N'=== TẠO DATABASE NenTangHocLieu THÀNH CÔNG 100% ===';
USE NenTangHocLieu;
GO

-- XÓA PROC NẾU ĐÃ TỒN TẠI
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'psDeleteHocLieu') DROP PROC psDeleteHocLieu;
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'psLoginNguoiDung') DROP PROC psLoginNguoiDung;
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'psInsertHocLieu') DROP PROC psInsertHocLieu;
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'psUpdateHocLieu') DROP PROC psUpdateHocLieu;
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'psGetMonHoc') DROP PROC psGetMonHoc;
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'psGetHocLieu') DROP PROC psGetHocLieu;
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'psUpdateNguoiDung') DROP PROC psUpdateNguoiDung;
GO

-- 1. XÓA HỌC LIỆU
CREATE PROC psDeleteHocLieu (@MaHocLieu INT)
AS
BEGIN
    BEGIN TRAN
        DELETE FROM HocLieu WHERE MaHocLieu = @MaHocLieu;
    IF (@@ERROR <> 0)
        ROLLBACK TRAN
    ELSE
        COMMIT TRAN
END
GO

-- 2. ĐĂNG NHẬP
CREATE PROC psLoginNguoiDung (@Email NVARCHAR(100), @MatKhau NVARCHAR(255))
AS
BEGIN
    BEGIN TRAN
        SELECT MaNguoiDung, HoTen, Email, MaVaiTro, TenVaiTro = (SELECT TenVaiTro FROM VaiTro WHERE VaiTro.MaVaiTro = NguoiDung.MaVaiTro)
        FROM NguoiDung 
        WHERE Email = @Email AND MatKhau = @MatKhau;
    IF (@@ERROR <> 0)
        ROLLBACK TRAN
    ELSE
        COMMIT TRAN
END
GO
USE [NenTangHocLieu];
GO
ALTER PROC psLoginNguoiDung
    @Email NVARCHAR(100),
    @MatKhau NVARCHAR(255)
AS
BEGIN
    SELECT MaNguoiDung, HoTen, Email, MaVaiTro,
           TenVaiTro = (SELECT TenVaiTro FROM VaiTro WHERE VaiTro.MaVaiTro = NguoiDung.MaVaiTro)
    FROM NguoiDung
    WHERE Email = @Email
      AND MatKhau = @MatKhau
      AND MaVaiTro = 1      -- ✅ chỉ Admin
      AND TrangThai = 1;    -- ✅ (tuỳ bạn) chỉ cho tài khoản đang active
END


-- 3. THÊM HỌC LIỆU MỚI
CREATE PROC psInsertHocLieu
(
    @TieuDe NVARCHAR(200),
    @MoTa NVARCHAR(500),
    @DuongDanTep NVARCHAR(255),
    @LoaiTep NVARCHAR(50),
    @KichThuocTep BIGINT,
    @DoKho NVARCHAR(20),
    @MaChuDe INT,
    @MaNguoiDung INT,
    @MaMonHoc INT,
    @MaLopHoc INT
)
AS
BEGIN
    BEGIN TRAN
        INSERT INTO HocLieu(TieuDe, MoTa, DuongDanTep, LoaiTep, KichThuocTep, DoKho, MaChuDe, MaNguoiDung, MaMonHoc, MaLopHoc)
        VALUES (@TieuDe, @MoTa, @DuongDanTep, @LoaiTep, @KichThuocTep, @DoKho, @MaChuDe, @MaNguoiDung, @MaMonHoc, @MaLopHoc);
    IF (@@ERROR <> 0)
        ROLLBACK TRAN
    ELSE
        COMMIT TRAN
END
GO

-- 4. CẬP NHẬT HỌC LIỆU
CREATE PROC psUpdateHocLieu
(
    @MaHocLieu INT,
    @TieuDe NVARCHAR(200),
    @MoTa NVARCHAR(500),
    @DuongDanTep NVARCHAR(255),
    @LoaiTep NVARCHAR(50),
    @KichThuocTep BIGINT,
    @DoKho NVARCHAR(20),
    @MaChuDe INT,
    @MaMonHoc INT,
    @MaLopHoc INT,
    @DaDuyet BIT = NULL
)
AS
BEGIN
    BEGIN TRAN
        UPDATE HocLieu SET
            TieuDe = @TieuDe,
            MoTa = @MoTa,
            DuongDanTep = @DuongDanTep,
            LoaiTep = @LoaiTep,
            KichThuocTep = @KichThuocTep,
            DoKho = @DoKho,
            MaChuDe = @MaChuDe,
            MaMonHoc = @MaMonHoc,
            MaLopHoc = @MaLopHoc,
            DaDuyet = ISNULL(@DaDuyet, DaDuyet),
            TrangThaiDuyet = CASE WHEN @DaDuyet = 1 THEN N'Đã duyệt' WHEN @DaDuyet = 0 THEN N'Chờ duyệt' ELSE TrangThaiDuyet END
        WHERE MaHocLieu = @MaHocLieu;
    IF (@@ERROR <> 0)
        ROLLBACK TRAN
    ELSE
        COMMIT TRAN
END
GO

-- 5. LẤY DANH SÁCH MÔN HỌC
CREATE PROC psGetMonHoc (@MaMonHoc INT = NULL)
AS
BEGIN
    IF (@MaMonHoc IS NULL)
        SELECT MaMonHoc, TenMonHoc FROM MonHoc ORDER BY TenMonHoc;
    ELSE
        SELECT MaMonHoc, TenMonHoc FROM MonHoc WHERE MaMonHoc = @MaMonHoc;
END
GO

-- 6. LẤY DANH SÁCH HỌC LIỆU
CREATE PROC psGetHocLieu (@MaHocLieu INT = NULL)
AS
BEGIN
    IF (@MaHocLieu IS NULL)
        SELECT * FROM HocLieu ORDER BY NgayDang DESC;
    ELSE
        SELECT * FROM HocLieu WHERE MaHocLieu = @MaHocLieu;
END
GO

-- 7. CẬP NHẬT THÔNG TIN NGƯỜI DÙNG
CREATE PROC psUpdateNguoiDung
(
    @MaNguoiDung INT,
    @HoTen NVARCHAR(100) = NULL,
    @Email NVARCHAR(100) = NULL,
    @MatKhau NVARCHAR(255) = NULL,
    @AnhDaiDien NVARCHAR(255) = NULL,
    @SoDienThoai NVARCHAR(15) = NULL,
    @NgaySinh DATE = NULL,
    @GioiTinh NVARCHAR(10) = NULL,
    @DiaChi NVARCHAR(255) = NULL
)
AS
BEGIN
    BEGIN TRAN
        UPDATE NguoiDung SET
            HoTen = ISNULL(@HoTen, HoTen),
            Email = ISNULL(@Email, Email),
            MatKhau = ISNULL(@MatKhau, MatKhau),
            AnhDaiDien = ISNULL(@AnhDaiDien, AnhDaiDien),
            SoDienThoai = ISNULL(@SoDienThoai, SoDienThoai),
            NgaySinh = ISNULL(@NgaySinh, NgaySinh),
            GioiTinh = ISNULL(@GioiTinh, GioiTinh),
            DiaChi = ISNULL(@DiaChi, DiaChi)
        WHERE MaNguoiDung = @MaNguoiDung;
    IF (@@ERROR <> 0)
        ROLLBACK TRAN
    ELSE
        COMMIT TRAN
END
GO

PRINT N'=== TẤT CẢ STORED PROCEDURE ĐÃ ĐƯỢC TẠO THÀNH CÔNG! ===';
