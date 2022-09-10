using System.Collections.Generic;

namespace AdsMarketSharing.DTOs.Product
{
    public class AddProductClassifyRequestDTO
    {
        public string Name { get; set; }
        public string[] ClassifyTypes { get; set; }
    }
}
