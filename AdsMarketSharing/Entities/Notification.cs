using AdsMarketSharing.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdsMarketSharing.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortMessage { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public NotifyType Type { get; set; }
        [ForeignKey(nameof(FromUser))]
        public int UserId { get; set; }
        public User FromUser { get; set; }
        public List<Notifytracker> Notifytrackers { get; set; }
    }
}
