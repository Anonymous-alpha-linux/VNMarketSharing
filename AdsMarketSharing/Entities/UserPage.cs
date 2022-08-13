using System.Collections.Generic;

namespace AdsMarketSharing.Entities
{
    public class UserPage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BannerUrl { get; set; }
        public string Biography { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public List<Product> Products { get; set; }
    }
}
