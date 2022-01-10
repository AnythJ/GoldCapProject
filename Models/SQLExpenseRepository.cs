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
        public Expense GetExpense(int Id)
        {
            return context.Expenses.Find(Id);
        }

        public IEnumerable<Expense> GetAllExpenses()
        {
            return context.Expenses.Where(e => e.ExpenseManagerLogin == userLogin);
        }

        public Expense Add(Expense expense)
        {
            context.Expenses.Add(expense);
            context.SaveChanges();

            return expense;
        }

        public Expense Delete(int id)
        {
            Expense expense = context.Expenses.Find(id);
            if (expense != null)
            {
                context.Expenses.Remove(expense);
                context.SaveChanges();
            }
            return expense;
        }

        public Expense Update(Expense expenseChanges)
        {
            var expense = context.Expenses.Attach(expenseChanges);
            expense.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();

            return expenseChanges;
        }


        // CATEGORIES
        public Category GetCategory(int Id)
        {
            return context.Categories.Find(Id);
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return context.Categories.Where(e => e.ExpenseManagerLogin == userLogin);
        }

        public Category AddCategory(Category category)
        {
            context.Categories.Add(category);
            context.SaveChanges();

            return category;
        }

        public Category DeleteCategory(int id)
        {
            var category = context.Categories.Find(id);
            if (category != null)
            {
                context.Categories.Remove(category);
                context.SaveChanges();
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

            for (int i = 0; i <= period; i++)
            {
                var exp = context.Expenses.Where(e => (e.Date.Value.Day == DateTime.Now.AddDays(-period + i).Day &&
                e.Date.Value.Month == DateTime.Now.AddDays(-period + i).Month && e.Date.Value.Year == DateTime.Now.AddDays(-period + i).Year) && e.ExpenseManagerLogin == userLogin).AsEnumerable();

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
            for (int i = 0; i <= period; i++)
            {
                var exp = context.Expenses.Where(e => (e.Date.Value.Day == DateTime.Now.AddDays(-period + i).Day &&
                e.Date.Value.Month == DateTime.Now.AddDays(-period + i).Month && e.Date.Value.Year == DateTime.Now.AddDays(-period + i).Year) && e.ExpenseManagerLogin == userLogin).AsEnumerable();

                
                var cate = context.Expenses.Where(e => e.Date.Value.Day == DateTime.Today.AddDays(-period + i).Day
                && e.Date.Value.Month == DateTime.Today.AddDays(-period + i).Month && e.Date.Value.Year == DateTime.Today.AddDays(-period + i).Year && e.ExpenseManagerLogin == userLogin).Select(e => e.Category).ToList<string>();
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

        public ExpenseRecurring DeleteRecurring(int id)
        {
            ExpenseRecurring expense = context.RecurringExpenses.Find(id);
            if (expense != null)
            {
                context.RecurringExpenses.Remove(expense);
                context.SaveChanges();
            }
            return expense;
        }

        public ExpenseRecurring AddRecurring(ExpenseRecurring expense)
        {
            context.RecurringExpenses.Add(expense);
            context.SaveChanges();

            return expense;
        }

        public ExpenseRecurring UpdateRecurring(ExpenseRecurring expenseChanges)
        {
            var expense = context.RecurringExpenses.Attach(expenseChanges);
            expense.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();

            return expenseChanges;
        }

        public ExpenseRecurring DeleteExpenses(ExpenseRecurring modelExpense)
        {
            var list = context.Expenses.Where(e => e.StatusId == modelExpense.Id && e.ExpenseManagerLogin == userLogin);

            if (list != null)
            {
                context.Expenses.RemoveRange(list);
                context.SaveChanges();
            }
            return modelExpense;
        }

        public ExpenseRecurring GetRecurring(int Id)
        {
            return context.RecurringExpenses.Find(Id);
        }

        public IEnumerable<Expense> AddExpenses(IEnumerable<Expense> expenses)
        {
            if (expenses != null)
            {
                context.Expenses.AddRange(expenses);
                context.SaveChanges();
            }

            return expenses;
        }

        public Income AddIncome(Income income)
        {
            context.Incomes.Add(income);
            context.SaveChanges();

            return income;
        }

        public Income DeleteIncome(int id)
        {
            Income income = context.Incomes.Find(id);
            if (income != null)
            {
                context.Incomes.Remove(income);
                context.SaveChanges();
            }
            return income;
        }

        public IEnumerable<Income> GetIncome(string userLogin)
        {
            return context.Incomes.AsNoTracking().Where(i => i.ExpenseManagerLogin == userLogin).AsEnumerable();
        
        }

        public Income UpdateIncome(Income incomeChanges)
        {
            var income = context.Incomes.Attach(incomeChanges);
            income.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();

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
