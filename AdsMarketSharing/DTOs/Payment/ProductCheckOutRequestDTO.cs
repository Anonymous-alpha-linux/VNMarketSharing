using AdsMarketSharing.Enum;
using System;
using System.Collections.Generic;

namespace AdsMarketSharing.DTOs.Payment
{
    public class ProductCheckOutRequestDTO
    {
        public List<OrderCreationDTO> Orders{ get; set; }
    }
}
