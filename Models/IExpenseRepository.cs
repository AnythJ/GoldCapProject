using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public interface IExpenseRepository : IGeneralRepository<Expense>
    {
        Task<IEnumerable<Expense>> GetAllAsync();
        List<_30daysModel> GetSumDayExpense30(int period);
        List<TooltipModel> GetTooltipList(int period);
        List<CategoryChart> GetCategoryRatios(int period);
        Task<ExpenseRecurring> DeleteExpensesAsync(ExpenseRecurring modelExpense);

    }
}
