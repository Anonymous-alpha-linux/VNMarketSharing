using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.Entities
{
    public class AccountRole
    {
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
