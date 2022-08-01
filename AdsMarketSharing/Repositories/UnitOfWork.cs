using System;
using System.Threading.Tasks;
using AdsMarketSharing.Data;
using AdsMarketSharing.Interfaces;

namespace AdsMarketSharing.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        public IUserRepository UserRepository { get; private set; }
        public IAttachmentRepository AttachmentRepository { get; private set; }
        public IReceiverAddressRepository ReceiverAddressRepository { get; private set; }

        public IProductionCategoryRepository ProductionCategoryRepository { get; private set; }
        public ICategoryRepository CategoryRepository { get; private set; }


        private readonly SQLExpressContext _dbContext;
        public UnitOfWork(SQLExpressContext dbContext)
        {
            _dbContext = dbContext;
            UserRepository = new UserRepository(dbContext);
            AttachmentRepository = new AttachmentRepository(dbContext);
            ReceiverAddressRepository = new ReceiverAddressRepository(dbContext);
            ProductionCategoryRepository = new ProductCategoryRepositoty(dbContext);
            CategoryRepository = new CategoryRepository(dbContext);
            
        }

        public async Task CompleteAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
