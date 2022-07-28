using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.DTOs.File
{
    public class AttachmentResponseDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string FilePath { get; set; }
        [Required]
        public string PublicPath { get; set; }
        [Required]
        public string FileType { get; set; }
        [Required]
        public string FileTag { get; set; }
        [Required]
        public float FileSize { get; set; }
    }
}
