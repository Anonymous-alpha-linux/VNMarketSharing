using AdsMarketSharing.DTOs.Notification;
using AdsMarketSharing.Entities;
using AdsMarketSharing.Models;
using System.Threading.Tasks;

namespace AdsMarketSharing.Interfaces
{
    public interface INotificationService
    {
        Task<ServiceResponse<Notification>> SendNotification(CreateNotificationRequestDTO request);

        Task<ServiceResponse<string>> ReceivedNotification(int notifyId, int userId);
    }
}
