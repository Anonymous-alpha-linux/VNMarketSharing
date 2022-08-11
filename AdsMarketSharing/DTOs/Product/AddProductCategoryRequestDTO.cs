using AdsMarketSharing.Entities;
using System.Collections.Generic;

namespace AdsMarketSharing.DTOs.Product
{
    public class AddProductCategoryRequestDTO
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int Inventory { get; set; }
        public int SoldQuantity { get; set; }
        public int SellerId { get; set; }
        public string Description { get; set; }
        public List<int> CategoryIds { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
        public List<Attachment> Attachment { get; set; }
    }
}
