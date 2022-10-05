using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.DTOs.UserPage
{
    public class UserPageCreationDTO
    {
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
        [MaxLength(30)]
        public string Description { get; set; }
        [MaxLength(120)]
        public string Biography { get; set; }
        [Phone]
        public string Phone { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}
