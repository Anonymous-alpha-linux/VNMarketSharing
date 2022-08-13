using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.DTOs.Product
{
    public class FilterProductRequestDTO
    {
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;
        [Range(0, 5)]
        public int Take { get; set; } = 5;
        public bool FollowAlpha { get; set; } = true;
        public bool FollowPrice { get; set; } = false;
        public int MinPrice { get; set; } = 0;
        public int MaxPrice { get; set; } = int.MaxValue;
        public bool FollowRating { get; set; } = true;
    }
}
