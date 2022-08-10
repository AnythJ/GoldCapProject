using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class ExpenseRepository : GeneralRepository<Expense>, IExpenseRepository
    {
        public ExpenseRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor) { }

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

        public async Task<IEnumerable<Expense>> GetAllAsync()
        {
            return await context.Expenses.Where(e => e.ExpenseManagerLogin == userLogin).ToListAsync();
        }

        public List<CategoryChart> GetCategoryRatios(int period)
        {
            var categoryNames = context.Categories.Where(e => e.ExpenseManagerLogin == userLogin).Select(n => n.Name).ToList();

            var expenses = context.Expenses.Where(e => (e.Date.Value >= DateTime.Now.AddDays(-period)) && e.ExpenseManagerLogin == userLogin).AsEnumerable();

            List<CategoryChart> newList = new();


            foreach (var item in categoryNames)
            {
                CategoryChart newCat = new();
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

        public List<LineChart30DaysModel> GetSumOfExpensesInEachDayInLast30Days(int period)
        {
            List<LineChart30DaysModel> finalList = new();
            List<Expense> totalExpensesForUser = context.Expenses.Where(e => e.ExpenseManagerLogin == userLogin).ToList<Expense>();

            for (int i = 0; i <= period; i++)
            {
                IEnumerable<Expense> exp = totalExpensesForUser.Where(e => (e.Date.Value.Day == DateTime.Now.AddDays(-period + i).Day &&
                e.Date.Value.Month == DateTime.Now.AddDays(-period + i).Month && 
                e.Date.Value.Year == DateTime.Now.AddDays(-period + i).Year)).AsEnumerable();

                LineChart30DaysModel model = new()
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

                List<List<decimal>> amountList = new();

                if (exp != null)
                {
                    List<decimal> smallerList = new();
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
    }
}
