using System.Collections.Generic;

namespace AdsMarketSharing.Entities
{
    public class ProductClassifyType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductClassifyId { get; set; }
        public virtual ProductClassify ProductClassify { get; set; }
        public virtual List<ProductClassfiyDetail> ProductClassifyKeys { get; set; }
        public virtual List<ProductClassfiyDetail> ProductClassifyValues { get; set; }
    }
}
