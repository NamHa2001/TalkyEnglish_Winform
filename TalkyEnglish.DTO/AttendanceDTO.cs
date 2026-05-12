using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkyEnglish.DTO
{
    public class AttendanceDTO
    {
        [DisplayName("ID Điểm Danh")]
        public int AttendanceID { get; set; }

        [Browsable(false)]
        public int EnrolmentID { get; set; }

        [DisplayName("Ngày Học")]
        public DateTime? AttendanceDate { get; set; }

        [DisplayName("Trạng Thái")]
        public bool IsPresent { get; set; } // True: Có mặt, False: Vắng

        // --- Trường bổ trợ hiển thị ---
        [DisplayName("Ghi Chú")]
        public string? Note { get; set; }
    }
}