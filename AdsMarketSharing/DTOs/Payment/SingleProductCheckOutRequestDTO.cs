using AdsMarketSharing.Entities;
using System.Collections.Generic;

namespace AdsMarketSharing.DTOs.Payment
{
    public class SingleProductCheckOutRequestDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public ReceiverAddress Address { get; set; }
    }
}
