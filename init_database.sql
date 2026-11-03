-- 1. Tạo Database CNPM nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'CNPM')
BEGIN
    CREATE DATABASE CNPM;
END
GO

USE CNPM;
GO

-- 2. Tạo bảng VaiTro
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'VaiTro')
BEGIN
    CREATE TABLE VaiTro (
        VaiTroID INT IDENTITY(1,1) PRIMARY KEY,
        TenVaiTro NVARCHAR(50) NOT NULL UNIQUE,
        MoTa NVARCHAR(255)
    );
END
GO

-- 3. Tạo bảng TaiKhoan
-- Lưu ý: Column ID và VaiTro được map tương thích với model C# (TaiKhoanID và VaiTroID)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TaiKhoan')
BEGIN
    CREATE TABLE TaiKhoan (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        TenDangNhap NVARCHAR(50) NOT NULL UNIQUE,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        MatKhau NVARCHAR(255) NOT NULL,
        HoTen NVARCHAR(100) NOT NULL,
        VaiTro INT NOT NULL,
        TrangThai BIT NOT NULL DEFAULT 1,
        NgayTao DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

        CONSTRAINT FK_TaiKhoan_VaiTro
            FOREIGN KEY (VaiTro)
            REFERENCES VaiTro(VaiTroID)
    );
END
GO

-- 4. Tạo bảng HocPhan
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HocPhan')
BEGIN
    CREATE TABLE HocPhan (
        HocPhanID INT IDENTITY(1,1) PRIMARY KEY,
        MaHocPhan NVARCHAR(20) NOT NULL UNIQUE,
        TenHocPhan NVARCHAR(150) NOT NULL,
        SoTinChi INT NOT NULL DEFAULT 3,
        SoTietLyThuyet INT NOT NULL DEFAULT 0,
        SoTietThucHanh INT NOT NULL DEFAULT 0,
        MoTa NVARCHAR(500) NULL,
        TrangThai BIT NOT NULL DEFAULT 1
    );
END
GO

-- 5. Thêm dữ liệu mẫu (VaiTro, TaiKhoan, HocPhan)
IF NOT EXISTS (SELECT * FROM VaiTro WHERE TenVaiTro = 'Admin')
BEGIN
    INSERT INTO VaiTro (TenVaiTro, MoTa) VALUES 
    (N'Admin', N'Quản trị viên hệ thống'),
    (N'User', N'Người dùng / Giảng viên / Sinh viên');
END
GO

IF NOT EXISTS (SELECT * FROM TaiKhoan WHERE TenDangNhap = 'admin')
BEGIN
    DECLARE @AdminVaiTroID INT = (SELECT TOP 1 VaiTroID FROM VaiTro WHERE TenVaiTro = 'Admin');
    DECLARE @UserVaiTroID INT = (SELECT TOP 1 VaiTroID FROM VaiTro WHERE TenVaiTro = 'User');

    INSERT INTO TaiKhoan (TenDangNhap, Email, MatKhau, HoTen, VaiTro, TrangThai, NgayTao) VALUES
    ('admin', 'admin@example.com', 'admin123', N'Quản Trị Viên', @AdminVaiTroID, 1, SYSDATETIME()),
    ('user', 'user@example.com', 'user123', N'Người Dùng Mẫu', @UserVaiTroID, 1, SYSDATETIME());
END
GO

IF NOT EXISTS (SELECT * FROM HocPhan WHERE MaHocPhan = 'INT1306')
BEGIN
    INSERT INTO HocPhan (MaHocPhan, TenHocPhan, SoTinChi, SoTietLyThuyet, SoTietThucHanh, MoTa, TrangThai) VALUES
    ('INT1306', N'Lập trình Web nâng cao', 3, 30, 30, N'Học về ASP.NET Core, MVC và Entity Framework', 1),
    ('INT1310', N'Cơ sở dữ liệu', 3, 36, 18, N'Kiến thức về RDBMS, SQL Server, thiết kế CSDL', 1),
    ('INT1315', N'Công nghệ phần mềm', 3, 45, 0, N'Quy trình phát triển phần mềm, Agile/Scrum', 1),
    ('MAT1101', N'Toán rời rạc', 3, 45, 0, N'Đồ thị, logic toán và đại số boolean', 1);
END
GO
