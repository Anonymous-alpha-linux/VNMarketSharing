using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace AdsMarketSharing.Entities
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Inventory { get; set; }
        public bool InPages { get; set; }
        public string Description { get; set; }
        private int _soldQuantity;
        public int SoldQuantity { 
            get
            {
                return _soldQuantity;
            }
            set
            {
                _soldQuantity = this.Orders.Sum(s => s.Amount);
            }
        }
        public bool HasAccepted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        [ForeignKey(nameof(UserPage))]
        public int UserPageId { get; set; }
        public UserPage UserPage { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
        public List<Attachment> Attachments { get; set; }
        public List<ProductClassify> ProductClassifies { get; set; }
        public List<Order> Orders { get; set; }
        public List<Review> Reviews { get; set; }
    }
}
