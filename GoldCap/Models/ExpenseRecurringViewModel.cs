using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class ExpenseRecurringViewModel : ExpenseRecurring
    {
        public List<bool> Weekdays { get; set; }
    }
}
