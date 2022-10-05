using AdsMarketSharing.Entities;
using AdsMarketSharing.Enum;
using System;
using System.Collections.Generic;

namespace AdsMarketSharing.DTOs.Payment
{
    public class InvoiceResponseDTO
    {
        public int Id { get; set; }
        public decimal CashAmount { get; set; }
        public ShippingMethod Shipping { get; set; }
        public string OnlineRef { get; set; }
        public bool HasPaid { get; set; } = false;
        public string Bank { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderResponseDTO> Orders { get; set; }
    }
}
