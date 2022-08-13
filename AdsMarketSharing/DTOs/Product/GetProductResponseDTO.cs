using AdsMarketSharing.Entities;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AdsMarketSharing.DTOs.Product
{
    public class GetProductResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Inventory { get; set; }
        public bool InPages { get; set; }
        public string Description { get; set; }
        public int SoldQuantity { get; set; }
        public int UserPageId { get; set; }
        public List<GetCategoryResponseDTO> ProductCategories { get; set; }
        public List<string> Urls { get; set; }
    }
}
