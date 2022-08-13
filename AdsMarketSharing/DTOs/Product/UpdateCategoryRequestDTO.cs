namespace AdsMarketSharing.DTOs.Product
{
    public class UpdateCategoryRequestDTO
    {
        public string Name { get; set; }
        public string Level { get; set; }
        public int? CategoryId { get; set; }
    }
}
