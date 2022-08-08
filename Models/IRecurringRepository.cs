using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public interface IRecurringRepository : IGeneralRepository<ExpenseRecurring>
    {
        public IEnumerable<ExpenseRecurring> GetAll();
    }
}
