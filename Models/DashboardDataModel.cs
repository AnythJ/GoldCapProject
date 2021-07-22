using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class DashboardDataModel
    {
        public IEnumerable<Expense> ExpensesLast30Days { get; set; }
        public int[] Last30Days { get; set; }
        public int Day30 { get; set; }
        public string TestString { get; set; }
        public Expense[] ExpensesLast30DaysArray { get; set; }
        public List<decimal> summed30 { get; set; }
    }
}
