using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AdsMarketSharing.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int? ParentCategoryId { get; set; }
        [ForeignKey("ParentCategoryId")]
        [JsonIgnore]
        public Category ParentCategory { get; set; }
        [JsonIgnore]
        public List<Category> SubCategories { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
    }
}
