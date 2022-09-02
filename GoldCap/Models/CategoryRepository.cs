using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace GoldCap.Models
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor) { }

        public IEnumerable<Category> GetAll()
        {
            return context.Categories.Where(e => e.ExpenseManagerLogin == userLogin);
        }
    }
}
