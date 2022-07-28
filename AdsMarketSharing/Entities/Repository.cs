using System.Collections.Generic;

namespace AdsMarketSharing.Entities
{
    public class Repository
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
