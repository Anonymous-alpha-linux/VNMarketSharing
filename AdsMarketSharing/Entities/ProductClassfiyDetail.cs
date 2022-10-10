using System.ComponentModel.DataAnnotations.Schema;

namespace AdsMarketSharing.Entities
{
    public class ProductClassfiyDetail
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int Inventory { get; set; }
        public Attachment PresentImage { get; set; } 

        [ForeignKey(nameof(ClassifyTypeKey))]
        public int ClassifyTypeKeyId { get; set; }
        public ProductClassifyType ClassifyTypeKey{ get; set; }

        [ForeignKey(nameof(ClassifyTypeValue))]
        public int ClassifyTypeValueId { get; set; }
        public ProductClassifyType ClassifyTypeValue { get; set; }
    }
}
