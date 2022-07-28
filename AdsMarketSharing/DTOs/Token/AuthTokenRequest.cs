using System;
using System.ComponentModel.DataAnnotations;
namespace AdsMarketSharing.DTOs.Token
{
    public class AuthTokenRequest
    {
        [Required(ErrorMessage = "Accesstoken must be specific")]
        public string Token { get; set; }
        [Required(ErrorMessage = "Refreshtoken must be specific")]
        public string RefreshToken { get; set; }

    }
}
