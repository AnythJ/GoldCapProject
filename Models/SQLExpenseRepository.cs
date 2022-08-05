using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class SQLExpenseRepository : IExpenseRepository
    {
        private readonly AppDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly string userLogin;

        public SQLExpenseRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.userLogin = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        // EXPENSES
        public async Task<Expense> GetExpenseAsync(int Id)
        {
            return await context.Expenses.FindAsync(Id);
        }

        public async Task<IEnumerable<Expense>> GetAllExpensesAsync()
        {
            return await context.Expenses.Where(e => e.ExpenseManagerLogin == userLogin).ToListAsync();
        }

        public async Task<Expense> AddAsync(Expense expense)
        {
            await context.Expenses.AddAsync(expense);
            await context.SaveChangesAsync();

            return expense;
        }

        public async Task<Expense> DeleteAsync(int id)
        {
            Expense expense = await context.Expenses.FindAsync(id);
            if (expense != null)
            {
                context.Expenses.Remove(expense);
                await context.SaveChangesAsync();
            }
            return expense;
        }

        public async Task<Expense> UpdateAsync(Expense expenseChanges)
        {
            var expense = context.Expenses.Attach(expenseChanges);
            expense.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await context.SaveChangesAsync();

            return expenseChanges;
        }


        // CATEGORIES
        public async Task<Category> GetCategoryAsync(int Id)
        {
            return await context.Categories.FindAsync(Id);
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return context.Categories.Where(e => e.ExpenseManagerLogin == userLogin);
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            return category;
        }

        public async Task<Category> DeleteCategoryAsync(int id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category != null)
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
            }

            return category;
        }

        // DASHBOARD
        public List<Category> GetCategoryList()
        {
            IEnumerable<Category> ctg = context.Categories.Where(e => e.ExpenseManagerLogin == userLogin);
            List<Category> ctgList = ctg.ToList();

            return ctgList;
        }

        public List<_30daysModel> GetSumDayExpense30(int period)
        {
            List<_30daysModel> finalList = new List<_30daysModel>();
            List<Expense> totalExpensesForUser = context.Expenses.Where(e => e.ExpenseManagerLogin == userLogin).ToList<Expense>();

            for (int i = 0; i <= period; i++)
            {
                IEnumerable<Expense> exp = totalExpensesForUser.Where(e => (e.Date.Value.Day == DateTime.Now.AddDays(-period + i).Day &&
                e.Date.Value.Month == DateTime.Now.AddDays(-period + i).Month && e.Date.Value.Year == DateTime.Now.AddDays(-period + i).Year)).AsEnumerable();

                _30daysModel model = new _30daysModel()
                {
                    Amount = 0,
                    TimeStamp = DateTime.Now.AddDays(-period + i).Day.ToString(),
                    OneId = -1,
                    MonthName = DateTime.Now.AddDays(-period + i).ToString("MMMM", CultureInfo.InvariantCulture)
                };

                if (exp != null)
                {
                    foreach (var item in exp)
                    {
                        model.Amount += (decimal)item.Amount;
                        model.OneId = item.Id;
                    }
                }


                finalList.Add(model);
            }

            return finalList;
        }

        public List<CategoryChart> GetMostUsedCategoryRatios()
        {
            var categoryNames = context.Categories.Where(e => e.ExpenseManagerLogin == userLogin).Select(n => n.Name).ToList();

            var expenses = context.Expenses.Where(e => (e.Date.Value >= DateTime.Now.AddDays(-30)) && e.ExpenseManagerLogin == userLogin).AsEnumerable();

            List<CategoryChart> newList = new List<CategoryChart>();


            foreach (var item in categoryNames)
            {
                CategoryChart newCat = new CategoryChart();
                newCat.CategoryName = item;
                decimal first = (decimal)expenses.Where(c => c.Category == item).Count();
                decimal second = (decimal)expenses.Count();
                if (second != 0)
                {
                    newCat.CategoryPercentage = System.Math.Round((first / second) * 100, 2);
                }
                else
                    newCat.CategoryPercentage = 0;


                newList.Add(newCat);
            }

            return newList.OrderByDescending(d => d.CategoryPercentage).ToList<CategoryChart>();
        }

        public List<CategoryChart> GetCategoryRatios(int period)
        {
            var categoryNames = context.Categories.Where(e => e.ExpenseManagerLogin == userLogin).Select(n => n.Name).ToList();

            var expenses = context.Expenses.Where(e => (e.Date.Value >= DateTime.Now.AddDays(-period)) && e.ExpenseManagerLogin == userLogin).AsEnumerable();

            List<CategoryChart> newList = new List<CategoryChart>();


            foreach (var item in categoryNames)
            {
                CategoryChart newCat = new CategoryChart();
                newCat.CategoryName = item;
                decimal first = (decimal)expenses.Where(c => c.Category == item).Sum(e => e.Amount);
                decimal second = (decimal)expenses.Sum(e => e.Amount);
                if (second != 0)
                {
                    newCat.CategoryPercentage = System.Math.Round((first / second) * 100, 2);
                }
                else
                    newCat.CategoryPercentage = 0;


                newList.Add(newCat);
            }

            return newList.OrderByDescending(d => d.CategoryPercentage).ToList<CategoryChart>();
        }

        public List<TooltipModel> GetTooltipList(int period)
        {
            List<TooltipModel> finalList = new List<TooltipModel>();
            List<Expense> totalExpensesForUser = context.Expenses.Where(e => e.ExpenseManagerLogin == userLogin).ToList<Expense>();
            for (int i = 0; i <= period; i++)
            {
                var exp = totalExpensesForUser.Where(e => (e.Date.Value.Day == DateTime.Now.AddDays(-period + i).Day &&
                e.Date.Value.Month == DateTime.Now.AddDays(-period + i).Month && e.Date.Value.Year == DateTime.Now.AddDays(-period + i).Year)).AsEnumerable();


                var cate = totalExpensesForUser.Where(e => e.Date.Value.Day == DateTime.Today.AddDays(-period + i).Day
                && e.Date.Value.Month == DateTime.Today.AddDays(-period + i).Month && e.Date.Value.Year == DateTime.Today.AddDays(-period + i).Year).Select(e => e.Category).ToList<string>();
                List<string> noRepeats = cate.Distinct().ToList();

                List<List<decimal>> amountList = new List<List<decimal>>();

                if (exp != null)
                {
                    List<decimal> smallerList = new List<decimal>();
                    foreach (var item in noRepeats)
                    {
                        var amount = exp.Where(e => e.Category == item).Select(e => e.Amount);
                        decimal fa = 0;
                        foreach (var a in amount)
                        {
                            fa += (decimal)a;
                        }
                        smallerList.Add(fa);
                    }
                    amountList.Add(smallerList);
                }

                TooltipModel model = new TooltipModel()
                {
                    CategoryListTooltip = noRepeats,
                    Amount = amountList
                };

                finalList.Add(model);
            }

            return finalList;
        }

        public IEnumerable<ExpenseRecurring> GetAllRecurring()
        {
            return context.RecurringExpenses.Where(e => e.ExpenseManagerLogin == userLogin);
        }

        public async Task<ExpenseRecurring> DeleteRecurringAsync(int id)
        {
            ExpenseRecurring expense = await context.RecurringExpenses.FindAsync(id);
            if (expense != null)
            {
                context.RecurringExpenses.Remove(expense);
                await context.SaveChangesAsync();
            }
            return expense;
        }

        public async Task<ExpenseRecurring> AddRecurringAsync(ExpenseRecurring expense)
        {
            await context.RecurringExpenses.AddAsync(expense);
            await context.SaveChangesAsync();

            return expense;
        }

        public async Task<ExpenseRecurring> UpdateRecurringAsync(ExpenseRecurring expenseChanges)
        {
            var expense = context.RecurringExpenses.Attach(expenseChanges);
            expense.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await context.SaveChangesAsync();

            return expenseChanges;
        }

        public async Task<ExpenseRecurring> DeleteExpensesAsync(ExpenseRecurring modelExpense)
        {
            var list = context.Expenses.Where(e => e.StatusId == modelExpense.Id && e.ExpenseManagerLogin == userLogin);

            if (list != null)
            {
                context.Expenses.RemoveRange(list);
                await context.SaveChangesAsync();
            }
            return modelExpense;
        }

        public async Task<ExpenseRecurring> GetRecurringAsync(int Id)
        {
            return await context.RecurringExpenses.FindAsync(Id);
        }

        public async Task<IEnumerable<Expense>> AddExpensesAsync(IEnumerable<Expense> expenses)
        {
            if (expenses != null)
            {
                await context.Expenses.AddRangeAsync(expenses);
                await context.SaveChangesAsync();
            }

            return expenses;
        }

        public async Task<Income> AddIncomeAsync(Income income)
        {
            await context.Incomes.AddAsync(income);
            await context.SaveChangesAsync();

            return income;
        }

        public async Task<Income> DeleteIncomeAsync(int id)
        {
            Income income = await context.Incomes.FindAsync(id);
            if (income != null)
            {
                context.Incomes.Remove(income);
                await context.SaveChangesAsync();
            }
            return income;
        }

        public IEnumerable<Income> GetIncome(string userLogin)
        {
            return context.Incomes.AsNoTracking().Where(i => i.ExpenseManagerLogin == userLogin).AsEnumerable();

        }

        public async Task<Income> UpdateIncomeAsync(Income incomeChanges)
        {
            var income = context.Incomes.Attach(incomeChanges);
            income.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await context.SaveChangesAsync();

            return incomeChanges;
        }

        public IEnumerable<Expense> DeleteAllExpenses()
        {
            var list = context.Expenses.Where(e => e.ExpenseManagerLogin == userLogin).ToList();

            foreach (var item in list)
            {
                context.Expenses.Remove(item);
                context.SaveChanges();
            }

            return null;
        }
    }
}
