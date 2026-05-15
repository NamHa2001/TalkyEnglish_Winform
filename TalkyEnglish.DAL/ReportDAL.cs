using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkyEnglish.DTO;

namespace TalkyEnglish.DAL
{
    public class ReportDAL
    {
        public List<RevenueReportDTO> GetRevenueByDate(DateTime fromDate, DateTime toDate)
        {
            using (var db = new TalkyDbContext())
            {
                var query = from i in db.Invoices
                            join e in db.Enrolments on i.EnrollmentID equals e.EnrollmentID
                            join u in db.Users on e.StudentID equals u.UserID
                            join c in db.Courses on e.CourseID equals c.CourseID
                            // SỬA Ở ĐÂY: Thêm .HasValue để check null, và dùng .Value.Date
                            where i.PaymentDate.HasValue
                                  && i.PaymentDate.Value.Date >= fromDate.Date
                                  && i.PaymentDate.Value.Date <= toDate.Date
                            orderby i.PaymentDate descending
                            select new RevenueReportDTO
                            {
                                MaHoaDon = "HD" + i.InvoiceID.ToString("D3"),
                                TenHocVien = u.FullName,
                                TenKhoaHoc = c.CourseName,
                                SoTien = i.TotalAmount,
                                // SỬA Ở ĐÂY: Dùng .Value để ép kiểu DateTime? về DateTime thường
                                NgayThu = i.PaymentDate.Value
                            };

                return query.ToList();
            }
        }
    }
}
