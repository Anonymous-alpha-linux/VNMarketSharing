using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace AdsMarketSharing.Entities
{
    public class Reply
    {
        [Key]
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ReplyFrom))]
        public int? ReviewId { get; set; }
        public Review ReplyFrom { get; set; }

        [ForeignKey(nameof(UserPage))]
        public int? UserPageId { get; set; }
        public UserPage UserPage { get; set; }
    }
}
