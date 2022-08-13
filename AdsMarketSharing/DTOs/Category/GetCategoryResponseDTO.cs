namespace AdsMarketSharing.DTOs.Category
{
    public class GetCategoryResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
