using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.DTOs.Account
{
    public class GetAccountInfoDTO
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string AccessToken { get; set; }
        public List<GetRoleDTO> Roles { get; set; }

    }
}
