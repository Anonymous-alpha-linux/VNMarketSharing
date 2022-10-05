namespace AdsMarketSharing.DTOs.Review
{
    public class ReviewProductCreationDTO
    {
        public int Rate { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Comment { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
    }
}
