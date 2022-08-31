using System.Collections.Generic;

namespace AdsMarketSharing.Entities
{
    public class ProductClassify
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public List<ProductClassifyType> ProductClassifyTypes { get; set; }
    }
}
