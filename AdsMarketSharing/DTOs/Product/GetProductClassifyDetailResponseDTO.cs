namespace AdsMarketSharing.DTOs.Product
{
    public class GetProductClassifyDetailResponseDTO
    {
        public int ProductClassifyKeyId { get; set; }
        public string ProductClassifyKey { get; set; }
        public int ProductClassifyValueId { get; set; }
        public string ProductClassifyValue { get; set; }
        public decimal Price { get; set; }
        public int Inventory { get; set; }
    }
}
