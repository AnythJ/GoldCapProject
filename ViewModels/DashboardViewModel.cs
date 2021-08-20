using GoldCap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.ViewModels
{
    public class DashboardViewModel
    {
        public IEnumerable<Expense> Expenses { get; set; }
        public IEnumerable<CategoryChart> Categories { get; set; }
        public List<StatCircle> CirclesStats { get; set; }
        public List<StatPill> PillsStats { get; set; }


    }
}
