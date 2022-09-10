﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdsMarketSharing.Entities
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Inventory { get; set; }
        public bool InPages { get; set; }
        public string Description { get; set; }
        public int SoldQuantity { get; set; }
        public bool HasAccepted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey(nameof(UserPage))]
        public int UserPageId { get; set; }
        public UserPage UserPage { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
        public List<Attachment> Attachments { get; set; }
        public List<ProductClassify> ProductClassifies { get; set; }
    }
}
