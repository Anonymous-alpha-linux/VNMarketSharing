using AdsMarketSharing.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdsMarketSharing.Entities
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpireTime { get; set; } = DateTime.UtcNow.AddDays(1);
        public BankCode BankCode { get; set; }
        public string? Last4Digits { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Invoice Invoice { get; set; }
    }
}
