using AdsMarketSharing.Enum;
using AdsMarketSharing.Services.Payment;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using AdsMarketSharing.DTOs.User;

namespace AdsMarketSharing.DTOs.Payment
{
    public class OrderCreationDTO
    {
        public string BuyerFullName { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        private decimal _total = 0;
        public virtual decimal Total
        {
            get => _total;
            set
            {
                _total = Price * Amount;
            }
        }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpireTime { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public OrderType Type { get; set; } = OrderType.TOPUP;
        public string ProductImage { get; set; }
        public int ProductId { get; set; }
        public int BuyerId { get; set; }
        public int MerchantId { get; set; }
        public int? AddressId { get; set; }
        public AddAddressRequestDTO?  Address { get; set; }
        //public InvoiceCreationDTO Invoice { get; set; }
    }
}
