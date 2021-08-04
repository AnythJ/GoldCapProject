using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class DashboardDataModel
    {
        public List<_30daysModel> ListLast30 { get; set; }
        public List<CategoryChart> CategoryRatios { get; set; }
        public int CategoryCount { get; set; }
        public List<TooltipModel> TooltipList { get; set; }
        public List<Expense> ExpensesList { get; set; }
    }
}
