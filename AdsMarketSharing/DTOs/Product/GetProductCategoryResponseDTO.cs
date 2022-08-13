using AdsMarketSharing.Entities;

namespace AdsMarketSharing.DTOs.Product
{
    public class GetProductCategoryResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
    }
}
