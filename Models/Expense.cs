using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Amount")]
        [Required(ErrorMessage = "Invalid")]
        public decimal Amount { get; set; }
        [DisplayName("Category")]
        public Ctg Category { get; set; }
        [StringLength(135)]
        [DisplayName("Description")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Invalid")]
        [DisplayName("Date")]
        public DateTime Date { get; set; }
    }
}
