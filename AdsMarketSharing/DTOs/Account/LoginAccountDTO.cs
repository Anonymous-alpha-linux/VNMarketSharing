using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.DTOs.Account
{
    public class LoginAccountDTO
    {
        [Required(ErrorMessage = "You must fill your email")]
        [EmailAddress(ErrorMessage = "This field must be an email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password must be specified")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
