using AdsMarketSharing.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.DTOs.User
{
    public class GenerateUserRequestDTO
    {
        public string OrganizationName { get; set; }
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
