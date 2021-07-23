﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class MockExpenseRepository : IExpenseRepository
    {
        private List<Expense> _expenseList;
        public MockExpenseRepository()
        {

        }

        public Expense Add(Expense expense)
        {
            expense.Id = _expenseList.Max(e => e.Id) + 1;
            _expenseList.Add(expense);

            return expense;
        }

        public IEnumerable<Expense> GetAllExpenses()
        {
            return _expenseList;
        }

        public Expense Delete(int id)
        {
            Expense expense = _expenseList.FirstOrDefault(e => e.Id == id);
            if(expense != null)
            {
                _expenseList.Remove(expense);
            }
            return expense;
        }

        public Expense GetExpense(int Id)
        {
            return _expenseList.FirstOrDefault(e => e.Id == Id);
        }

        public Expense Update(Expense expenseChanges)
        {
            Expense expense = _expenseList.FirstOrDefault(e => e.Id == expenseChanges.Id);
            if (expense != null)
            {
                expense.Amount = expenseChanges.Amount;
                expense.Category = expenseChanges.Category;
                expense.Description = expenseChanges.Description;
                expense.Date = expenseChanges.Date;
            }
            return expense;
        }

        public Category GetCategory(int Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Category> GetAllCategories()
        {
            throw new NotImplementedException();
        }

        public Category AddCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public Category DeleteCategory(int id)
        {
            throw new NotImplementedException();
        }

        public SelectList GetAllCategoriesToList()
        {
            throw new NotImplementedException();
        }

        public List<Category> GetCategoryList()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Expense> GetLast30Days()
        {
            throw new NotImplementedException();
        }

        public Expense GetExpenseFrom30DaysAgo()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Expense> GetSumDayExpense30()
        {
            throw new NotImplementedException();
        }

        List<decimal> IExpenseRepository.GetSumDayExpense30()
        {
            throw new NotImplementedException();
        }
    }
}
