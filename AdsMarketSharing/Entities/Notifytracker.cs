using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdsMarketSharing.Entities
{
    public class Notifytracker
    {
        // Own Properties
        public int Id { get; set; }
        public bool HasSeen { get; set; }
        public DateTime? SeenTime { get; set; }

        // Relationship
        [ForeignKey(nameof(Notification))]
        public int NotifyId { get; set; }
        public Notification Notification { get; set; }
        public int UserId { get; set; }
        public User ToUser { get; set; }
    }
}