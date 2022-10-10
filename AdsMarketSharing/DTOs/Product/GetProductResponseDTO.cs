using System;
using System.Collections.Generic;
using AdsMarketSharing.DTOs.UserPage;

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
        public GetUserPageResponseDTO UserPage { get; set; }
        public bool HasAccepted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public List<GetCategoryResponseDTO> ProductCategories { get; set; }
        public List<GetProductClassifyResponseDTO> ProductClassifies { get; set; }
        public List<GetProductClassifyDetailResponseDTO> ProductDetails { get; set; }
        public List<string> Urls { get; set; }
        public int ReviewAmount { get; set; } = 0;
    }
}
