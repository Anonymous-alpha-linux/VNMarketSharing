using System;
using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.Models.Token
{
    public class Tokens
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
