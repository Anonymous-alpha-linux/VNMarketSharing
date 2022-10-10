namespace AdsMarketSharing.DTOs.User
{
    public class GetUserWithoutBiographyDTO
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public string? Avatar { get; set; } = null;
    }
}
