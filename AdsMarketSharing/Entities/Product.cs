using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdsMarketSharing.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Inventory { get; set; }
        public int SoldQuantity { get; set; }
        public User User { get; set; }
        [ForeignKey(nameof(User))]
        public int SellerId { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
    }
}
