using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class StatPill : Expense
    {
        public int AmountInt { get; set; }
        public decimal Percentage { get; set; }
        public string DatetimeString { get; set; }
    }
}
