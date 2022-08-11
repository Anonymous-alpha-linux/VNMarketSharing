using System;

namespace AdsMarketSharing.Entities
{
    public class ProductOffer
    {
        public int Id { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public decimal DiscountPercentage { get; set; }
        public int UserPageId { get; set; }
        public UserPage UserPage { get; set; }
        public int ProductId { get; set; }
        public Product product { get; set; }
    }
}
