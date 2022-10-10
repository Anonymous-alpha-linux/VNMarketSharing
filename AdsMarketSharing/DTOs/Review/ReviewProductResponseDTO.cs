using AdsMarketSharing.DTOs.User;
using System;
using System.Collections.Generic;

namespace AdsMarketSharing.DTOs.Review
{
    public class ReviewProductResponseDTO
    {
        public int Id { get; set; }
        public int Rate { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Comment { get; set; }
        public GetUserWithoutBiographyDTO User { get; set; }
        public int ReplyAmount { get; set; } = 0;
        public List<ReplyReviewResponseDTO> Replies { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
