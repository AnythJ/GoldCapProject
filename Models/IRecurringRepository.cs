using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public interface IRecurringRepository : IGenericRepository<ExpenseRecurring>
    {
        public IEnumerable<ExpenseRecurring> GetAll();
    }
}
