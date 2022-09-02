using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class SortMenu
    {
        public string DescriptionSearch { get; set; }

        [Display(Prompt = "From")]
        public decimal PriceFrom { get; set; }

        [Display(Prompt = "To")]
        public decimal PriceTo { get; set; }

        [Range(typeof(DateTime), "1/1/2000", "1/1/2040", ErrorMessage = "Invalid date")]
        public DateTime? DateFrom { get; set; }

        [Range(typeof(DateTime), "1/1/2000", "1/1/2040", ErrorMessage = "Invalid date")]
        public DateTime? DateTo { get; set; }

        public List<bool> ChosenCategories { get; set; }
    }
}
