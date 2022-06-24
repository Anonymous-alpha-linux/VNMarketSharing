using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.Models.Auth
{
    public class AccountRole
    {
        [Key]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
