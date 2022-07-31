using AdsMarketSharing.Data;
using AdsMarketSharing.Entities;
using AdsMarketSharing.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AdsMarketSharing.Repositories
{
    public class ReceiverAddressRepository: GenericRepository<ReceiverAddress>, IReceiverAddressRepository
    {
        private readonly SQLExpressContext _dbContext;

        public ReceiverAddressRepository(SQLExpressContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public override async Task<ReceiverAddress> Upsert(ReceiverAddress entity)
        {
            bool hasExisted = await _dbContext.ReceiverAddresses.AnyAsync(address => address == entity) ;
            if (!hasExisted)
            {
                return await Add(entity);
            }

            return _dbContext.Update(entity).Entity;
        }
    }
}
