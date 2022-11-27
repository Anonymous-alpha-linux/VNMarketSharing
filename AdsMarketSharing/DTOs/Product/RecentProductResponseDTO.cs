namespace AdsMarketSharing.DTOs.Product
{
    public class RecentProductResponseDTO
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string SellerName { get; set; }
        public string SellerAvatar { get; set; }
        public long OrderAmount { get; set; }

    }
}
