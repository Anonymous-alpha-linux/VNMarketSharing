using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace AdsMarketSharing.Entities
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        public int Inventory { get; set; }
        public bool InPages { get; set; }
        public string Description { get; set; }
        public int SoldQuantity { get; set; }
        [ForeignKey(nameof(UserPage))]
        public int UserPageId { get; set; }
        public UserPage UserPages { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
        public List<Attachment> Attachment { get; set; }
    }
}
