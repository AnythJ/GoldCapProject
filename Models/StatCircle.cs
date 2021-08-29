using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class StatCircle
    {
        public string PercentageStartRight { get; set; }
        public string PercentageLeft { get; set; }
        public string PercentageRight { get; set; }
        public string LeftSpeed { get; set; }
        public string RightSpeed { get; set; }
        public decimal TooltipPercentage { get; set; }
        public int SumLast30Days { get; set; }
        public int SumBeforeLast30Days { get; set; }
        public int UnderMonthAmount { get; set; }
        public decimal TopCategoryPercentage { get; set; }
        public string TopCategoryName { get; set; }
        public int TopCategoryAmount { get; set; }
        public string PeriodName { get; set; }
        public int TotalIncome { get; set; }
        public decimal IncomePercentage { get; set; }

    }
}
