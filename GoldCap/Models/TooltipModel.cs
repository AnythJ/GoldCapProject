using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class TooltipModel
    {
        public List<List<decimal>> Amount { get; set; }
        public List<string> CategoryListTooltip { get; set; }
    }
}
