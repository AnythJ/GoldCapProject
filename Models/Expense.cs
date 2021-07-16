using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public Ctg Category { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}
