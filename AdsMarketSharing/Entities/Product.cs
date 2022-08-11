using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdsMarketSharing.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Inventory { get; set; }
        public bool InPages { get; set; }
        public int SoldQuantity { get; set; }
        public string Description { get; set; }
        [ForeignKey("user")]
        public int SellerId { get; set; }
        public User User { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
        public List<Attachment> Attachment { get; set; }
    }
}
