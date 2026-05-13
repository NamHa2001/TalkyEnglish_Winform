use DB_TalkyEnglish
GO

ALTER TABLE Courses 
ADD Level NVARCHAR(50),
    CreatedAt DATETIME DEFAULT GETDATE();

    ALTER TABLE Courses
ADD CourseCode AS ('KH' + RIGHT('000' + CAST(CourseID AS VARCHAR(3)), 3));

ALTER TABLE Courses ADD Duration NVARCHAR(100) NULL;

SELECT COLUMN_NAME, DATA_TYPE, COLUMN_DEFAULT, IS_COMPUTED 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Users';


SELECT 
    name AS [Tên cột],
    TYPE_NAME(system_type_id) AS [Kiểu dữ liệu],
    is_computed AS [Cột tính toán?],
    definition AS [Công thức tính]
FROM sys.columns c
JOIN sys.computed_columns cc ON c.object_id = cc.object_id AND c.column_id = cc.column_id
WHERE object_id = OBJECT_ID('Users');

-- 1. Thêm cột StudentCode (Mã tự tăng HV001, HV002...)
ALTER TABLE Users
ADD StudentCode AS ('HV' + RIGHT('000' + CAST([UserID] AS [varchar](3)), 3));

-- 2. Thêm cột CourseName (Tên khóa học)
ALTER TABLE Users
ADD CourseName NVARCHAR(255);

-- 3. Thêm cột Level (Trình độ)
ALTER TABLE Users
ADD [Level] NVARCHAR(100);

CREATE TABLE TeachingAssignments (
    AssignmentID INT PRIMARY KEY IDENTITY(1,1),
    InstructorID INT NOT NULL,
    CourseID INT NOT NULL,
    AssignedDate DATETIME DEFAULT GETDATE(),
    Note NVARCHAR(255),
    
    -- Ràng buộc khóa ngoại trỏ đúng vào bảng Users và Courses trong ảnh của bạn
    CONSTRAINT FK_Assignment_User FOREIGN KEY (InstructorID) REFERENCES Users(UserID),
    CONSTRAINT FK_Assignment_Course FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
);


SELECT UserID FROM Users;
SELECT CourseID FROM Courses;

USE DB_TalkyEnglish;
DELETE FROM TeachingAssignments;

select *
from TeachingAssignments


CREATE TABLE Announcements (
    AnnounceID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(250) NOT NULL,
    Content NVARCHAR(MAX),
    PublishDate DATETIME DEFAULT GETDATE(),
    Category NVARCHAR(50) -- Ví dụ: 'Lịch nghỉ', 'Khuyến mãi', 'Lớp mới'
);


CREATE TABLE Enrolments (
    EnrolmentID INT PRIMARY KEY IDENTITY(1,1),
    StudentID INT NOT NULL, -- Khóa ngoại liên kết Users.UserID
    CourseID INT NOT NULL,  -- Khóa ngoại liên kết Courses.CourseID
    EnrolDate DATETIME DEFAULT GETDATE(),
    PaymentStatus NVARCHAR(50) DEFAULT N'Chưa đóng', -- N'Đã đóng', N'Còn nợ'
    FOREIGN KEY (StudentID) REFERENCES Users(UserID),
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
);


CREATE TABLE Attendance (
    AttendanceID INT PRIMARY KEY IDENTITY(1,1),
    EnrolmentID INT NOT NULL,
    AttendanceDate DATETIME DEFAULT GETDATE(),
    IsPresent BIT DEFAULT 1, -- 1: Có mặt, 0: Vắng
    FOREIGN KEY (EnrolmentID) REFERENCES Enrolments(EnrolmentID)
);

CREATE TABLE Grades (
    GradeID INT PRIMARY KEY IDENTITY(1,1),
    EnrolmentID INT NOT NULL,
    MidTerm FLOAT DEFAULT 0,
    FinalTerm FLOAT DEFAULT 0,
    Note NVARCHAR(MAX),
    FOREIGN KEY (EnrolmentID) REFERENCES Enrolments(EnrolmentID)
);

select *
from Users


-- Xóa nếu email này đã tồn tại để tránh trùng lặp dữ liệu lỗi
DELETE FROM [Users] WHERE Email = 'admin@talky.edu.vn';

-- Chèn tài khoản Admin mới đúng theo cấu trúc cột trong ảnh eeb93a
INSERT INTO [Users] (
    [FullName], 
    [Email], 
    [PasswordHash], -- Tên cột chính xác trong ảnh của bạn
    [Role], 
    [PhoneNumber],
    [CreatedAt],
    [Gender],
    [Status]
)
VALUES (
    N'Quản Trị Viên',           -- FullName
    'admin@talky.edu.vn',       -- Email
    '123456',                   -- PasswordHash (Tạm để 123456 để test)
    'Admin',                    -- Role
    '0987654321',               -- PhoneNumber
    GETDATE(),                  -- CreatedAt
    N'Nam',                     -- Gender
    'Active'                    -- Status
);

-- Kiểm tra lại dòng vừa tạo
SELECT UserID, FullName, Email, PasswordHash, Role FROM [Users] WHERE Email = 'admin@talky.edu.vn';


INSERT INTO Announcements (Title, Content, PublishDate)
VALUES 
(N'Thông báo nghỉ lễ Quốc khánh', N'Trung tâm nghỉ từ ngày 02/09 đến hết ngày 03/09.', GETDATE()),
(N'Lịch thi cuối khóa IELTS', N'Các bạn chú ý xem lịch thi tại phòng 302 vào thứ 7.', GETDATE());

-- 1. Bổ sung các đầu điểm thành phần và tỷ lệ chuyên cần
ALTER TABLE Grades 
ADD AttendanceScore FLOAT DEFAULT 0,
    AttendancePercentage FLOAT DEFAULT 0,
    TotalSessions NVARCHAR(20);

-- 2. Bổ sung các cột kết quả tổng kết và xếp loại
ALTER TABLE Grades
ADD AverageScore FLOAT DEFAULT 0,
    GradeLetter NVARCHAR(5),
    Ranking NVARCHAR(20);

-- 3. Bổ sung phần nhận xét của giáo viên
ALTER TABLE Grades
ADD TeacherComment NVARCHAR(MAX),
    CommentDate DATETIME;


    SELECT * FROM Enrollments WHERE StudentID = 9;


    select *
from Users


DROP TABLE Enrolments;

SELECT name 
FROM sys.foreign_keys 
WHERE referenced_object_id = OBJECT_ID('Enrolments');

-- Cắt sợi dây nối với bảng Attendance
ALTER TABLE Attendance DROP CONSTRAINT FK__Attendanc__Enrol__01142BA1;

-- Cắt sợi dây nối với bảng Grades
ALTER TABLE Grades DROP CONSTRAINT FK__Grades__Enrolmen__05D8E0BE;


-- Thêm cột PaymentStatus vào bảng THẬT Enrollments (2 chữ l)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Enrollments') AND name = 'PaymentStatus')
BEGIN
    ALTER TABLE Enrollments ADD PaymentStatus NVARCHAR(50) DEFAULT N'Chưa đóng';
END