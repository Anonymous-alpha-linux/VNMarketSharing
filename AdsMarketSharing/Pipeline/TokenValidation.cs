using Microsoft.AspNetCore.Http;

namespace AdsMarketSharing.Pipeline
{
    public class TokenValidation
    {
        private readonly RequestDelegate _next;
        public TokenValidation(RequestDelegate next)
        {
            _next = next;
        }

    }
}
