using System.Collections.Generic;

namespace AdsMarketSharing.Entities
{
    public class ProductClassify
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SoldQuantity { get; set; }
        public List<string> ItemType { get; set; }
    }
}
