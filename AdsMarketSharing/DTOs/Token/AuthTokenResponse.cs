using Microsoft.IdentityModel.Tokens;

namespace AdsMarketSharing.DTOs.Token
{
    public class AuthTokenResponse
    {
        public string JWTToken { get; set; }
        public string RefreshToken { get; set; }
        public SecurityToken Token { get; set; }
    }
}
