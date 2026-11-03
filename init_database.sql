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

-- 4. Thêm dữ liệu mẫu (Admin và User)
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
