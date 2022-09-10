using AdsMarketSharing.DTOs.File;
using AdsMarketSharing.Validations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AdsMarketSharing.DTOs.Product
{
    public class AddProductRequestDTO
    {
        //[MaxLength(80)]
        public string Name { get; set; }
        public decimal Price { get; set; }
        //[Range(1, int.MaxValue)] 
        public int Inventory { get; set; }
        public bool InPages { get; set; }
        //[MinLength(240)]
        public string Description { get; set; }
        public int UserPageId { get; set; } = 5;
        public List<int> CategoryIds { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [FileSizeValidator(5)]
        [ContentTypeValidator(ContentTypeGroup.Image)]
        public List<IFormFile> Files { get; set; }
        [JsonIgnore]
        public List<AttachmentResponseDTO> Attachments { get; set; }
        public List<AddProductClassifyRequestDTO> ProductClassifies { get; set; }
        public List<AddProductClassifyDetailRequestDTO> ProductDetails { get; set; }
    }
}
