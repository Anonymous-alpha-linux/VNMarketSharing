using AdsMarketSharing.DTOs.Token;
using AdsMarketSharing.Enum;
using AdsMarketSharing.Models;
using AdsMarketSharing.Models.Token;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AdsMarketSharing.Interfaces
{
    public interface IToken
    {
        Task<ServiceResponse<AuthTokenResponse>> GenerateAuthToken(List<Claim> claims, TokenConfiguration<string, TokenType> tokenConfiguration);
        string GenerateMailToken(List<Claim> claims, TokenConfiguration<string, TokenType> tokenConfiguration);
        Task<bool> ValidateMailToken(string token);
        Task<string> GetClaimValue(string token, string claimType);
    }
}
