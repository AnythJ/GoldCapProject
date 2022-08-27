using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class RecurringRepository : GenericRepository<ExpenseRecurring>, IRecurringRepository
    {
        public RecurringRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor) { }

       

        public IEnumerable<ExpenseRecurring> GetAll()
        {
            return context.RecurringExpenses.Where(e => e.ExpenseManagerLogin == userLogin);
        }
    }
}
