using AdsMarketSharing.Enum;
using AdsMarketSharing.Models.Token;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AdsMarketSharing.Interfaces
{
    public interface IToken
    {
        string GenerateMailToken(List<Claim> claims, TokenConfiguration<string, TokenType> tokenConfiguration);
        Task<bool> ValidateMailToken(string token);
        Task<string> GetClaimValue(string token, string claimType);
    }
}
