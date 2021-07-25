using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class SQLExpenseRepository : IExpenseRepository
    {
        private readonly AppDbContext context;

        public SQLExpenseRepository(AppDbContext context)
        {
            this.context = context;
        }

        // EXPENSES
        public Expense GetExpense(int Id)
        {
            return context.Expenses.Find(Id);
        }

        public IEnumerable<Expense> GetAllExpenses()
        {
            return context.Expenses;
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
            return context.Categories;
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
            IEnumerable<Category> ctg = context.Categories;
            List<Category> ctgList = ctg.ToList();

            return ctgList;
        }

        public List<_30daysModel> GetSumDayExpense30()
        {
            List<_30daysModel> finalList = new List<_30daysModel>();

            for (int i = 0; i <= 30; i++)
            {
                var exp = context.Expenses.Where(e => (e.Date.Value.Day == DateTime.Now.AddDays(-30 + i).Day &&
                e.Date.Value.Month == DateTime.Now.AddDays(-30 + i).Month)).AsEnumerable();

                _30daysModel model = new _30daysModel()
                {
                    Amount = 0,
                    //TimeStamp = DateTime.Now.AddDays(-30 + i).Day.ToString() + " / " + (ShortMonth)(DateTime.Today.Month)
                    TimeStamp = DateTime.Now.AddDays(-30 + i).Day.ToString()
                };

                if (exp != null)
                {
                    foreach (var item in exp)
                    {
                        model.Amount += (decimal)item.Amount;
                    }
                }


                finalList.Add(model);
            }

            return finalList;
        }

        public List<CategoryChart> GetCategoryRatios()
        {
            var categoryNames = context.Categories.Select(n => n.Name).ToList();

            var expenses = context.Expenses.Where(e => (e.Date.Value >= DateTime.Now.AddDays(-30))).AsEnumerable();

            List<CategoryChart> newList = new List<CategoryChart>();
            

            foreach (var item in categoryNames)
            {
                CategoryChart newCat = new CategoryChart();
                newCat.CategoryName = item;
                decimal first = (decimal)expenses.Where(c => c.Category == item).Count();
                decimal second = (decimal)expenses.Count();
                newCat.CategoryPercentage = (first / second)*100;

                newList.Add(newCat);
            }

            return newList.OrderByDescending(d => d.CategoryPercentage).ToList<CategoryChart>();
        }
    }
}
