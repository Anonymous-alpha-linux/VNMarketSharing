using AdsMarketSharing.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.DTOs.User
{
    public class UpdateUserRequestDTO
    {
        public string OrganizationName { get; set; }
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int AttachmentId { get; set; }
        public Attachment Avatar { get; set; }
        [Required]
        public int AccountId { get; set; }
    }
}
