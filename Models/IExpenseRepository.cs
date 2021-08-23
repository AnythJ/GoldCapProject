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
        Expense GetExpense(int Id);
        IEnumerable<Expense> GetAllExpenses();
        Expense Add(Expense expense);
        Expense Update(Expense expenseChanges);
        Expense Delete(int id);
        List<_30daysModel> GetSumDayExpense30();
        List<TooltipModel> GetTooltipList();
        IEnumerable<Expense> AddExpenses(IEnumerable<Expense> expenses);
        

        // Category

        Category GetCategory(int Id);
        IEnumerable<Category> GetAllCategories();
        Category AddCategory(Category category);
        Category DeleteCategory(int id);

        // NEW
        List<Category> GetCategoryList();
        List<CategoryChart> GetCategoryRatios();

        // Recurring expense

        IEnumerable<ExpenseRecurring> GetAllRecurring();
        ExpenseRecurring DeleteRecurring(int id);
        ExpenseRecurring AddRecurring(ExpenseRecurring expense);
        ExpenseRecurring UpdateRecurring(ExpenseRecurring expenseChanges);
        ExpenseRecurring DeleteExpenses(ExpenseRecurring modelExpense);
        ExpenseRecurring GetRecurring(int id);

        // Income
        Income AddIncome(Income income);
        Income DeleteIncome(int id);
        IEnumerable<Income> GetIncome(string userLogin);
        Income UpdateIncome(Income incomeChanges);



    }
}
