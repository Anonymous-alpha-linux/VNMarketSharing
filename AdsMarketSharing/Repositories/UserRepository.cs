using AdsMarketSharing.Data;
using AdsMarketSharing.DTOs.User;
using AdsMarketSharing.Entities;
using AdsMarketSharing.Enum;
using AdsMarketSharing.Interfaces;
using AdsMarketSharing.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AdsMarketSharing.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly SQLExpressContext _dbContext;
        
        public UserRepository(SQLExpressContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<User> Upsert(User entity)
        {
            if(!(await _dbContext.Users.AnyAsync(user => user == entity))) {
                return await Add(entity);
            }

            return _dbContext.Update(entity).Entity;
        }
    }
}
