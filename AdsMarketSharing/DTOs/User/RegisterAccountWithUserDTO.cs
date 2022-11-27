using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using AdsMarketSharing.Entities;
using AdsMarketSharing.DTOs.Account;
using Microsoft.AspNetCore.Http;
using AdsMarketSharing.DTOs.File;

namespace AdsMarketSharing.DTOs.User
{
    public class RegisterAccountWithUserDTO
    {
        [MaxLength(50)]
        public string OrganizationName { get; set; }
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
        public RegisterNewAccountDTO Account { get; set; }
        public IFormFile Image { get; set; }
        public AttachmentResponseDTO Avatar { get; set; }
    }
}
