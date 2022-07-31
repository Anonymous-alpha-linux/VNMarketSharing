using System;
using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.DTOs.User
{
    public class AddAddressRequestDTO
    {
        [Required]
        public string ReceiverName { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string Province { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        public string Zipcode { get; set; }
        public bool IsDefault { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public int AddressType { get; set; }
        public int UserId { get; set; }
    }
}
