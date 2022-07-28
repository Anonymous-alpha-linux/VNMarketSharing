using AdsMarketSharing.Data;
using AdsMarketSharing.Entities;
using AdsMarketSharing.Interfaces;

namespace AdsMarketSharing.Repositories
{
    public class AttachmentRepository : GenericRepository<Attachment>, IAttachmentRepository
    {
        private SQLExpressContext _dbContext;
        public AttachmentRepository(SQLExpressContext dbContext): base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
