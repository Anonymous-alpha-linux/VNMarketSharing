using System;

namespace AdsMarketSharing.DTOs.Notification
{
    public class CreateNotificationTrackerDTO
    {
        public int UserId { get; set; }
        public int NotifyId { get; set; }
        public bool HasSeen { get; set; } = false;
        public DateTime? SeenTime { get; set; }
    }
}
