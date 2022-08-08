using System.Collections.Generic;

namespace GoldCap.Models
{
    public interface IIncomeRepository : IGeneralRepository<Income>
    {
        public IEnumerable<Income> GetAll(string userLogin);
    }
}
