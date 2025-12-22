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

-- ==============================
-- BẢNG CATEGORY
-- ==============================
CREATE TABLE Category (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(150) NOT NULL,
    Slug NVARCHAR(200) NOT NULL UNIQUE,
    Description NVARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- ==============================
-- BẢNG KHÓA HỌC
-- ==============================
CREATE TABLE KhoaHoc (
    MaKhoaHoc INT IDENTITY(1,1) PRIMARY KEY,
    TenKhoaHoc NVARCHAR(200) NOT NULL,
    Slug NVARCHAR(200) NULL,
    MoTa NVARCHAR(1000) NULL,
    AnhBia NVARCHAR(500) NULL,
    MaGiaoVien INT NOT NULL,
    TrangThaiDuyet NVARCHAR(20) NOT NULL DEFAULT N'Draft',
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayGuiDuyet DATETIME NULL,
    NgayDuyet DATETIME NULL,
    NguoiDuyetId INT NULL,
    LyDoTuChoi NVARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_KhoaHoc_GiaoVien FOREIGN KEY (MaGiaoVien) REFERENCES NguoiDung(MaNguoiDung),
    CONSTRAINT FK_KhoaHoc_NguoiDuyet FOREIGN KEY (NguoiDuyetId) REFERENCES NguoiDung(MaNguoiDung)
);
GO

-- ==============================
-- BẢNG ĐĂNG KÝ KHÓA HỌC
-- ==============================
CREATE TABLE DangKyKhoaHoc (
    MaDangKy INT IDENTITY(1,1) PRIMARY KEY,
    MaKhoaHoc INT NOT NULL,
    MaHocSinh INT NOT NULL,
    NgayDangKy DATETIME NOT NULL DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'DangHoc',
    CONSTRAINT FK_DangKy_KhoaHoc FOREIGN KEY (MaKhoaHoc) REFERENCES KhoaHoc(MaKhoaHoc),
    CONSTRAINT FK_DangKy_HocSinh FOREIGN KEY (MaHocSinh) REFERENCES NguoiDung(MaNguoiDung)
);
GO

CREATE UNIQUE INDEX UX_DangKy_Unique ON DangKyKhoaHoc(MaKhoaHoc, MaHocSinh);
GO

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
    TrangThaiDuyet NVARCHAR(20) DEFAULT N'Chờ duyệt' CHECK (TrangThaiDuyet IN (N'Chờ duyệt', N'Đã duyệt', N'Từ chối')),
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
-- BẢNG QUIZ
-- ==============================
CREATE TABLE Quiz (
    MaQuiz INT IDENTITY(1,1) PRIMARY KEY,
    TenQuiz NVARCHAR(200) NOT NULL,
    MoTa NVARCHAR(500) NULL,
    MaKhoaHoc INT NULL,
    MaGiaoVien INT NOT NULL,
    ThoiGianLamBai INT NULL,
    NgayTao DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) DEFAULT N'Draft',
    CONSTRAINT FK_Quiz_GiaoVien FOREIGN KEY (MaGiaoVien) REFERENCES NguoiDung(MaNguoiDung),
    CONSTRAINT FK_Quiz_KhoaHoc FOREIGN KEY (MaKhoaHoc) REFERENCES KhoaHoc(MaKhoaHoc)
);
GO

CREATE TABLE CauHoi (
    MaCauHoi INT IDENTITY(1,1) PRIMARY KEY,
    MaQuiz INT NOT NULL,
    NoiDung NVARCHAR(MAX) NOT NULL,
    LoaiCauHoi NVARCHAR(50) NOT NULL,
    Diem FLOAT DEFAULT 1,
    CONSTRAINT FK_CauHoi_Quiz FOREIGN KEY (MaQuiz) REFERENCES Quiz(MaQuiz) ON DELETE CASCADE
);
GO

CREATE TABLE LuaChon (
    MaLuaChon INT IDENTITY(1,1) PRIMARY KEY,
    MaCauHoi INT NOT NULL,
    NoiDung NVARCHAR(500) NOT NULL,
    LaDapAnDung BIT DEFAULT 0,
    CONSTRAINT FK_LuaChon_CauHoi FOREIGN KEY (MaCauHoi) REFERENCES CauHoi(MaCauHoi) ON DELETE CASCADE
);
GO

CREATE TABLE KetQuaQuiz (
    MaKetQua INT IDENTITY(1,1) PRIMARY KEY,
    MaQuiz INT NOT NULL,
    MaHocSinh INT NOT NULL,
    Diem FLOAT NULL,
    ThoiGianBatDau DATETIME NULL,
    ThoiGianKetThuc DATETIME NULL,
    CONSTRAINT FK_KetQuaQuiz_Quiz FOREIGN KEY (MaQuiz) REFERENCES Quiz(MaQuiz),
    CONSTRAINT FK_KetQuaQuiz_HocSinh FOREIGN KEY (MaHocSinh) REFERENCES NguoiDung(MaNguoiDung)
);
GO

CREATE TABLE TraLoi (
    MaTraLoi INT IDENTITY(1,1) PRIMARY KEY,
    MaKetQua INT NOT NULL,
    MaCauHoi INT NOT NULL,
    TraLoi NVARCHAR(MAX) NULL,
    CONSTRAINT FK_TraLoi_KetQua FOREIGN KEY (MaKetQua) REFERENCES KetQuaQuiz(MaKetQua) ON DELETE CASCADE,
    CONSTRAINT FK_TraLoi_CauHoi FOREIGN KEY (MaCauHoi) REFERENCES CauHoi(MaCauHoi)
);
GO

-- ==============================
-- NHẬP DỮ LIỆU MẪU
-- ==============================

-- VaiTro
SET IDENTITY_INSERT VaiTro ON;
INSERT INTO VaiTro (MaVaiTro, TenVaiTro) 
VALUES (1, N'Admin'), (2, N'Giảng viên'), (3, N'Sinh viên');
SET IDENTITY_INSERT VaiTro OFF;

-- MonHoc
SET IDENTITY_INSERT MonHoc ON;
INSERT INTO MonHoc (MaMonHoc, TenMonHoc) 
VALUES (1, N'Lập trình Java'), (2, N'Cơ sở dữ liệu'), (3, N'Mạng máy tính');
SET IDENTITY_INSERT MonHoc OFF;

-- LopHoc
SET IDENTITY_INSERT LopHoc ON;
INSERT INTO LopHoc (MaLopHoc, TenLopHoc) 
VALUES (1, N'CNTT2A'), (2, N'CNTT2B'), (3, N'CNTT2C');
SET IDENTITY_INSERT LopHoc OFF;

-- ChuDe
SET IDENTITY_INSERT ChuDe ON;
INSERT INTO ChuDe (MaChuDe, TenChuDe, MoTa) 
VALUES
(1, N'Lập trình hướng đối tượng', NULL),
(2, N'Cấu trúc dữ liệu', NULL),
(3, N'Cơ sở dữ liệu quan hệ', NULL),
(4, N'Quản lý file', NULL),
(5, N'Lập trình web', NULL);
SET IDENTITY_INSERT ChuDe OFF;

-- NguoiDung
SET IDENTITY_INSERT NguoiDung ON;
INSERT INTO NguoiDung (MaNguoiDung, HoTen, Email, MatKhau, MaVaiTro, GioiTinh, DiaChi) 
VALUES
(1, N'Admin', N'admin@example.com', N'123456', 1, N'Nam', N'Đà Nẵng'),
(2, N'GV Trần B', N'gv@example.com', N'123456', 2, N'Nữ', N'Hà Nội'),
(3, N'GV Lê C', N'gv2@example.com', N'123456', 2, N'Nam', N'HCM'),
(4, N'SV Phạm D', N'sv@example.com', N'123456', 3, N'Nữ', N'Đà Nẵng'),
(5, N'SV Hoàng E', N'sv2@example.com', N'123456', 3, N'Nam', N'Huế');
SET IDENTITY_INSERT NguoiDung OFF;

-- Category
INSERT INTO Category (CategoryName, Slug, Description, IsActive)
VALUES
(N'Lập trình Web', 'lap-trinh-web', N'HTML, CSS, JavaScript, ASP.NET', 1),
(N'Lập trình Mobile', 'lap-trinh-mobile', N'Android, iOS, Flutter', 1),
(N'Cơ sở dữ liệu', 'co-so-du-lieu', N'SQL Server, MySQL, MongoDB', 1),
(N'Trí tuệ nhân tạo', 'tri-tue-nhan-tao', N'Machine Learning, AI', 1),
(N'Thiết kế đồ họa', 'thiet-ke-do-hoa', N'Photoshop, Illustrator', 0);

-- KhoaHoc
INSERT INTO KhoaHoc (TenKhoaHoc, Slug, MoTa, MaGiaoVien, TrangThaiDuyet, NgayGuiDuyet)
VALUES
(N'Lập trình C# cơ bản', N'lap-trinh-csharp-co-ban', N'Khóa học cho người mới bắt đầu', 2, N'ChoDuyet', GETDATE()),
(N'SQL Server nền tảng', N'sql-server-nen-tang', N'Học từ cơ bản đến thực hành', 2, N'Draft', NULL);

-- Duyệt khóa học
UPDATE KhoaHoc
SET TrangThaiDuyet = N'DaDuyet',
    NgayDuyet = GETDATE(),
    NguoiDuyetId = 1,
    LyDoTuChoi = NULL
WHERE MaKhoaHoc = 1;

-- DangKyKhoaHoc
INSERT INTO DangKyKhoaHoc (MaKhoaHoc, MaHocSinh)
VALUES (1, 4);

-- HocLieu
SET IDENTITY_INSERT HocLieu ON;
INSERT INTO HocLieu (MaHocLieu, TieuDe, MoTa, DuongDanTep, LoaiTep, KichThuocTep, DoKho, MaChuDe, MaNguoiDung, MaMonHoc, MaLopHoc, DaDuyet, TrangThaiDuyet) 
VALUES
(1, N'Bài giảng OOP Cơ bản', N'Nội dung OOP', N'/files/oop.pdf', N'PDF', 1024000, N'Dễ', 1, 2, 1, 1, 1, N'Đã duyệt'),
(2, N'Video SQL Join', N'Các loại join', N'/files/join.mp4', N'Video', 52428800, N'Trung bình', 3, 2, 2, 1, 1, N'Đã duyệt'),
(3, N'Bài tập Collections', NULL, N'/files/collections.docx', N'Word', 51200, N'Khó', 2, 3, 1, 1, 0, N'Chờ duyệt'),
(4, N'Slide Mạng máy tính', NULL, N'/files/mang.pptx', N'PPT', 8192000, N'Trung bình', 2, 2, 3, 1, 1, N'Đã duyệt'),
(5, N'HTML cơ bản', NULL, N'/files/html.pdf', N'PDF', 204800, N'Dễ', 5, 3, 3, 1, 1, N'Đã duyệt');
SET IDENTITY_INSERT HocLieu OFF;

-- BinhLuan
INSERT INTO BinhLuan (MaHocLieu, MaNguoiDung, NoiDung, DanhGia, MaBinhLuanCha) 
VALUES
(1, 4, N'Rất dễ hiểu ạ!', 5, NULL),
(1, 5, N'Giảng hay quá thầy ơi', 5, NULL),
(2, 4, N'Video chất lượng cao', 4, NULL),
(1, 2, N'Cảm ơn các em đã góp ý', NULL, 1);

-- Cập nhật điểm trung bình
UPDATE HocLieu 
SET DiemTrungBinh = (
    SELECT AVG(CAST(DanhGia AS FLOAT)) 
    FROM BinhLuan 
    WHERE BinhLuan.MaHocLieu = HocLieu.MaHocLieu AND DanhGia IS NOT NULL
)
WHERE EXISTS (
    SELECT 1 FROM BinhLuan 
    WHERE BinhLuan.MaHocLieu = HocLieu.MaHocLieu AND DanhGia IS NOT NULL
);
GO

PRINT N'=== NHẬP DỮ LIỆU MẪU THÀNH CÔNG ===';
GO

-- ==============================
-- STORED PROCEDURES
-- ==============================

-- 1. Xóa học liệu
CREATE PROC psDeleteHocLieu 
    @MaHocLieu INT
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

-- 2. Đăng nhập (chỉ Admin)
CREATE PROC psLoginNguoiDung
    @Email NVARCHAR(100),
    @MatKhau NVARCHAR(255)
AS
BEGIN
    SELECT MaNguoiDung, HoTen, Email, MaVaiTro,
           TenVaiTro = (SELECT TenVaiTro FROM VaiTro WHERE VaiTro.MaVaiTro = NguoiDung.MaVaiTro)
    FROM NguoiDung
    WHERE Email = @Email
      AND MatKhau = @MatKhau
      AND MaVaiTro = 1
      AND TrangThai = 1;
END
GO

-- 3. Thêm học liệu mới
CREATE PROC psInsertHocLieu
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

-- 4. Cập nhật học liệu
CREATE PROC psUpdateHocLieu
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
            TrangThaiDuyet = CASE 
                WHEN @DaDuyet = 1 THEN N'Đã duyệt' 
                WHEN @DaDuyet = 0 THEN N'Chờ duyệt' 
                ELSE TrangThaiDuyet 
            END
        WHERE MaHocLieu = @MaHocLieu;
    IF (@@ERROR <> 0)
        ROLLBACK TRAN
    ELSE
        COMMIT TRAN
END
GO

-- 5. Lấy danh sách môn học
CREATE PROC psGetMonHoc 
    @MaMonHoc INT = NULL
AS
BEGIN
    IF (@MaMonHoc IS NULL)
        SELECT MaMonHoc, TenMonHoc FROM MonHoc ORDER BY TenMonHoc;
    ELSE
        SELECT MaMonHoc, TenMonHoc FROM MonHoc WHERE MaMonHoc = @MaMonHoc;
END
GO

-- 6. Lấy danh sách học liệu
CREATE PROC psGetHocLieu 
    @MaHocLieu INT = NULL
AS
BEGIN
    IF (@MaHocLieu IS NULL)
        SELECT * FROM HocLieu ORDER BY NgayDang DESC;
    ELSE
        SELECT * FROM HocLieu WHERE MaHocLieu = @MaHocLieu;
END
GO

-- 7. Cập nhật thông tin người dùng
CREATE PROC psUpdateNguoiDung
    @MaNguoiDung INT,
    @HoTen NVARCHAR(100) = NULL,
    @Email NVARCHAR(100) = NULL,
    @MatKhau NVARCHAR(255) = NULL,
    @AnhDaiDien NVARCHAR(255) = NULL,
    @SoDienThoai NVARCHAR(15) = NULL,
    @NgaySinh DATE = NULL,
    @GioiTinh NVARCHAR(10) = NULL,
    @DiaChi NVARCHAR(255) = NULL
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

-- ==============================
-- STORED PROCEDURES CHO QUIZ
-- ==============================

CREATE PROC psInsertQuiz
    @TenQuiz NVARCHAR(200),
    @MoTa NVARCHAR(500),
    @MaKhoaHoc INT = NULL,
    @MaGiaoVien INT,
    @ThoiGianLamBai INT = NULL,
    @TrangThai NVARCHAR(20) = N'Draft'
AS
BEGIN
    INSERT INTO Quiz (TenQuiz, MoTa, MaKhoaHoc, MaGiaoVien, ThoiGianLamBai, TrangThai)
    VALUES (@TenQuiz, @MoTa, @MaKhoaHoc, @MaGiaoVien, @ThoiGianLamBai, @TrangThai);
    SELECT SCOPE_IDENTITY() AS MaQuiz;
END
GO

CREATE PROC psInsertCauHoi
    @MaQuiz INT,
    @NoiDung NVARCHAR(MAX),
    @LoaiCauHoi NVARCHAR(50),
    @Diem FLOAT = 1
AS
BEGIN
    INSERT INTO CauHoi (MaQuiz, NoiDung, LoaiCauHoi, Diem)
    VALUES (@MaQuiz, @NoiDung, @LoaiCauHoi, @Diem);
    SELECT SCOPE_IDENTITY() AS MaCauHoi;
END
GO

CREATE PROC psInsertLuaChon
    @MaCauHoi INT,
    @NoiDung NVARCHAR(500),
    @LaDapAnDung BIT = 0
AS
BEGIN
    INSERT INTO LuaChon (MaCauHoi, NoiDung, LaDapAnDung)
    VALUES (@MaCauHoi, @NoiDung, @LaDapAnDung);
END
GO

CREATE PROC psGetQuizByGiaoVien
    @MaGiaoVien INT
AS
BEGIN
    SELECT * FROM Quiz WHERE MaGiaoVien = @MaGiaoVien;
END
GO

CREATE PROC psGetQuizForStudent
    @MaHocSinh INT,
    @MaKhoaHoc INT
AS
BEGIN
    SELECT q.* FROM Quiz q
    INNER JOIN DangKyKhoaHoc dk ON q.MaKhoaHoc = dk.MaKhoaHoc
    WHERE dk.MaHocSinh = @MaHocSinh 
      AND q.MaKhoaHoc = @MaKhoaHoc 
      AND q.TrangThai = N'Published';
END
GO

CREATE PROC psSubmitKetQua
    @MaQuiz INT,
    @MaHocSinh INT,
    @Diem FLOAT,
    @ThoiGianBatDau DATETIME,
    @ThoiGianKetThuc DATETIME
AS
BEGIN
    INSERT INTO KetQuaQuiz (MaQuiz, MaHocSinh, Diem, ThoiGianBatDau, ThoiGianKetThuc)
    VALUES (@MaQuiz, @MaHocSinh, @Diem, @ThoiGianBatDau, @ThoiGianKetThuc);
    SELECT SCOPE_IDENTITY() AS MaKetQua;
END
GO

CREATE PROC psInsertTraLoi
    @MaKetQua INT,
    @MaCauHoi INT,
    @TraLoi NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO TraLoi (MaKetQua, MaCauHoi, TraLoi)
    VALUES (@MaKetQua, @MaCauHoi, @TraLoi);
END
GO
USE NenTangHocLieu;
GO
ALTER TABLE HocLieu ADD MaKhoaHoc INT NULL;
ALTER TABLE HocLieu ADD CONSTRAINT FK_HocLieu_KhoaHoc
    FOREIGN KEY (MaKhoaHoc) REFERENCES KhoaHoc(MaKhoaHoc) ON DELETE SET NULL;
CREATE INDEX IX_HocLieu_MaKhoaHoc ON HocLieu(MaKhoaHoc);
USE NenTangHocLieu;
GO

-- Thêm cột MaKhoaHoc nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'HocLieu' AND COLUMN_NAME = 'MaKhoaHoc')
BEGIN
    ALTER TABLE HocLieu ADD MaKhoaHoc INT NULL;
    
    -- Tạo foreign key (nếu xóa khóa học thì set null cho tài liệu)
    ALTER TABLE HocLieu ADD CONSTRAINT FK_HocLieu_KhoaHoc 
        FOREIGN KEY (MaKhoaHoc) REFERENCES KhoaHoc(MaKhoaHoc) ON DELETE SET NULL;
    
    -- Tạo index để tìm nhanh
    CREATE INDEX IX_HocLieu_MaKhoaHoc ON HocLieu(MaKhoaHoc);
    
    PRINT 'Đã thêm cột MaKhoaHoc thành công';
END
ELSE
BEGIN
    PRINT 'Cột MaKhoaHoc đã tồn tại';
END
GO
USE NenTangHocLieu;
GO

-- === THÊM CÁC CỘT CÒN THIẾU VÀO BẢNG KetQuaQuiz (NẾU CHƯA CÓ) ===

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'KetQuaQuiz' AND COLUMN_NAME = 'TongDiem')
BEGIN
    ALTER TABLE KetQuaQuiz ADD TongDiem FLOAT NULL;
    PRINT 'Đã thêm cột TongDiem';
END
ELSE
    PRINT 'Cột TongDiem đã tồn tại';

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'KetQuaQuiz' AND COLUMN_NAME = 'SoCauDung')
BEGIN
    ALTER TABLE KetQuaQuiz ADD SoCauDung INT NULL DEFAULT 0;
    PRINT 'Đã thêm cột SoCauDung';
END
ELSE
    PRINT 'Cột SoCauDung đã tồn tại';

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'KetQuaQuiz' AND COLUMN_NAME = 'TongCau')
BEGIN
    ALTER TABLE KetQuaQuiz ADD TongCau INT NULL;
    PRINT 'Đã thêm cột TongCau';
END
ELSE
    PRINT 'Cột TongCau đã tồn tại';

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'KetQuaQuiz' AND COLUMN_NAME = 'NgayNop')
BEGIN
    ALTER TABLE KetQuaQuiz ADD NgayNop DATETIME NULL DEFAULT GETDATE();
    PRINT 'Đã thêm cột NgayNop';
END
ELSE
    PRINT 'Cột NgayNop đã tồn tại';

-- Thêm cột ThoiGianLam tính bằng giây (computed column - không cần lưu thủ công)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'KetQuaQuiz' AND COLUMN_NAME = 'ThoiGianLamGiay')
BEGIN
    ALTER TABLE KetQuaQuiz ADD ThoiGianLamGiay AS 
        CASE 
            WHEN ThoiGianBatDau IS NOT NULL AND ThoiGianKetThuc IS NOT NULL 
            THEN DATEDIFF(SECOND, ThoiGianBatDau, ThoiGianKetThuc)
            ELSE NULL 
        END PERSISTED;  -- PERSISTED để có thể tạo index nếu cần
    PRINT 'Đã thêm cột computed ThoiGianLamGiay (giây)';
END
ELSE
    PRINT 'Cột ThoiGianLamGiay đã tồn tại';

-- === TẠO INDEX ĐỂ TÌM NHANH ===
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_KetQuaQuiz_MaHocSinh' AND object_id = OBJECT_ID('KetQuaQuiz'))
BEGIN
    CREATE INDEX IX_KetQuaQuiz_MaHocSinh ON KetQuaQuiz(MaHocSinh);
    PRINT 'Đã tạo index IX_KetQuaQuiz_MaHocSinh';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_KetQuaQuiz_MaQuiz' AND object_id = OBJECT_ID('KetQuaQuiz'))
BEGIN
    CREATE INDEX IX_KetQuaQuiz_MaQuiz ON KetQuaQuiz(MaQuiz);
    PRINT 'Đã tạo index IX_KetQuaQuiz_MaQuiz';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_KetQuaQuiz_NgayNop' AND object_id = OBJECT_ID('KetQuaQuiz'))
BEGIN
    CREATE INDEX IX_KetQuaQuiz_NgayNop ON KetQuaQuiz(NgayNop DESC);
    PRINT 'Đã tạo index IX_KetQuaQuiz_NgayNop';
END

PRINT '=== CẬP NHẬT BẢNG KetQuaQuiz HOÀN TẤT THÀNH CÔNG ===';
GO
USE NenTangHocLieu;
GO

-- Bảng lưu chi tiết trả lời từng câu của mỗi lần làm bài
CREATE TABLE TraLoiChiTiet (
    MaTraLoi INT IDENTITY(1,1) PRIMARY KEY,
    MaKetQua INT NOT NULL,
    MaCauHoi INT NOT NULL,
    TraLoi NVARCHAR(MAX) NULL,  -- Lưu nội dung hoặc ID lựa chọn (ví dụ: "A" hoặc "1")
    DungSai BIT NOT NULL,       -- TRUE nếu đúng, FALSE nếu sai
    CONSTRAINT FK_TraLoiChiTiet_KetQua FOREIGN KEY (MaKetQua) REFERENCES KetQuaQuiz(MaKetQua) ON DELETE CASCADE,
    CONSTRAINT FK_TraLoiChiTiet_CauHoi FOREIGN KEY (MaCauHoi) REFERENCES CauHoi(MaCauHoi) ON DELETE CASCADE
);

-- Index để tìm nhanh
CREATE INDEX IX_TraLoiChiTiet_MaKetQua ON TraLoiChiTiet(MaKetQua);
GO

PRINT 'Tạo bảng TraLoiChiTiet thành công!';

PRINT N'=== TẤT CẢ STORED PROCEDURES ĐÃ ĐƯỢC TẠO THÀNH CÔNG ===';
PRINT N'=== TẠO DATABASE NenTangHocLieu HOÀN TẤT 100% ===';
GO