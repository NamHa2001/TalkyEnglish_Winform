using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkyEnglish.DTO
{
   public class TeachingAssignmentDTO
    {
        [Key]
        public int AssignmentID { get; set; }
        public int InstructorID { get; set; }
        public int CourseID { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string Note { get; set; }
        [NotMapped]
        public string InstructorName { get; set; }
        [NotMapped]
        public string CourseName { get; set; }
    }
}
