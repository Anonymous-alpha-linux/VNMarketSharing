using AdsMarketSharing.DTOs.File;

namespace AdsMarketSharing.DTOs.Product
{
    public class AddProductAttachmentRequestDTO
    {
        public AddProductRequestDTO Product { get; set; }
        public AttachmentResponseDTO Attachment { get; set; }
    }
}
