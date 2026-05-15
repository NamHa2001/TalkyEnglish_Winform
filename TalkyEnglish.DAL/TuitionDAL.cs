using System;
using System.Collections.Generic;
using System.Linq;
using TalkyEnglish.DTO;

namespace TalkyEnglish.DAL
{
    public class TuitionDAL
    {
        // 1. Hàm lấy danh sách học viên kèm thông tin khóa học và học phí
        public List<TuitionDTO> GetTuitionList()
        {
            using (var db = new TalkyDbContext())
            {
                var query = from e in db.Enrolments
                            join u in db.Users on e.StudentID equals u.UserID
                            join c in db.Courses on e.CourseID equals c.CourseID
                            // Sắp xếp ngày đăng ký mới nhất lên đầu cho dễ quản lý
                            orderby e.EnrollmentDate descending
                            select new TuitionDTO
                            {
                                EnrollmentID = e.EnrollmentID,
                                StudentCode = u.StudentCode,
                                FullName = u.FullName,
                                CourseName = c.CourseName,
                                Price = c.Price ?? 0,
                                EnrollmentDate = e.EnrollmentDate,
                                PaymentStatus = e.PaymentStatus
                            };
                return query.ToList();
            }
        }

        // 2. Hàm xác nhận thanh toán (Tuyệt chiêu dùng Transaction)
        // Chỉ cần 2 tham số truyền từ BUS/GUI xuống
        public bool ConfirmPayment(int enrollmentId, decimal amount)
        {
            using (var db = new TalkyDbContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var enrolment = db.Enrolments.Find(enrollmentId);
                        if (enrolment == null) return false;

                        enrolment.PaymentStatus = "Paid";

                        var newInvoice = new Invoice
                        {
                            EnrollmentID = enrollmentId,
                            // Tự động lấy StudentID từ chính lượt đăng ký này, khỏi cần truyền từ ngoài vào!
                            StudentID = enrolment.StudentID,
                            TotalAmount = amount,
                            PaymentMethod = "Tiền mặt",
                            PaymentDate = DateTime.Now
                        };
                        db.Invoices.Add(newInvoice);

                        db.SaveChanges();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
    }
}