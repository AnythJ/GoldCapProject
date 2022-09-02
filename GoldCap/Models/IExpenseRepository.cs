using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public interface IExpenseRepository : IGenericRepository<Expense>
    {
        Task<IEnumerable<Expense>> GetAllAsync();
        List<LineChart30DaysModel> GetSumOfExpensesInEachDayInLast30Days(int period);
        List<TooltipModel> GetTooltipList(int period);
        List<CategoryChart> GetCategoryRatios(int period);
        Task<ExpenseRecurring> DeleteExpensesAsync(ExpenseRecurring modelExpense);

        IEnumerable<Expense> GetAll();
        void DeleteExpenses(ExpenseRecurring modelExpense);
    }
}
