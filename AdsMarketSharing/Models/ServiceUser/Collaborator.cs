using System;
using System.Collections.Generic;

using AdsMarketSharing.Enum;

namespace AdsMarketSharing.Models.ServiceUser
{
    public class Collaborator
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Hosting_URL { get; set; }
        public int Port { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LeaveAt { get; set; }
        //public List<Payment> Payments { get; set; }
        public string Token { get; set; }
        public CollaboratorStatus CurrentStatus { get; set; }
    }
}
