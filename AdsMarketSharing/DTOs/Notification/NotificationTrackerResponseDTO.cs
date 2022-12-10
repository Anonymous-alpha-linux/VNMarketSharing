using System;

namespace AdsMarketSharing.DTOs.Notification
{
    public class NotificationTrackerResponseDTO
    {
        public int NotifyId { get; set; }
        public bool HasSeen { get; set; }
        public DateTime? SeenTime { get; set; }
        public NotificationResponseDTO Notification { get; set; }
    }
}
