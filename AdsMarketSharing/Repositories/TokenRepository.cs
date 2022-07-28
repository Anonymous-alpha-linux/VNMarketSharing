using System;
using System.Text;
using System.Security.Claims;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;

using AdsMarketSharing.Interfaces;
using AdsMarketSharing.Models.Token;
using AdsMarketSharing.Data;
using AdsMarketSharing.Enum;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Linq;
using AdsMarketSharing.Models;
using AdsMarketSharing.DTOs.Token;
using Microsoft.Extensions.Configuration;

namespace AdsMarketSharing.Repositories
{
    public class TokenRepository : IToken
    {
        private readonly SQLExpressContext _context;
        private readonly IConfiguration _configuration;

        public TokenRepository(SQLExpressContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<ServiceResponse<AuthTokenResponse>> GenerateAuthToken(List<Claim> claims, TokenConfiguration<string, TokenType> tokenConfiguration)
        {
            var response = new ServiceResponse<AuthTokenResponse>();

            try
            {

            }
            catch (ServiceResponseException<ResponseStatus> e)
            {

            }
            return response;
        }

        public async Task<ServiceResponse<AuthTokenResponse>> RefreshJWTToken(AuthTokenRequest tokenRequest)
        {
            var response = new ServiceResponse<AuthTokenResponse>();

            try
            {

            }
            catch (ServiceResponseException<ResponseStatus> e)
            {

                throw;
            }
            return response;
        }

        public async Task<ServiceResponse<ClaimResponse>> VerifyAuthToken(AuthTokenRequest tokenRequest)
        {
            var response = new ServiceResponse<ClaimResponse>();
            return response;
        }

        public string GenerateMailToken(List<Claim> claims,TokenConfiguration<string,TokenType> tokenConfiguration)
        {

            SymmetricSecurityKey key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(tokenConfiguration.TokenKey)
            );

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = tokenConfiguration.ExpiresTime,
                SigningCredentials = credentials,
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        public async Task<ServiceResponse<MailTokenResponse>> GenerateScriptToken(List<Claim> claims, TokenConfiguration<string, TokenType> tokenConfiguration)
        {
            var response = new ServiceResponse<MailTokenResponse>();
            return response;
        }

        public async Task<bool> ValidateMailToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken validatedToken;
                IPrincipal principal = tokenHandler.ValidateToken(token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    // Clock skew compensates for server time drift.
                    // We recommend 5 minutes or less:
                    ClockSkew = TimeSpan.FromMinutes(5),
                    // Ensure the token hasn't expired:
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    // Ensure the token audience matches our audience value (default true):
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    // Specify the key used to sign the token:
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:MailToken").Value)),
                    RequireSignedTokens = true
                },out validatedToken);

            }
            catch (Exception e)
            {

                return false;
            }
            return true;
        }

        public async Task<string> GetClaimValue(string token,string claimType) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            string stringClaimValue = securityToken.Claims.First(claim => claim.Type == claimType).Value;
            return stringClaimValue;
        }
    }
}
