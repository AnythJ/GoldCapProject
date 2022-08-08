using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class GeneralRepository<T> : IGeneralRepository<T> where T : class
    {
        protected readonly AppDbContext context;
        protected readonly IHttpContextAccessor httpContextAccessor;
        protected readonly string userLogin;

        public GeneralRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.userLogin = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<T> GetAsync(int id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            await context.Set<T>().AddAsync(entity);
            await context.SaveChangesAsync();

            return entity;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities != null)
            {
                await context.Set<T>().AddRangeAsync(entities);
                await context.SaveChangesAsync();
            }

            return entities;
        }

        public async Task<T> DeleteAsync(int id)
        {
            T entity = await context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                context.Set<T>().Remove(entity);
                await context.SaveChangesAsync();
            }
            return entity;
        }


        public async Task<T> UpdateAsync(T entityChanged)
        {
            var entity = context.Set<T>().Attach(entityChanged);
            entity.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await context.SaveChangesAsync();

            return entityChanged;
        }
    }
}
