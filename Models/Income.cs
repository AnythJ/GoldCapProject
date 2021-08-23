using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class Income
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Amount")]
        [Required(ErrorMessage = "Invalid")]
        public decimal? Amount { get; set; }

        [StringLength(135)]
        [DisplayName("Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Invalid")]
        [DisplayName("First Paycheck Date")]
        [Column(TypeName = "smalldatetime")]
        [Range(typeof(DateTime), "1/1/2000", "1/1/2040", ErrorMessage = "Invalid date")]
        public DateTime? Date { get; set; }
        public string ExpenseManagerLogin { get; set; }
    }
}
