using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.Models.Auth
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<AccountRole> accountRoles { get; set; }
    }
}
