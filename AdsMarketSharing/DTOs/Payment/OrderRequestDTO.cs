using AdsMarketSharing.Services.Payment;

namespace AdsMarketSharing.DTOs.Payment
{
    public class OrderRequestDTO
    {
        public int OrderId { get; set; }
        public OrderStatus Status { get; set; }
    }
}
