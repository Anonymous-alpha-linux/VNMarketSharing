using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdsMarketSharing.Entities
{
    public class ProductClassifyType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductClassifyId { get; set; }
        public virtual ProductClassify ProductClassify { get; set; }
        //[InverseProperty("ClassifyTypeKey")]
        public virtual List<ProductClassfiyDetail> ProductClassifyKeys { get; set; }
        //[InverseProperty("ClassifyTypeValue")]
        public virtual List<ProductClassfiyDetail> ProductClassifyValues { get; set; }
    }
}
