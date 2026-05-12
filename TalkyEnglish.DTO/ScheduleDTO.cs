using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkyEnglish.DTO
{
    public class ScheduleDTO
    {
        public DateTime Date { get; set; }
        public string DayOfWeek { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string CourseName { get; set; }
        public string TeacherName { get; set; }
        public string RoomName { get; set; }
        public string Note { get; set; }
    }
}
