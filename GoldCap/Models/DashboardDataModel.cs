﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class DashboardDataModel
    {
        public List<LineChart30DaysModel> ListLast30 { get; set; }
        public List<CategoryChart> CategoryRatios { get; set; }
        public int CategoryCount { get; set; }
        public List<TooltipModel> TooltipList { get; set; }
        public IEnumerable<Expense> ExpensesList { get; set; }
    }
}
