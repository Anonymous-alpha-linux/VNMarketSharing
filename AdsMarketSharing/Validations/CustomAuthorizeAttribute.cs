using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace AdsMarketSharing.Validations
{
    public class CustomAuthorizeAttribute: AuthorizeAttribute
    {
    }

    internal class AuthorizePolicyProvider : IAuthorizationPolicyProvider
    {
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            throw new System.NotImplementedException();
        }
    }
}
