using System;

namespace AdsMarketSharing.DTOs.UserPage
{
    public class GetUserPageWithoutDescriptionDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PageAvatar { get; set; }
        public string BannerUrl { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
