using AdsMarketSharing.Data;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AdsMarketSharing.Hubs
{
    public class ReviewHub : Hub
    {
        private readonly SQLExpressContext _dbContext;
        public ReviewHub(SQLExpressContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task SendReview()
        {
            return Clients.All.SendAsync(ReviewBroadCastConstraint.RECEIVE_REVIEW);
        }
        
    }
}
