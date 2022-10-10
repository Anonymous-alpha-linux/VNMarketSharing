using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdsMarketSharing.Entities
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public int Rate { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Reply> Replies { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
