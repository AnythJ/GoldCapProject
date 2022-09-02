using System.Collections.Generic;

namespace GoldCap.Models
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        public IEnumerable<Category> GetAll();
    }
}
