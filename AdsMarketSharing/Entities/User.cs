using Microsoft.AspNetCore.Http;
using System;

namespace AdsMarketSharing.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int AttachmentId { get; set; }
        public Attachment Avatar { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
