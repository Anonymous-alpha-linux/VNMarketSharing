using System.Collections.Generic;
using System.Linq;
using AdsMarketSharing.Enum;

namespace AdsMarketSharing.DTOs.Payment
{
    public class InvoiceCreationDTO
    {
        private decimal _cashAmount { get; set; }
        public decimal CashAmount { 
            get => _cashAmount; 
            set {
                _cashAmount = this.Orders.Sum(o => o.Total);
            }
        }
        public int? PaymentId { get; set; }
        public ShippingMethod Shipping { get; set; }
        public PaymentCreationDTO? Payment { get; set; }
        public List<OrderCreationDTO> Orders { get; set; }
        public int UserId { get; set; }
    }
}
