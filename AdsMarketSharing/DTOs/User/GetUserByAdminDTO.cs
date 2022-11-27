using System;
using AdsMarketSharing.DTOs.Account;

namespace AdsMarketSharing.DTOs.User
{
    public class GetUserByAdminDTO
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public bool Enabled { get; set; } = true;
        public bool IsActive { get; set; } = false;
        public DateTime RegisteredTime { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
    }
}
