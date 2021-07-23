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
            if(expense != null)
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
            if(category != null)
            {
                context.Categories.Remove(category);
                context.SaveChanges();
            }

            return category;
        }

        // NEW
        public List<Category> GetCategoryList()
        {
            IEnumerable<Category> ctg = context.Categories;
            List<Category> ctgList = ctg.ToList();

            return ctgList;
        }

        public List<decimal> GetSumDayExpense30()
        {
            List<Expense> expensesGrouped = context.Expenses.
                Where(e => e.Date >= DateTime.Now.AddDays(-30)).AsEnumerable().OrderBy(d => d.Date).ToList();
            int[] last30 = Helper.Last30DaysArray();


            List<decimal> sumAmounts = new List<decimal>();
            for (int i = 0; i < 30; i++)
            {
                sumAmounts.Add(0);
            }

            for(int i = 0; i < 30; i++)
            {
              // sumAmounts[i] = context.Expenses.Where(d => d.Date.Value.day)
                //
            }
            
            


            return sumAmounts;
        }
    }
}
