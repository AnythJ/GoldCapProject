using System.Collections;using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public interface IGeneralRepository<T> where T : class
    {
        Task<T> GetAsync(int id);
        Task<T> AddAsync(T entity);
        Task<T> DeleteAsync(int id);
        Task<T> UpdateAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
    }
}
