using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public interface IExpenseRepository
    {
        // Expense
        Task<Expense> GetExpenseAsync(int Id);
        Task<IEnumerable<Expense>> GetAllExpensesAsync();
        Task<Expense> AddAsync(Expense expense);
        Task<Expense> UpdateAsync(Expense expenseChanges);
        Task<Expense> DeleteAsync(int id);
        List<_30daysModel> GetSumDayExpense30(int period);
        List<TooltipModel> GetTooltipList(int period);
        Task<IEnumerable<Expense>> AddExpensesAsync(IEnumerable<Expense> expenses);
        IEnumerable<Expense> DeleteAllExpenses();
        

        // Category

        Task<Category> GetCategoryAsync(int Id);
        IEnumerable<Category> GetAllCategories();
        Task<Category> AddCategoryAsync(Category category);
        Task<Category> DeleteCategoryAsync(int id);

        List<Category> GetCategoryList();
        List<CategoryChart> GetCategoryRatios(int period);

        // Recurring expense

        IEnumerable<ExpenseRecurring> GetAllRecurring();
        Task<ExpenseRecurring> DeleteRecurringAsync(int id);
        Task<ExpenseRecurring> AddRecurringAsync(ExpenseRecurring expense);
        Task<ExpenseRecurring> UpdateRecurringAsync(ExpenseRecurring expenseChanges);
        Task<ExpenseRecurring> DeleteExpensesAsync(ExpenseRecurring modelExpense);
        Task<ExpenseRecurring> GetRecurringAsync(int id);

        // Income
        Task<Income> AddIncomeAsync(Income income);
        Task<Income> DeleteIncomeAsync(int id);
        IEnumerable<Income> GetIncome(string userLogin);
        Task<Income> UpdateIncomeAsync(Income incomeChanges);



    }
}
