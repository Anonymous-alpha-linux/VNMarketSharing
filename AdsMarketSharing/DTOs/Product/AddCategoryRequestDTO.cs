using AdsMarketSharing.Entities;
using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.DTOs.Product
{
    public class AddCategoryRequestDTO
    {
        [Required]
        public string Name { get; set; }
        public int Level { get; set; } = 0;
        public int? ParentCategoryId { get; set; }
    }
}
