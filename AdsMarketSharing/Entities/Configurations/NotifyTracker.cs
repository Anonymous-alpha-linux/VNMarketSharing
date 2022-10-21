using System;

namespace AdsMarketSharing.Entities.Configurations
{
    public class NotifyTracker
    {
        public int Id { get; set; }
        public int NotifyId { get; set; }
        public Notification Notification { get; set; }
        public int ToUserId { get; set; }
        public User ToUser { get; set; }
        public bool HasRead { get; set; }
        public DateTime? SeenTime { get; set; }
    }
}
