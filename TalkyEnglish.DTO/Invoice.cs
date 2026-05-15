using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkyEnglish.DTO
{
    [Table("Invoices")]
    public class Invoice
    {
        [Key]
        public int InvoiceID { get; set; }

        public int? StudentID { get; set; } // Cột cũ

        public decimal TotalAmount { get; set; }

        [StringLength(50)]
        public string PaymentMethod { get; set; }

        public DateTime? PaymentDate { get; set; }

        public int? EnrollmentID { get; set; } // Cột khóa ngoại anh em mình vừa mới thêm!
    }
}
