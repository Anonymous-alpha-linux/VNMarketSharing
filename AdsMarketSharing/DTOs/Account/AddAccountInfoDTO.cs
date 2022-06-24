using AdsMarketSharing.Enum;
using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.DTOs.Account
{
    public class AddAccountInfoDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(10, ErrorMessage = "Password must be  more than 10 characters to ensure your persistence")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessage ="Password and confirm password did not match")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Please select your role")]
        public int RoleId { get; set; }
    }
}
