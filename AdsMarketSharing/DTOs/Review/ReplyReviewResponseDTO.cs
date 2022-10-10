using AdsMarketSharing.DTOs.User;
using AdsMarketSharing.DTOs.UserPage;
using System;

namespace AdsMarketSharing.DTOs.Review
{
    public class ReplyReviewResponseDTO
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int ReviewId { get; set; }
        public DateTime CreatedAt { get; set; }
        public GetUserPageWithoutDescriptionDTO UserPage { get; set; }
    }
}
