using System.Collections.Generic;

namespace GoldCap.Models
{
    public interface IIncomeRepository : IGenericRepository<Income>
    {
        public IEnumerable<Income> GetAll(string userLogin);
    }
}
