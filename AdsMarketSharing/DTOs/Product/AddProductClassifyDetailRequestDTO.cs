using System;
using System.Collections.Generic;
using AdsMarketSharing.Entities;
using AdsMarketSharing.DTOs.File;
using Microsoft.AspNetCore.Http;

namespace AdsMarketSharing.DTOs.Product
{
    public class AddProductClassifyDetailRequestDTO
    {
        public decimal Price { get; set; }
        public int Inventory { get; set; }
        public int[] ClassifyIndexes { get; set; }
        public ProductClassifyType ClassifyTypeKey { get; set; }
        public ProductClassifyType ClassifyTypeValue { get; set; }
        public IFormFile Image { get; set; }
        public AttachmentResponseDTO PresentImage { get; set; }
    }
}
