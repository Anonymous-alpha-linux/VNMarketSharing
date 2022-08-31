using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.DTOs.Product
{
    public class GetCategoryRequestDTO
    {
        public int Level { get; set; } = 0;
        public int? ParentId { get; set; }
        [Range(1, int.MaxValue)]
        public int? Page { get; set; }
        public int? Take { get; set; }
    }
}
