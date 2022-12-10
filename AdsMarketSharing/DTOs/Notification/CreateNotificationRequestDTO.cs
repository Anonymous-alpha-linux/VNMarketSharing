using AdsMarketSharing.Enum;
using System;
using System.Collections.Generic;

namespace AdsMarketSharing.DTOs.Notification
{
    public class CreateNotificationRequestDTO
    {
        public string Title { get; set; }
        public string ShortMessage { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public NotifyType Type { get; set; }
        public int UserId { get; set; }
        public List<int> ToUsers { get; set; }
    }
}
