using AdsMarketSharing.Data;
using AdsMarketSharing.Entities;
using AdsMarketSharing.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AdsMarketSharing.Repositories
{
    public class ProductCategoryRepositoty : GenericRepository<ProductCategory>, IProductionCategoryRepository
    {
        private readonly SQLExpressContext _dbContext;

        public ProductCategoryRepositoty(SQLExpressContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<ProductCategory> Upsert(ProductCategory entity)
        {
            if (!(await _dbContext.ProductCategories.AnyAsync(categogry => categogry == entity)))
            {
                return await Add(entity);
            }

            return _dbContext.Update(entity).Entity;
        }
    }
}
