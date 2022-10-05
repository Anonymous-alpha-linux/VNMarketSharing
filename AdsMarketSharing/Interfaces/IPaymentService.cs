using AdsMarketSharing.Models;
using System.Threading.Tasks;
using AdsMarketSharing.Entities;
using AdsMarketSharing.DTOs.Payment;

namespace AdsMarketSharing.Interfaces
{
    public interface IPaymentService
    {
        Task<ServiceResponse<string>> Checkout(Invoice orderInformation, string returnUrl);
        Task<ServiceResponse<PaymentConfirmationDTO>> Confirm();
    }
}
