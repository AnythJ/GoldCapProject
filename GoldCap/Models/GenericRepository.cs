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
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext context;
        protected readonly IHttpContextAccessor httpContextAccessor;
        protected readonly string userLogin;

        public GenericRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.userLogin = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        public virtual async Task<T> GetAsync(int id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await context.Set<T>().AddAsync(entity);

            return entity;
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities != null)
            {
                await context.Set<T>().AddRangeAsync(entities);
            }

            return entities;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            T entity = await context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                context.Set<T>().Remove(entity);
            }

            return true;
        }


        public virtual async Task<T> UpdateAsync(T entityChanged)
        {
            var entity = context.Set<T>().Attach(entityChanged);
            entity.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();

            return entityChanged;
        }

        public T Get(int id)
        {
            return context.Set<T>().Find(id);
        }

        public T Add(T entity)
        {
            context.Set<T>().Add(entity);
            context.SaveChanges();

            return entity;
        }

        public IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            if (entities != null)
            {
                context.Set<T>().AddRange(entities);
                context.SaveChanges();
            }

            return entities;
        }

        public T Delete(int id)
        {
            T entity = context.Set<T>().Find(id);
            if (entity != null)
            {
                context.Set<T>().Remove(entity);
                context.SaveChanges();
            }
            return entity;
        }


        public T Update(T entityChanged)
        {
            var entity = context.Set<T>().Attach(entityChanged);
            entity.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();

            return entityChanged;
        }
    }
}
