using System.Collections.Generic;

namespace AdsMarketSharing.DTOs.Product
{
    public class GetProductClassifyResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<GetProductClassifyTypeResponseDTO> ClassifyTypes { get; set; }
    }
}
