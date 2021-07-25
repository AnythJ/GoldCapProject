using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class DashboardDataModel
    {
        public int Day30 { get; set; }
        public List<_30daysModel> ListLast30 { get; set; }
        public List<CategoryChart> CategoryRatios { get; set; }
        public int CategoryCount { get; set; }
    }
}
