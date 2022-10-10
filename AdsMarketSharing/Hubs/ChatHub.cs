using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AdsMarketSharing.Hubs
{
    public class ChatHub: Hub
    {
        public Task TestChat()
        {
            return Clients.All.SendAsync("broadcastMessage", "i'm chathub","hello world");
        }
        [Authorize]
        [HubMethodName("sendMessageToAll")]
        public Task SendMessageToAll(string name, string message) {
            return Clients.All.SendAsync("broadcastMessage", name, message);
        }
        [HubMethodName("sendMessgeToCaller")]
        public Task SendMessageToCaller(string name,string message)
        {
            return Clients.Caller.SendAsync("broadcastMessage", name, message);
        }
        [HubMethodName("sendMessageToUser")]
        public Task SendMessageToUser(string connectionId, string message)
        {
            return Clients.Client(connectionId).SendAsync("broadcastMessage", message);
        }
        [HubMethodName("joinChatRoom")]
        public Task JoinRoom(string user, string room) {
            return Groups.AddToGroupAsync(user, room);
        }
    }
}
