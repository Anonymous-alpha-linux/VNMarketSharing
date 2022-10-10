using System;

namespace AdsMarketSharing.DTOs.Review
{
    public class ReviewProductCreationDTO
    {
        public string Name { get; set; }
        public int Rate { get; set; }
        public string Subject { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public int ProductId { get; set; }
    }
}
