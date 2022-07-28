using AdsMarketSharing.DTOs.Account;
using AdsMarketSharing.DTOs.Token;
using AdsMarketSharing.Models;
using System.Threading.Tasks;

namespace AdsMarketSharing.Interfaces
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<GetAccountInfoDTO>> Login(LoginAccountDTO requestAccount);
        ServiceResponse<bool> Logout();
        Task<ServiceResponse<GetRegisterInfo>> Register(RegisterNewAccountDTO requestAccount);
        Task<ServiceResponse<GetUserAccountDTO>> GetUser();
        Task<ServiceResponse<GetUserAccountDTO>> GetUserByEmail(string email);
        Task<ServiceResponse<GetRoleDTO>> AssignRole(AssignRoleToAccountDTO requestRoleAccount);
        Task<ServiceResponse<GetAccountInfoDTO>> ConfirmEmail(int accountId);
        Task<ServiceResponse<AuthTokenResponse>> RefreshToken(AuthTokenRequest authTokenRequest);
        Task<ServiceResponse<bool>> ChangePassword(ChangePasswordDTO request);
        Task<bool> EmailExists(string email);
    }

}
