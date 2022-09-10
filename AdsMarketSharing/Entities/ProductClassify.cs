using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdsMarketSharing.Entities
{
    public class ProductClassify
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public List<ProductClassifyType> ProductClassifyTypes { get; set; }
    }
}
