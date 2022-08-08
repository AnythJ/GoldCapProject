using System.Collections.Generic;

namespace GoldCap.Models
{
    public interface ICategoryRepository : IGeneralRepository<Category>
    {
        public IEnumerable<Category> GetAll();
    }
}
