using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AdsMarketSharing.Hubs
{
    public class NotifyHub: Hub
    {
        public async Task Send(string name, string message)
        {
            await Clients.All.SendAsync("broadcastNotify", name, message);
        }
    }
}
