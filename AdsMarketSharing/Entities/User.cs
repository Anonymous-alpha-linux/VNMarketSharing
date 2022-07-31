using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int AttachmentId { get; set; }
        public Attachment Avatar { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public List<ReceiverAddress> receiverAddress { get;set; }
    }
}
