using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GoldCap.Models
{
    public class IncomeRepository : GeneralRepository<Income>, IIncomeRepository
    {
        public IncomeRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor) { }

        public IEnumerable<Income> GetAll(string userLogin)
        {
            return context.Incomes.AsNoTracking().Where(i => i.ExpenseManagerLogin == userLogin).AsEnumerable();
        }
    }
}
