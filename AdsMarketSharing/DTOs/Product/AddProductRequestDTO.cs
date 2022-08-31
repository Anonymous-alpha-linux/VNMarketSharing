using AdsMarketSharing.DTOs.File;
using AdsMarketSharing.Validations;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AdsMarketSharing.DTOs.Product
{
    public class AddProductRequestDTO
    {
        [MaxLength(50)]
        public string Name { get; set; }
        public decimal Price { get; set; }
        [Range(1, int.MaxValue)]
        public int Inventory { get; set; }
        public bool InPages { get; set; } = true;
        [MinLength(100)]
        public string Description { get; set; }
        public int UserPageId { get; set; }
        public List<int> CategoryIds { get; set; }
        [FileSizeValidator(5)]
        [ContentTypeValidator(ContentTypeGroup.Image)]
        public List<IFormFile> Files { get; set; }
        [JsonIgnore]
        public List<AttachmentResponseDTO> Attachments { get; set; }
        public List<AddProductClassifyRequestDTO> Classifies { get; set; }
    }
}
