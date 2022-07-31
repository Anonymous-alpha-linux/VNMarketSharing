﻿using AutoMapper;
using AdsMarketSharing.DTOs.Account;
using AdsMarketSharing.Entities;
using AdsMarketSharing.DTOs.File;
using AdsMarketSharing.DTOs.User;

namespace AdsMarketSharing
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterNewAccountDTO, Account>();
            CreateMap<Account, GetAccountInfoDTO>();
            CreateMap<AssignRoleToAccountDTO, AccountRole>();
            CreateMap<AttachmentResponseDTO, Attachment>();
            CreateMap<GenerateUserRequestDTO, User>();
            CreateMap<AddAddressRequestDTO, ReceiverAddress>();
            CreateMap<UpdateAddressRequestDTO, ReceiverAddress>();
        }
    }
}
