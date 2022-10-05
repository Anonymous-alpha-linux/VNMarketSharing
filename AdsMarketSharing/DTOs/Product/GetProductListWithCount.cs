using System.Collections.Generic;

namespace AdsMarketSharing.DTOs.Product
{
    public class GetProductListWithCount
    {
        public List<GetProductResponseDTO> ProductList { get; set; }
        public int Amount { get; set; }
    }
}
