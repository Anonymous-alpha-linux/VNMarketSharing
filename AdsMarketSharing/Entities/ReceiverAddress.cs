using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdsMarketSharing.Entities
{
    public class ReceiverAddress
    {
        [Key]
        public int Id { get; set; }
        public string ReceiverName { get; set; }
        public string StreetAddress { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Zipcode { get; set; }
        public bool IsDefault { get; set; } = false;
        public DateTime CreatedAt {get;set;}
        public DateTime UpdatedAt { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public int AddressType { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
