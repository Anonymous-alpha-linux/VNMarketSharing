using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AdsMarketSharing.Pipeline
{
    public class EmailVerification
    {
        public readonly IHttpContextAccessor _httpContextAccessor;
        public readonly RequestDelegate _next;

        public EmailVerification(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public EmailVerification(IHttpContextAccessor httpContextAccessor, RequestDelegate next) : this(httpContextAccessor)
        {
            _next = next;
        }

        // Pipeline stage:
        public async Task ConfirmEmail(HttpContext context)
        {
           
        }

    }
}
