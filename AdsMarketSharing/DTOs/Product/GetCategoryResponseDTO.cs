using System.Collections.Generic;

namespace AdsMarketSharing.DTOs.Product
{
    public class GetCategoryResponseDTO
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; } = 0;
        public int ParentId { get; set; }
        public int SubCategoryCount { get; set; } = 0;
        //public ICollection<GetCategoryResponseDTO> SubCategories { get; set; }
    }
}
