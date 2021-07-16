using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public interface IExpenseRepository
    {
        Expense GetExpense(int Id);
        IEnumerable<Expense> GetAllExpenses();
        Expense Add(Expense expense);
        Expense Update(Expense expenseChanges);
        Expense Delete(int id);
    }
}
