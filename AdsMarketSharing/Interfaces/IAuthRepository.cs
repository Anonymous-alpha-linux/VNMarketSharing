using AdsMarketSharing.DTOs.Account;
using AdsMarketSharing.Models;
using System.Threading.Tasks;

namespace AdsMarketSharing.Interfaces
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<GetAccountInfoDTO>> Login(LoginAccount requestAccount);
        Task<ServiceResponse<GetRegisterInfo>> Register(AddAccountInfoDTO requestAccount);
        Task<ServiceResponse<GetRoleDTO>> AssignRole(AssignRoleToAccountDTO requestRoleAccount);
        Task<ServiceResponse<bool>> ConfirmEmail(int accountId);
        Task<bool> EmailExists(string email);
    }

}
