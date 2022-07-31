using System;
using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.DTOs.User
{
    public class UpdateAddressRequestDTO
    {
        public string ReceiverName { get; set; }
        public string StreetAddress { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Zipcode { get; set; }
        public bool IsDefault { get; set; } = false;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
