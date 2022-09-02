using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetAsync(int id);
        Task<T> AddAsync(T entity);
        Task<bool> DeleteAsync(int id);
        Task<T> UpdateAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        T Get(int id);
        T Add(T entity);
        T Delete(int id);
        T Update(T entity);
        IEnumerable<T> AddRange(IEnumerable<T> entities);
    }
}
