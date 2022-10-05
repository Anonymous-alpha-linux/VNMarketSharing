namespace AdsMarketSharing.DTOs.Review
{
    public class ReviewProductResponseDTO
    {
        public int Id { get; set; }
        public int Rate { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Comment { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
    }
}
