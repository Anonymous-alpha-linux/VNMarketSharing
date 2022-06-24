using AutoMapper;
using AdsMarketSharing.DTOs.Account;
using AdsMarketSharing.Models.Auth;

namespace AdsMarketSharing
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AddAccountInfoDTO, Account>();
            CreateMap<Account, GetAccountInfoDTO>();
            CreateMap<AssignRoleToAccountDTO, AccountRole>();
        }
    }
}
