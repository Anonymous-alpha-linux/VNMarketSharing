using Microsoft.IdentityModel.Tokens;

namespace AdsMarketSharing.DTOs.Token
{
    public class TokenResponse
    {
        public string MailToken { get; set; }
        public SecurityToken Token { get; set; }
    }
}
