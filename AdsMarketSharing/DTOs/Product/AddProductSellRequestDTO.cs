using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.DTOs.Product
{
    public class AddProductSellRequestDTO
    {
        public decimal Price { get; set; }
        [Range(1, int.MaxValue)]
        public int Inventory { get; set; }
    }
}
