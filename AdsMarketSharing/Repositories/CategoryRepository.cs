using AdsMarketSharing.Data;
using AdsMarketSharing.Entities;
using AdsMarketSharing.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AdsMarketSharing.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly SQLExpressContext _dbContext;
        public CategoryRepository(SQLExpressContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }
        public override async Task<Category> Upsert(Category entity)
        {
            if (!(await _dbContext.Categories.AnyAsync(user => user == entity)))
            {
                return await Add(entity);
            }

            return _dbContext.Update(entity).Entity;
        }
    }
}
