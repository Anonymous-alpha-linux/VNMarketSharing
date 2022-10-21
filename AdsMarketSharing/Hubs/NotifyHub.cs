using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AdsMarketSharing.Hubs
{
    public class NotifyHub: Hub
    {
        public Task NotifyReview(string name, string message)
        {
            return Clients.All.SendAsync("broadcastNotify", name, message);
        }

        //public Task NotifyNewProduct(int productId) {
           
        //}

        //public Task NotifyProductInspection(int productId)
        //{

        //}
    }
}
