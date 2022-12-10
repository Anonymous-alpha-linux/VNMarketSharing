using AdsMarketSharing.Entities;
using AdsMarketSharing.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace AdsMarketSharing.DTOs.Notification
{
    public class NotificationResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortMessage { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public NotifyType Type { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
