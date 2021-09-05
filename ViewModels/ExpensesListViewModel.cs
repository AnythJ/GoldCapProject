using GoldCap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.ViewModels
{
    public class ExpensesListViewModel
    {
        public IEnumerable<Expense> Expenses { get; set; }
        public List<Category> CategoriesList { get; set; }
        public string DescriptionSearch { get; set; }
        public int PriceFrom { get; set; }
        public int PriceTo { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public List<bool> ChosenCategories { get; set; }
    }
}
