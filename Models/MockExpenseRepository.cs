﻿using System;
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
    }
}
