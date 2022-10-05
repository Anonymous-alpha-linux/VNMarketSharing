using AdsMarketSharing.Enum;
using AdsMarketSharing.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdsMarketSharing.Entities
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }
        public decimal CashAmount { get; set; }
        public ShippingMethod Shipping { get; set; }
        public string OnlineRef { get; set; }
        public bool HasPaid { get; set; } = false;
        [ForeignKey(nameof(User))]
        public int? UserId { get; set; }
        public User User { get; set; }
        [ForeignKey(nameof(Payment))]
        public int? PaymentId { get; set; }
        public Payment Payment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<Order> Orders { get; set; }
    }
}
