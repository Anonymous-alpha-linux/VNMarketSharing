using AdsMarketSharing.Data;
using AdsMarketSharing.DTOs.Notification;
using AdsMarketSharing.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AdsMarketSharing.Hubs
{
    [Authorize]
    public class NotifyHub: Hub
    {
        private readonly SQLExpressContext _dbContext;
        private readonly IMapper _mapper;

        public NotifyHub(SQLExpressContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        //public Task TestNotify()
        //{
        //    var adminList = _dbContext.AccountRoles
        //        .Include(p => p.Account)
        //            .ThenInclude(p => p.User)
        //        .Include(p => p.Role)
        //        .Where(p => p.Role.Name == Static.AccountRole.Administrator);

        //    return Clients.Users(adminList.Select(p => p.AccountId.ToString()).ToList())
        //        .SendAsync(NotifyBroadCastConstraints.RECEIVE_NOTIFY, "Receive notification successfully");
        //}
        public Task NotifyNewProduct(string url)
        {
            string accountIdString = Context.User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;
            int.TryParse(accountIdString, out int accountId);

            var account = _dbContext.Accounts.Include(p => p.User).FirstOrDefault(p => p.Id == accountId);

            var adminList = _dbContext.AccountRoles
                .Include(p => p.Account)
                    .ThenInclude(p => p.User)
                .Include(p => p.Role)
                .Where(p => p.Role.Name == Static.AccountRole.Administrator);

            var receiverList = adminList.Select(p => p.Account.User.Id).ToList();

            var newNotification = new CreateNotificationRequestDTO()
            {
                CreatedAt = DateTime.Now,
                ShortMessage = "Created new product",
                Title = account.User.OrganizationName,
                ToUsers = receiverList,
                UserId = account.User.Id,
                Type = Enum.NotifyType.NewProduct,
                Url = url,
            };

            var dbNotification = _mapper.Map<Notification>(newNotification);


            _dbContext.Notifications.Add(dbNotification);
            _dbContext.SaveChanges();

            var sentNotification = _mapper.Map<NotificationResponseDTO>(dbNotification);

            var sentResource = new NotificationTrackerResponseDTO()
            {
                Notification = sentNotification,
                HasSeen = false,
                SeenTime = null,
                NotifyId = sentNotification.Id
            };

            return Clients.Users(adminList.Select(p => p.AccountId.ToString()).ToList())
                .SendAsync(NotifyBroadCastConstraints.RECEIVE_NOTIFY, sentResource);
        }
        public Task NotifyReview(string name, string message)
        {
            return Clients.All.SendAsync("broadcastNotify", name, message);
        }

        //public Task NotifyProductInspection(int productId)
        //{

        //}
    }
}
