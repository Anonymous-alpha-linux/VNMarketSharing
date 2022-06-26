using AdsMarketSharing.Enum;
using AdsMarketSharing.Models.Token;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.Models.Auth
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool Enabled { get; set; } = true;
        public bool IsActive { get; set; } = false;
        public DateTime RegisteredTime { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
        public AccountStatus Status { get; set; } = AccountStatus.Newbie;
        public List<AccountRole> AccountRoles { get; set; }
        public List<RefreshToken> RefreshToken { get; set; }
    }

}
