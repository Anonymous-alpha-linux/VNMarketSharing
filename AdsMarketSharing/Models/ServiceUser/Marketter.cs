using AdsMarketSharing.Enum;
using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.Models.ServiceUser
{
    public class Marketter
    {
        [Key]
        public int Id { get; set; }
        public string Orginization_Name { get; set; }
        public CollaboratorDomain Domain { get; set; }
    }
}
