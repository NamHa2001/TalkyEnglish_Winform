using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkyEnglish.DAL;
using TalkyEnglish.DTO;

namespace TalkyEnglish.BUS
{
    public class ReportBUS
    {
        private ReportDAL _reportDAL = new ReportDAL();

        public List<RevenueReportDTO> GetRevenueByDate(DateTime fromDate, DateTime toDate)
        {
            // Tầng BUS kiểm tra logic (ví dụ: Từ ngày không được lớn hơn Đến ngày)
            if (fromDate > toDate)
            {
                throw new Exception("Lỗi: 'Từ ngày' không thể lớn hơn 'Đến ngày'!");
            }

            return _reportDAL.GetRevenueByDate(fromDate, toDate);
        }
    }
}
