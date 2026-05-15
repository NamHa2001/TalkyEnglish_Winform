using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkyEnglish.DTO
{
    public class RevenueReportDTO
    {
        public string MaHoaDon { get; set; }
        public string TenHocVien { get; set; }
        public string TenKhoaHoc { get; set; }
        public decimal SoTien { get; set; }
        public DateTime NgayThu { get; set; }
    }
}
