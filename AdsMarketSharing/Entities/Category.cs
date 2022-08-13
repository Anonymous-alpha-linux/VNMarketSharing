using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AdsMarketSharing.Entities
{
    public class Category
    {
        public Category()
        {
            SubCategories = new List<Category>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int? ParentCategoryId { get; set; }
        [ForeignKey(nameof(ParentCategoryId))]
        public virtual Category ParentCategory { get; set; }
        public virtual ICollection<Category> SubCategories { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
    }
}
