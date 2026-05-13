using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkyEnglish.DTO
{
    public class NotificationStatusDTO
    {
        public int StatusID { get; set; }
        public int AnnounceID { get; set; }
        public int UserID { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}
