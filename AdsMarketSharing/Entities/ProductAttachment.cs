namespace AdsMarketSharing.Entities
{
    public class ProductAttachment
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int AttachmentId { get; set; }
        public Attachment Attachment { get; set; }
    }
}
