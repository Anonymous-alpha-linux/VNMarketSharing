using AdsMarketSharing.Data;
using AdsMarketSharing.DTOs.Notification;
using AdsMarketSharing.Interfaces;
using AdsMarketSharing.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AdsMarketSharing.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly SQLExpressContext _dbContext;
        private readonly IMapper _mapper;
        public NotificationService(SQLExpressContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<Entities.Notification>> SendNotification(CreateNotificationRequestDTO request)
        {
          
            try
            {
                var newNotify = _mapper.Map<Entities.Notification>(request);

                _dbContext.Notifications.Add(newNotify);
                _dbContext.SaveChanges();
               
                return new ServiceResponse<Entities.Notification>() { 
                    Data =  newNotify,
                    Message = "Created new record of notification",
                    ServerMessage = "Sent notification",
                    Status = Enum.ResponseStatus.Successed,
                    StatusCode = 200
                };

            }
            catch (ServiceResponseException<string> response)
            {

                return new ServiceResponse<Entities.Notification>()
                {
                    Data = null,
                    Message = response.Message,
                    ServerMessage = response.Message,
                    StatusCode = 400,
                    Status = Enum.ResponseStatus.Failed
                };
            }
            
        }

        public async Task<ServiceResponse<string>> ReceivedNotification(int notifyId, int userId)
        {
            try
            {
                var notify = _dbContext.Notifytrackers.FirstOrDefault(p => p.Id == notifyId && p.UserId == userId);
                
                if (!notify.HasSeen)
                {
                    notify.HasSeen = true;
                    notify.SeenTime = DateTime.Now;
                    _dbContext.Update(notify);
                    _dbContext.SaveChanges();

                    return new ServiceResponse<string>()
                    {
                        Data = "User has accessed to this notification",
                        Message = "User has accessed to this notification",
                        ServerMessage = "Updated the notification",
                        Status = Enum.ResponseStatus.Successed,
                        StatusCode = 200
                    };
                }

                return new ServiceResponse<string>()
                {
                    Data = "",
                    Message = "",
                    ServerMessage = "Accessed to the service",
                    Status = Enum.ResponseStatus.Nothing,
                    StatusCode = 200
                };
            }
            catch (ServiceResponseException<string> error)
            {

                return new ServiceResponse<string>() { 
                    Data = "",
                    Message = "Error in server",
                    ServerMessage = error.Message,
                    StatusCode= error.StatusCode != null ? error.StatusCode : 500,
                    Status = Enum.ResponseStatus.Failed
                };

            }
        }

        
    }
}
