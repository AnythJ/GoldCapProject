using GoldCap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.ViewModels
{
    public class ExpensesListViewModel
    {
        public IEnumerable<Expense> Expenses { get; set; }
        public List<Category> CategoriesList { get; set; }
        public SortMenu SortMenu { get; set; }
        public List<Expense> NotificationList { get; set; }
    }
}
