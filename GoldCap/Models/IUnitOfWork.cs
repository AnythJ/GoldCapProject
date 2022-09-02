using System.Threading.Tasks;

namespace GoldCap.Models
{
    public interface IUnitOfWork
    {
        IExpenseRepository ExpenseRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IRecurringRepository RecurringRepository { get; }
        IIncomeRepository IncomeRepository { get; }

        Task CompleteAsync();
    }
}
