using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AdsMarketSharing.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> All();
        Task<T> GetById(int id);
        Task<T> Add(T entity);
        Task<bool> Delete(int id);
        Task<T> Upsert(T entity);
        Task<T> Update(int id, T entity);
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> Sort(Expression<Func<T, bool>> predicate, string sortOrder);
        Task<IEnumerable<T>> Filter(Expression<Func<T, bool>> predicate );
        Task<IEnumerable<T>> Paging();
    }
}
