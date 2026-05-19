using System;
using System.Collections.Generic;
using System.Linq;
using TalkyEnglish.DTO;
using Microsoft.EntityFrameworkCore;

namespace TalkyEnglish.DAL
{
    public class GradesDAL
    {


        public List<GradesDTO> GetGradesByStudentID(int studentId)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    var query = from en in db.Enrolments
                                join c in db.Courses on en.CourseID equals c.CourseID
                                // Join sang bảng Người dùng (Users) để lấy tên Giảng viên phụ trách khóa học
                                join u in db.Users on c.InstructorID equals u.UserID into teacherGroup
                                from t in teacherGroup.DefaultIfEmpty()
                                    // Join sang bảng Điểm số (Grades)
                                join g in db.Grades on en.EnrollmentID equals g.EnrolmentID into gradeGroup
                                from g in gradeGroup.DefaultIfEmpty()
                                where en.StudentID == studentId
                                select new GradesDTO
                                {
                                    EnrolmentID = en.EnrollmentID,
                                    CourseName = c.CourseName,
                                    TeacherName = t != null ? t.FullName : "Chưa phân công",
                                    Semester = "HK1/2026", // Hoặc c.Semester tùy thuộc vào database của nhóm bro

                                    // Lấy điểm số gốc từ DB bảng điểm
                                    AttendanceScore = g != null ? g.AttendanceScore : 0,
                                    MidTerm = g != null ? g.MidTerm : 0,
                                    FinalTerm = g != null ? g.FinalTerm : 0,
                                    AverageScore = g != null ? g.AverageScore : 0,
                                    Note = g != null ? g.Note : "Chưa có nhận xét từ giảng viên.",
                                    CommentDate = g != null ? g.CommentDate : null,

                                    // Tính toán Xếp loại và Điểm chữ trực tiếp phục vụ cho giao diện học viên
                                    Ranking = g != null ? (g.AverageScore >= 8.0 ? "Giỏi" : g.AverageScore >= 6.5 ? "Khá" : g.AverageScore >= 5.0 ? "Trung Bình" : "Yếu") : "Chưa nhập điểm",
                                    GradeLetter = g != null ? (g.AverageScore >= 8.5 ? "A" : g.AverageScore >= 7.0 ? "B" : g.AverageScore >= 5.5 ? "C" : "D") : "-"
                                };

                    return query.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL GetGradesByStudentID: " + ex.Message);
                }
            }
        }
        // 1. Lấy danh sách học viên thuộc một khóa học kèm điểm số của họ (nếu có)
        public List<GradesDTO> GetStudentGradesByCourse(int courseId)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    // ĐÃ SỬA: Join thêm bảng Courses để lấy đúng CourseName gốc dưới SQL
                    var query = from en in db.Enrolments
                                join u in db.Users on en.StudentID equals u.UserID
                                join c in db.Courses on en.CourseID equals c.CourseID // Lấy tên khóa học từ đây
                                join g in db.Grades on en.EnrollmentID equals g.EnrolmentID into gradeGroup
                                from g in gradeGroup.DefaultIfEmpty()
                                where en.CourseID == courseId && u.Role == "Student"
                                select new GradesDTO
                                {
                                    EnrolmentID = en.EnrollmentID,
                                    GradeID = u.UserID, // MƯỢN TẠM GradeID để chứa UserID (Mã học viên số) đổ lên cột Mã học viên
                                    CourseName = c.CourseName, // ĐÃ SỬA: Lấy c.CourseName chuẩn 100% thay vì en.CourseName bị rỗng
                                    TeacherName = u.FullName, // MƯỢN TẠM TeacherName để chứa tên thật của Học Viên
                                    Semester = g != null ? "Đã nhập điểm" : "Chưa nhập điểm", // MƯỢN TẠM Semester để làm trạng thái hiển thị

                                    // Giữ nguyên các đầu điểm
                                    AttendanceScore = g != null ? g.AttendanceScore : 0,
                                    MidTerm = g != null ? g.MidTerm : 0,
                                    FinalTerm = g != null ? g.FinalTerm : 0,
                                    AverageScore = g != null ? g.AverageScore : 0,
                                    Note = g != null ? g.Note : ""
                                };

                    return query.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL GetStudentGradesByCourse: " + ex.Message);
                }
            }
        }

        // 2. Hàm Lưu hoặc Cập nhật điểm của học viên xuống Database
        public bool SaveOrUpdateGrade(GradesDTO gradeDto)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    // Kiểm tra xem bản ghi điểm ứng với EnrollmentID này đã tồn tại chưa
                    var existingGrade = db.Grades.FirstOrDefault(g => g.EnrolmentID == gradeDto.EnrolmentID);

                    if (existingGrade != null)
                    {
                        // Nếu đã có điểm -> Tiến hành cập nhật (UPDATE)
                        existingGrade.AttendanceScore = gradeDto.AttendanceScore;
                        existingGrade.MidTerm = gradeDto.MidTerm; // Điểm Bài Tập mượn biến MidTerm
                        existingGrade.FinalTerm = gradeDto.FinalTerm;
                        existingGrade.AverageScore = gradeDto.AverageScore;
                        existingGrade.Note = gradeDto.Note;
                        existingGrade.CommentDate = DateTime.Now;
                    }
                    else
                    {
                        // ĐÃ SỬA: Sử dụng trực tiếp class GradesDTO (Vì trong DbContext của nhóm bro map file này làm gốc)
                        var newGrade = new GradesDTO
                        {
                            EnrolmentID = gradeDto.EnrolmentID,
                            AttendanceScore = gradeDto.AttendanceScore,
                            MidTerm = gradeDto.MidTerm,
                            FinalTerm = gradeDto.FinalTerm,
                            AverageScore = gradeDto.AverageScore,
                            Note = gradeDto.Note,
                            CommentDate = DateTime.Now
                        };
                        db.Grades.Add(newGrade);
                    }

                    return db.SaveChanges() > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL SaveOrUpdateGrade: " + ex.Message);
                }
            }
        }
    }
}