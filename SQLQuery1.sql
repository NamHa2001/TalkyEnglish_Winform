CREATE DATABASE DB_TalkyEnglish;
GO
USE DB_TalkyEnglish;
GO

-- 1. Bảng Danh mục khóa học (Categories)
CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX)
);

-- 2. Bảng Người dùng (Users) - Lưu chung Admin, Giảng viên, Học viên
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARCHAR(MAX) NOT NULL, -- Sẽ lưu BCrypt Hash
    Role NVARCHAR(20) CHECK (Role IN ('Admin', 'Instructor', 'Student')),
    PhoneNumber VARCHAR(15),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- 3. Bảng Khóa học (Courses)
CREATE TABLE Courses (
    CourseID INT PRIMARY KEY IDENTITY(1,1),
    CourseName NVARCHAR(200) NOT NULL,
    Price DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Description NVARCHAR(MAX),
    InstructorID INT, -- Khóa ngoại liên kết Users(Role='Instructor')
    CategoryID INT,
    Status NVARCHAR(50) DEFAULT 'Active', -- Active, Draft, Archived
    CONSTRAINT FK_Course_Instructor FOREIGN KEY (InstructorID) REFERENCES Users(UserID),
    CONSTRAINT FK_Course_Category FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);

-- 4. Bảng Đăng ký (Enrollments) - Quan hệ n-n giữa Học viên và Khóa học
CREATE TABLE Enrollments (
    EnrollmentID INT PRIMARY KEY IDENTITY(1,1),
    StudentID INT,
    CourseID INT,
    EnrollmentDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Enroll_Student FOREIGN KEY (StudentID) REFERENCES Users(UserID),
    CONSTRAINT FK_Enroll_Course FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
);

-- 5. Bảng Hóa đơn (Invoices)
CREATE TABLE Invoices (
    InvoiceID INT PRIMARY KEY IDENTITY(1,1),
    StudentID INT,
    TotalAmount DECIMAL(18, 2) NOT NULL,
    PaymentMethod NVARCHAR(50), -- MoMo, Banking, Card
    PaymentDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Invoice_Student FOREIGN KEY (StudentID) REFERENCES Users(UserID)
);
GO



-- Thêm Admin mẫu (Mật khẩu giả định: admin123)
INSERT INTO Users (FullName, Email, PasswordHash, Role) 
VALUES (N'Quản trị viên', 'admin@system.com', 'hashed_string_here', 'Admin');

-- Thêm Danh mục
INSERT INTO Categories (CategoryName) VALUES (N'Lập trình'), (N'Thiết kế đồ họa'), (N'Marketing');

-- Thêm Giảng viên mẫu
INSERT INTO Users (FullName, Email, PasswordHash, Role) 
VALUES (N'Nguyễn Văn A', 'instructor1@edu.com', 'hashed_string', 'Instructor');

-- Thêm Khóa học mẫu
INSERT INTO Courses (CourseName, Price, InstructorID, CategoryID)
VALUES (N'Lập trình C# WinForms cơ bản', 500000, 2, 1);

select *
from Users

INSERT INTO Users (FullName, Email, PasswordHash, Role, CreatedAt) 
VALUES (N'Thầy Giáo', 'admin@talky.com', '123', 'Admin', GETDATE());

ALTER TABLE Users ADD Birthday DATETIME NULL;
ALTER TABLE Users ADD Gender NVARCHAR(20) NULL;

INSERT INTO Users (FullName, Email, PasswordHash, Role, PhoneNumber, Birthday, Gender, CreatedAt)
VALUES 
(N'Nguyễn Văn Học', 'vanhoc@gmail.com', '123', 'Student', '0912345678', '2005-05-15', N'Nam', GETDATE()),
(N'Trần Thị Minh Anh', 'minhanh@gmail.com', '123', 'Student', '0987654321', '2006-10-20', N'Nữ', GETDATE()),
(N'Lê Hoàng Nam', 'hoangnam@gmail.com', '123', 'Student', '0905112233', '2004-02-28', N'Nam', GETDATE()),
(N'Phạm Thanh Thảo', 'thanhthao@gmail.com', '123', 'Student', '0933445566', '2007-12-12', N'Nữ', GETDATE()),
(N'Đỗ Quốc Bảo', 'quocbao@gmail.com', '123', 'Student', '0977889900', '2005-08-05', N'Khác', GETDATE());
GO

-- 3. Kiểm tra lại dữ liệu vừa thêm
SELECT * FROM Users WHERE Role = 'Student';

ALTER TABLE Users
ADD Degree NVARCHAR(100) NULL,
    Specialization NVARCHAR(100) NULL,
    Status NVARCHAR(50) NULL;

    UPDATE Users
SET Degree = N'Chưa cập nhật'
WHERE Degree IS NULL;

UPDATE Users
SET Specialization = N'Chưa cập nhật'
WHERE Specialization IS NULL;

UPDATE Users
SET Status = N'Đang hoạt động'
WHERE Status IS NULL;

INSERT INTO Users (FullName, Email, PhoneNumber, PasswordHash, Birthday, Gender, Role, CreatedAt, Degree, Specialization, Status)
VALUES (
    N'Nguyễn Thị Anh Thư', 
    'vanna.instructor@gmail.com', 
    '0987654321', 
    '123', 
    '1990-05-20', 
    N'Nữ', 
    'Instructor', 
    GETDATE(), 
    N'Thạc sĩ', 
    N'Tiếng Anh Giao tiếp', 
    N'Đang hoạt động'
);