using System;
using System.ComponentModel.DataAnnotations;
namespace AdsMarketSharing.DTOs.Token
{
    public class AuthTokenRequest
    {
            [Required]
            public string Token { get; set; }
            [Required]
            public string RefreshToken { get; set; }

    }
}
