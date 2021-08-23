using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class ExpenseRecurring
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Amount")]
        [Required(ErrorMessage = "Invalid")]
        public decimal? Amount { get; set; }

        [DisplayName("Category")]
        [Required(ErrorMessage = "Required")]
        public string Category { get; set; }

        [StringLength(135)]
        [DisplayName("Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Invalid")]
        [DisplayName("Date")]
        [Column(TypeName = "smalldatetime")]
        [Range(typeof(DateTime), "1/1/2000", "1/1/2040", ErrorMessage = "Invalid date")]
        public DateTime? Date { get; set; }

        [DisplayName("Status")]
        [Required(ErrorMessage = "Status is required")]
        public int Status { get; set; }
        public int? HowOften { get; set; }
        public string WeekdaysInString { get; set; }
        public string ExpenseManagerLogin { get; set; }
    }
}
