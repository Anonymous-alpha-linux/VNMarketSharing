namespace AdsMarketSharing.DTOs.Product
{
    public class UpdateCategoryRequestDTO
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
