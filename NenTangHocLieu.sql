USE master;
GO
-- Xóa database nếu tồn tại và chuyển sang chế độ độc quyền để đảm bảo DROP thành công
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'NenTangHocLieu')
BEGIN
    ALTER DATABASE NenTangHocLieu SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE NenTangHocLieu;
END
GO

-- Tạo và sử dụng Database mới
CREATE DATABASE NenTangHocLieu;
GO
USE NenTangHocLieu;
GO

-- ==============================
-- BẢNG VAI TRÒ NGƯỜI DÙNG
-- ==============================
CREATE TABLE VaiTro (
    MaVaiTro INT IDENTITY(1,1) PRIMARY KEY,
    TenVaiTro NVARCHAR(50) NOT NULL
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
-- BẢNG MÔN HỌC
-- ==============================
CREATE TABLE MonHoc (
    MaMonHoc INT IDENTITY(1,1) PRIMARY KEY,
    TenMonHoc NVARCHAR(100) NOT NULL
);
GO

-- ==============================
-- BẢNG CHỦ ĐỀ
-- ==============================
CREATE TABLE ChuDe (
    MaChuDe INT IDENTITY(1,1) PRIMARY KEY,
    TenChuDe NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255)
);
GO

-- ==============================
-- BẢNG LỚP HỌC
-- ==============================
CREATE TABLE LopHoc (
    MaLopHoc INT IDENTITY(1,1) PRIMARY KEY,
    TenLopHoc NVARCHAR(50) NOT NULL
);
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
    TrangThaiDuyet NVARCHAR(20) DEFAULT N'Chờ duyệt'
        CHECK (TrangThaiDuyet IN (N'Chờ duyệt', N'Đã duyệt', N'Từ chối')),
    NguoiDuyet INT,
    NgayDuyet DATETIME,
    LyDoTuChoi NVARCHAR(500),
    LuotXem INT DEFAULT 0,
    SoLuotTai INT DEFAULT 0,
    DiemTrungBinh FLOAT DEFAULT 0,
    CONSTRAINT FK_HocLieu_ChuDe FOREIGN KEY (MaChuDe)
        REFERENCES ChuDe(MaChuDe) ON DELETE SET NULL,
    CONSTRAINT FK_HocLieu_NguoiDung FOREIGN KEY (MaNguoiDung)
        REFERENCES NguoiDung(MaNguoiDung) ON DELETE SET NULL,
    CONSTRAINT FK_HocLieu_MonHoc FOREIGN KEY (MaMonHoc)
        REFERENCES MonHoc(MaMonHoc) ON DELETE SET NULL,
    CONSTRAINT FK_HocLieu_LopHoc FOREIGN KEY (MaLopHoc)
        REFERENCES LopHoc(MaLopHoc) ON DELETE SET NULL
);
GO

-- ==============================
-- BẢNG BÌNH LUẬN VÀ ĐÁNH GIÁ
-- ==============================
CREATE TABLE BinhLuan (
    MaBinhLuan INT IDENTITY(1,1) PRIMARY KEY,
    MaHocLieu INT,
    MaNguoiDung INT,
    NoiDung NVARCHAR(500),
    DanhGia INT CHECK (DanhGia BETWEEN 1 AND 5), -- Điểm đánh giá từ 1 đến 5
    MaBinhLuanCha INT,                          -- Dùng cho bình luận phản hồi (reply)
    NgayDang DATETIME DEFAULT GETDATE(),
    TrangThai BIT DEFAULT 1,
    CONSTRAINT FK_BinhLuan_HocLieu FOREIGN KEY (MaHocLieu)
        REFERENCES HocLieu(MaHocLieu) ON DELETE CASCADE,
    CONSTRAINT FK_BinhLuan_NguoiDung FOREIGN KEY (MaNguoiDung)
        REFERENCES NguoiDung(MaNguoiDung) ON DELETE SET NULL,
    CONSTRAINT FK_BinhLuan_Cha FOREIGN KEY (MaBinhLuanCha)
        REFERENCES BinhLuan(MaBinhLuan) -- Tham chiếu đến bình luận gốc
);
GO

-- ==============================
-- DỮ LIỆU MẪU BAN ĐẦU
-- ==============================
INSERT INTO VaiTro (MaVaiTro, TenVaiTro) 
VALUES  (N'1',N'Admin'), 
        (N'2', N'Giảng viên'), 
        (N'3', N'Sinh viên');
INSERT INTO MonHoc (MaMonHoc, TenMonHoc) VALUES
(N'1', N'Lập trình Java'), 
(N'2', N'Cơ sở dữ liệu'),
(N'3', N'Mạng máy tính'), 
(N'4', N'Phân tích hệ thống'),
(N'5', N'Kỹ năng mềm');
INSERT INTO LopHoc (MaLopHoc, TenLopHoc) VALUES 
(N'CNTT2A'), 
(N'CNTT2B'), 
(N'CNTT2C');
INSERT INTO ChuDe (TenChuDe, MoTa) VALUES
(N'Lập trình hướng đối tượng', N'Khái niệm lớp, kế thừa, đa hình'),
(N'Cấu trúc dữ liệu', N'Danh sách, ngăn xếp, hàng đợi'),
(N'Cơ sở dữ liệu quan hệ', N'Khóa chính, khóa ngoại, ràng buộc'),
(N'Quản lý file', N'Xử lý tệp trong C#'),
(N'Lập trình web', N'HTML, CSS, ASP.NET MVC');
INSERT INTO NguoiDung (HoTen, Email, MatKhau, MaVaiTro, GioiTinh, DiaChi)
VALUES
(N'Nguyễn Văn A', N'vana@example.com', N'123456', 1, N'Nam', N'Đà Nẵng'), -- 1: Admin
(N'Trần Thị B', N'thib@example.com', N'123456', 2, N'Nữ', N'Hà Nội'),      -- 2: Giảng viên
(N'Lê Văn C', N'vanc@example.com', N'123456', 2, N'Nam', N'Hồ Chí Minh'),    -- 3: Giảng viên
(N'Phạm Thị D', N'thid@example.com', N'123456', 3, N'Nữ', N'Đà Nẵng'),      -- 4: Sinh viên
(N'Hoàng Văn E', N'vane@example.com', N'123456', 3, N'Nam', N'Huế');        -- 5: Sinh viên
INSERT INTO HocLieu (TieuDe, MoTa, DuongDanTep, LoaiTep, DoKho, MaChuDe, MaNguoiDung, MaMonHoc, MaLopHoc, DaDuyet, TrangThaiDuyet)
VALUES
(N'Bài giảng OOP Cơ bản', N'/uploads/oop.pdf', N'PDF', N'Dễ', 1, 2, 1, 1, 1, N'Đã duyệt'), -- HL 1
(N'Video hướng dẫn SQL Join', N'/uploads/sqljoin.mp4', N'Video', N'Trung bình', 3, 2, 2, 2, 1, N'Đã duyệt'), -- HL 2
(N'Bài tập Java Collections', N'/uploads/collections.docx', N'Word', N'Khó', 2, 3, 1, 1, 0, N'Chờ duyệt'), -- HL 3 (Chưa duyệt)
(N'PowerPoint Mạng máy tính', N'/uploads/mang.pptx', N'PPT', N'Trung bình', 2, 2, 3, 1, 1, N'Đã duyệt'), -- HL 4
(N'HTML cơ bản', N'/uploads/html.pdf', N'PDF', N'Dễ', 5, 3, 5, 3, 1, N'Đã duyệt'); -- HL 5
GO

-- ==============================
-- BỔ SUNG DỮ LIỆU MẪU CHO BẢNG BÌNH LUẬN
-- ==============================
INSERT INTO BinhLuan (MaHocLieu, MaNguoiDung, NoiDung, DanhGia, MaBinhLuanCha)
VALUES
(1, 4, N'Bài giảng rất dễ hiểu, giúp em nắm vững kiến thức về lớp và đối tượng.', 5, NULL),  -- BL 1: SV D đánh giá HL 1 (OOP)
(2, 5, N'Video chất lượng, hình ảnh minh họa rõ ràng về các loại join.', 4, NULL),           -- BL 2: SV E đánh giá HL 2 (SQL Join)
(1, 3, N'Nội dung chuẩn, phù hợp cho sinh viên mới bắt đầu, đáng giá 5 sao!', 5, NULL),     -- BL 3: GV C đánh giá HL 1 (OOP)
(4, 4, N'Slide trình bày đẹp, thông tin đầy đủ về mô hình TCP/IP. Rất hữu ích.', 5, NULL),   -- BL 4: SV D đánh giá HL 4 (Mạng)
(5, 5, N'Tài liệu HTML này hơi cũ, cần cập nhật thêm về HTML5 và CSS Grid.', 3, NULL);      -- BL 5: SV E đánh giá HL 5 (HTML)
GO

-- Cập nhật bình luận phản hồi (reply)
INSERT INTO BinhLuan (MaHocLieu, MaNguoiDung, NoiDung, DanhGia, MaBinhLuanCha)
VALUES
(1, 2, N'Cảm ơn phản hồi tích cực của bạn. Tôi sẽ xem xét bổ sung thêm ví dụ thực tế.', NULL, 1); -- BL 6: GV B (người đăng HL 1) trả lời BL 1
GO

-- ==============================
-- CẬP NHẬT ĐIỂM TRUNG BÌNH CHO HỌC LIỆU
-- ==============================
UPDATE HL
SET DiemTrungBinh = ISNULL(
    (
        SELECT AVG(CAST(DanhGia AS FLOAT))
        FROM BinhLuan BL
        WHERE BL.MaHocLieu = HL.MaHocLieu AND BL.DanhGia IS NOT NULL
    ), 0
)
FROM HocLieu HL
WHERE HL.MaHocLieu IN (SELECT DISTINCT MaHocLieu FROM BinhLuan WHERE DanhGia IS NOT NULL);
GO

-- ===================================================================================
-- STORED PROCEDURES (Áp dụng cho NenTangHocLieu)
-- ===================================================================================

-- DROP procedures if they already exist
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'psDeleteHocLieu') DROP PROC psDeleteHocLieu;
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'psLoginNguoiDung') DROP PROC psLoginNguoiDung;
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'psInsertHocLieu') DROP PROC psInsertHocLieu;
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'psUpdateHocLieu') DROP PROC psUpdateHocLieu;
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'psGetMonHoc') DROP PROC psGetMonHoc;
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'psGetHocLieu') DROP PROC psGetHocLieu;
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'psUpdateNguoiDung') DROP PROC psUpdateNguoiDung; -- DROP PROCEDURE MỚI
GO

-- 1. psDeleteHocLieu (Tương đương psDeleteRecordSANPHAM)
-- Xóa một bản ghi Học Liệu dựa trên MaHocLieu
CREATE PROC [dbo].[psDeleteHocLieu] (@MaHocLieu INT)
AS
BEGIN
    TRANSACTION;
    
    DELETE FROM HocLieu WHERE MaHocLieu = @MaHocLieu;
    
    IF(@@ERROR<>0)
        ROLLBACK TRANSACTION
    ELSE
        COMMIT TRANSACTION
END
GO

-- 2. psLoginNguoiDung (Tương đương psGetTableLOGIN)
-- Kiểm tra thông tin đăng nhập bằng Email và MatKhau
CREATE PROC [dbo].[psLoginNguoiDung] (@Email NVARCHAR(100), @MatKhau NVARCHAR(255))
AS
BEGIN
    TRANSACTION;
    
    SELECT MaNguoiDung, HoTen, Email, MaVaiTro, NgayTao
    FROM NguoiDung
    WHERE Email = @Email AND MatKhau = @MatKhau;
    
    IF(@@ERROR<>0)
        ROLLBACK TRANSACTION
    ELSE
        COMMIT TRANSACTION
END
GO

-- 3. psInsertHocLieu (Tương đương psInsertRecordSANPHAM)
-- Thêm một bản ghi Học Liệu mới
CREATE PROC [dbo].[psInsertHocLieu] (
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
    TRANSACTION;
    
    INSERT INTO HocLieu(TieuDe, MoTa, DuongDanTep, LoaiTep, KichThuocTep, DoKho, MaChuDe, MaNguoiDung, MaMonHoc, MaLopHoc)
    VALUES
    (@TieuDe, @MoTa, @DuongDanTep, @LoaiTep, @KichThuocTep, @DoKho, @MaChuDe, @MaNguoiDung, @MaMonHoc, @MaLopHoc);
    
    IF(@@ERROR<>0)
        ROLLBACK TRANSACTION
    ELSE
        COMMIT TRANSACTION
END
GO

-- 4. psUpdateHocLieu (Tương đương psUpdateRecordSANPHAM)
-- Cập nhật thông tin Học Liệu
CREATE PROC [dbo].[psUpdateHocLieu] (
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
    @DaDuyet BIT
)
AS
BEGIN
    TRANSACTION;
    
    UPDATE HocLieu
    SET TieuDe = @TieuDe,
        MoTa = @MoTa,
        DuongDanTep = @DuongDanTep,
        LoaiTep = @LoaiTep,
        KichThuocTep = @KichThuocTep,
        DoKho = @DoKho,
        MaChuDe = @MaChuDe,
        MaMonHoc = @MaMonHoc,
        MaLopHoc = @MaLopHoc,
        DaDuyet = @DaDuyet -- Có thể cập nhật thêm trạng thái duyệt
    WHERE MaHocLieu = @MaHocLieu;
    
    IF(@@ERROR<>0)
        ROLLBACK TRANSACTION
    ELSE
        COMMIT TRANSACTION
END
GO

-- 5. psGetMonHoc (Tương đương psGetTableDanhMuc)
-- Lấy thông tin một hoặc tất cả các Môn Học
CREATE PROC [dbo].[psGetMonHoc] (@MaMonHoc INT = NULL)
AS
BEGIN
    TRANSACTION;
    
    IF (@MaMonHoc IS NULL)
        SELECT MaMonHoc, TenMonHoc FROM MonHoc;
    ELSE
        SELECT MaMonHoc, TenMonHoc FROM MonHoc WHERE MaMonHoc = @MaMonHoc;
        
    IF(@@ERROR <> 0)
        ROLLBACK TRANSACTION
    ELSE
        COMMIT TRANSACTION
END
GO

-- 6. psGetHocLieu (Tương đương psGetTableSANPHAM)
-- Lấy thông tin một hoặc tất cả các Học Liệu
CREATE PROC [dbo].[psGetHocLieu] (@MaHocLieu INT = NULL)
AS
BEGIN
    TRANSACTION;
    
    IF (@MaHocLieu IS NULL)
        SELECT * FROM HocLieu;
    ELSE
        SELECT * FROM HocLieu WHERE MaHocLieu = @MaHocLieu;
        
    IF(@@ERROR<>0)
        ROLLBACK TRANSACTION
    ELSE
        COMMIT TRANSACTION
END
GO

-- 7. psUpdateNguoiDung (THỦ TỤC MỚI: Cập nhật thông tin người dùng)
-- Cập nhật thông tin cá nhân của người dùng
CREATE PROC [dbo].[psUpdateNguoiDung] (
    @MaNguoiDung INT,
    @HoTen NVARCHAR(100),
    @Email NVARCHAR(100),
    @MatKhau NVARCHAR(255),
    @AnhDaiDien NVARCHAR(255) = NULL,
    @SoDienThoai NVARCHAR(15) = NULL,
    @NgaySinh DATE = NULL,
    @GioiTinh NVARCHAR(10) = NULL,
    @DiaChi NVARCHAR(255) = NULL
    -- Thường không cho phép người dùng tự cập nhật MaVaiTro hoặc TrangThai
)
AS
BEGIN
    TRANSACTION;
    
    UPDATE NguoiDung
    SET HoTen = @HoTen,
        Email = @Email,
        MatKhau = @MatKhau,
        -- Sử dụng ISNULL để nếu tham số truyền vào là NULL, giữ lại giá trị cũ
        AnhDaiDien = ISNULL(@AnhDaiDien, AnhDaiDien),
        SoDienThoai = ISNULL(@SoDienThoai, SoDienThoai),
        NgaySinh = ISNULL(@NgaySinh, NgaySinh),
        GioiTinh = ISNULL(@GioiTinh, GioiTinh),
        DiaChi = ISNULL(@DiaChi, DiaChi)
    WHERE MaNguoiDung = @MaNguoiDung;
    
    IF (@@ERROR <> 0)
    BEGIN
        ROLLBACK TRANSACTION;
    END
    ELSE
    BEGIN
        COMMIT TRANSACTION;
    END
END
GO