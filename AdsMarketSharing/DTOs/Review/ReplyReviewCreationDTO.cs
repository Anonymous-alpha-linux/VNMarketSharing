using System;

namespace AdsMarketSharing.DTOs.Review
{
    public class ReplyReviewCreationDTO
    {
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public int UserPageId { get; set; }
    }
}
