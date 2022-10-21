using AdsMarketSharing.Enum;
using AdsMarketSharing.Services.Payment;
using AdsMarketSharing.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdsMarketSharing.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string BuyerFullName { get; set; }
        public string Description{ get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ExpireTime { get; set; } = DateTime.Now.AddDays(1);
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public OrderType Type { get; set; } = OrderType.TOPUP;

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [ForeignKey(nameof(Buyer))]
        public int BuyerId { get; set; }
        public User Buyer { get; set; }

        [ForeignKey(nameof(Merchant))]
        public int MerchantId { get; set; }
        public UserPage Merchant { get; set; }
        [ForeignKey(nameof(Address))]
        public int AddressId { get; set; }
        public ReceiverAddress Address { get; set; }

        [ForeignKey(nameof(Invoice))]
        public int? InvoiceId { get; set; }  
        public Invoice Invoice { get; set; }


        public override string ToString()
        {
            return $"Item: " + "{" +
                $"\nId: {Id} ," +
                $"\nCustomer: {BuyerFullName} " +
                //$"\nProduct name: {Product.Name} ," +
                //$"\nTo address: {Address.StreetAddress} {Address.Ward} - {Address.District} - {Address.City}" +
                //$"\nFrom: {Merchant.Name}" + 
                $"\n" + "}";
        }
    }
}
