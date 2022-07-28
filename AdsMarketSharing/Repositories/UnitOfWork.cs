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

        private readonly SQLExpressContext _dbContext;
        public UnitOfWork(SQLExpressContext dbContext)
        {
            _dbContext = dbContext;
            UserRepository = new UserRepository(dbContext);
            AttachmentRepository = new AttachmentRepository(dbContext);
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
