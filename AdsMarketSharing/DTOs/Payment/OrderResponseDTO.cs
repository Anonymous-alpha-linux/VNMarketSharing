using AdsMarketSharing.Enum;
using AdsMarketSharing.Services.Payment;
using System;

namespace AdsMarketSharing.DTOs.Payment
{
    public class OrderResponseDTO
    {
        public int Id { get; set; }
        public string BuyerFullName { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpireTime { get; set; }
        public string OrderStatus { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int MerchantId { get; set; }
        public string Merchant { get; set; }
        public int AddressId { get; set; }
        public string Address { get; set; }
        public string InvoiceRef { get; set; }
    }
}
