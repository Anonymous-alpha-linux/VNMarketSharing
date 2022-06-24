using AdsMarketSharing.Enum;

namespace AdsMarketSharing.Models.Ads
{
    public class Banner
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BannerId { get; set; }
        public string Image_URL { get; set; }
        public CollaboratorDomain CollaboratorDomain { get; set; } = CollaboratorDomain.Blogger;
    }
}
