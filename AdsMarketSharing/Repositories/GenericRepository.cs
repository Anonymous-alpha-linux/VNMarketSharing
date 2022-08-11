using AdsMarketSharing.Data;
using AdsMarketSharing.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AdsMarketSharing.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly SQLExpressContext _dbContext;
        DbSet<T> dbSet;
    
        public GenericRepository(SQLExpressContext dbContext)
        {
            _dbContext = dbContext;
            dbSet = _dbContext.Set<T>();
        }

        public virtual async Task<T> Add(T entity)
        {
            var newRecord = (await dbSet.AddAsync(entity)).Entity;
            return newRecord;
        }

        public virtual async Task<T> Update(int id,T entity)
        {
            var foundEntity  = await dbSet.FindAsync(id);
            if(foundEntity == null) { return null; }
            var updatedRecord = (dbSet.Update(entity)).Entity;
            return updatedRecord;
        }

        public virtual async Task<IEnumerable<T>> All()
        {
            return await dbSet.AsNoTracking().ToListAsync();
        }

        public virtual async Task<bool> Delete(int id)
        {
            var entity = await dbSet.FindAsync(id); 
            dbSet.Remove(entity);
            return true;
        }

        public virtual async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T> GetById(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual Task<T> Upsert(T entity)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<IEnumerable<T>> Sort(Expression<Func<T, bool>> predicate, string sortOrder)
        {
            return await dbSet.ToListAsync();
        }

        public virtual Task<IEnumerable<T>> Filter(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEnumerable<T>> Paging()
        {
            throw new NotImplementedException();
        }
    }
}
