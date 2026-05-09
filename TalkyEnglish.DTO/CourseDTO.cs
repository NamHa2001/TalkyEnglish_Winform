using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkyEnglish.DTO
{
    public class CourseDTO
    {
        public int CourseID { get; set; }
        public string? CourseName { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public int? InstructorID { get; set; }
        public int? CategoryID { get; set; }
        public string? Status { get; set; }
    }
}