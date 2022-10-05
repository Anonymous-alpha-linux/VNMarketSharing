using AdsMarketSharing.Enum;
using System;

namespace AdsMarketSharing.DTOs.Payment
{
    public class PaymentCreationDTO
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpireTime { get; set; } = DateTime.UtcNow.AddDays(1);
        public BankCode BankCode { get; set; }
        public string? CardNumber { get; set; }
        public int UserId { get; set; }
    }
}
