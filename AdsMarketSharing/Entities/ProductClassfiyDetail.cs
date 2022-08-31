namespace AdsMarketSharing.Entities
{
    public class ProductClassfiyDetail
    {
        public decimal Price { get; set; }
        public int Inventory { get; set; }
        public ProductClassifyType ClassifyTypeOne{ get; set; }
        public ProductClassifyType ClassifyTypeTwo { get; set; }
    }
}
