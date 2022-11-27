namespace AdsMarketSharing.DTOs.Payment
{
    public class OrderFilterRequestDTO
    {
        public string? SearchPattern { get; set; }
        public int? OrderStatus { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public int SellerId { get; set; }
    }
}
