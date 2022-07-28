using System.Collections.Generic;

namespace AdsMarketSharing.DTOs.Account
{
    public class GetUserAccountDTO
    {
        public string AccountId { get; set; }
        public string Email { get; set; }
        public List<GetRoleDTO> Roles { get; set; }
    }
}
