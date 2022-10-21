using System;

namespace AdsMarketSharing.Entities
{
    public class Notifytracker
    {
        public int Id { get; set; }
        public int NotifyId { get; set; }
        public Notification Notification { get; set; }
        public bool HasSeen { get; set; }
        public DateTime? SeenTime { get; set; }
        public int UserId { get; set; }
        public User ToUser { get; set; }
    }
}