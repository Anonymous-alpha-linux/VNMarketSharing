using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.DTOs.Product
{
    public class FilterProductRequestDTO
    {
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;
        [Range(0, 10)]
        public int Take { get; set; } = 5;
        public bool? FollowAlpha { get; set; }
        public bool? FollowPrice { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public bool? FollowRating { get; set; }
        public int? CategoryId { get; set; }
    }
}
