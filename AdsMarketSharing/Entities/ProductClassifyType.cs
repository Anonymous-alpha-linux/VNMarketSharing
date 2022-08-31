namespace AdsMarketSharing.Entities
{
    public class ProductClassifyType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductClassifyId { get; set; }
        public ProductClassify ProductClassify { get; set; }
    }
}
