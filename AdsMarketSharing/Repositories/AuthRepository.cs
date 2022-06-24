using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using AdsMarketSharing.Data;
using AdsMarketSharing.DTOs.Account;
using AdsMarketSharing.Interfaces;
using AdsMarketSharing.Models;
using AdsMarketSharing.Enum;
using AdsMarketSharing.Models.Auth;
using AutoMapper;

namespace AdsMarketSharing.Repositories
{
    public class AuthRepository: IAuthRepository
    {   
        private readonly IMapper _mapper;
        private readonly SQLExpressContext _context;
        private readonly IConfiguration _configuration;

        public AuthRepository(IMapper mapper, SQLExpressContext context,IConfiguration configuration)
        {
            _mapper = mapper;
            _context = context;
            _configuration = configuration;
        }
        public async Task<ServiceResponse<GetRegisterInfo>> Register(AddAccountInfoDTO requestAccount)
        {
            var response = new ServiceResponse<GetRegisterInfo>();
            try
            {
                // 1. Validate the existence of username or email within account
                if (await EmailExists(requestAccount.Email))
                {
                    throw new ServiceResponseException<ResponseStatus>(400, ResponseStatus.Failed, "User has been existed!");
                }
                // Hashing password before getting the register
                CreateHashedPassword(requestAccount.Password, out byte[] passwordHash, out byte[] passwordSalt);
                // 2. Search out role to assign
                Role foundRole = await _context.Roles.FirstOrDefaultAsync(role => role.Id == requestAccount.RoleId); 
                if(foundRole == null)
                {
                    throw new ServiceResponseException<ResponseStatus>(400,ResponseStatus.Failed,"Role didn't exist to assign");
                }
                // 3. Create new Account
                Account newAccount = _mapper.Map<Account>(requestAccount);
                newAccount.PasswordHash = passwordHash;
                newAccount.PasswordSalt = passwordSalt;

                // 4. Add the new account to db
                await _context.Accounts.AddAsync(newAccount);
                // 5. Save the current markup   
                await _context.SaveChangesAsync();

                // 6. Assign Role to Account
                int accountId = (await _context.Accounts.FirstOrDefaultAsync(account => account.Email == requestAccount.Email)).Id;
                var assignRoletoAccount = await AssignRole(new AssignRoleToAccountDTO()
                {
                    AccountId = accountId,
                    RoleId = foundRole.Id
                });

                // 7. Define the response
                response.Data = new GetRegisterInfo { 
                    AccountId = accountId,
                    Email = newAccount.Email,
                };
                response.Message = "Please check email to verify your email";
                response.ServerMessage = "Add new account successfully";
                response.Status = ResponseStatus.Successed;
                response.StatusCode = 201;
            }
            catch (ServiceResponseException<ResponseStatus> exception)
            {
                // 1. Define the error resposne 
                response.Data = null;
                response.Message = "Added failed!";
                response.ServerMessage = exception.Message;
                response.Status = exception.Value;
                response.StatusCode = exception.StatusCode;
            }
            return response;
        }
        public async Task<ServiceResponse<GetAccountInfoDTO>> Login(LoginAccount requestAccount)
        {
            var response = new ServiceResponse<GetAccountInfoDTO>();
            try
            {
                // 1. Validate imcomming request(included email/username);
                if(requestAccount.Email == null)
                {
                    throw new ServiceResponseException<ResponseStatus>(400,ResponseStatus.Failed,"Please fill the username or email");
                }
                // 2. Find account that corresspoding with request (included Email/Username);               
                var foundAccountRole = await _context.AccountRoles
                    .Include(ar => ar.Account)
                    .Include(ar => ar.Role)
                    .FirstOrDefaultAsync(ar => ar.Account.Email == requestAccount.Email);

                if(foundAccountRole == null)
                {
                    throw new ServiceResponseException<ResponseStatus>(400,ResponseStatus.Failed,"Your account doesn't currently exist. Please register to use");
                }
                // 3. Verify password;
                if(!VerifyPassword(foundAccountRole.Account, requestAccount.Password))
                {
                    throw new ServiceResponseException<ResponseStatus>(400,ResponseStatus.Failed,"Your password is incorrectly");
                }
                // 4. Verify "IsActive" account;
                if (!foundAccountRole.Account.IsActive)
                {
                    throw new ServiceResponseException<ResponseStatus>(403, ResponseStatus.NoActivatedAccount, foundAccountRole.AccountId, "Your account are not activated");
                }
                // 5. Define the success response; 
                // Mainly release the last oupt of response is token
                response.Data = new GetAccountInfoDTO 
                { 
                    AccountId = foundAccountRole.AccountId,
                    Email = foundAccountRole.Account.Email,
                    AccessToken = CreateToken(foundAccountRole.AccountId, foundAccountRole.Account.Email, foundAccountRole.Role.Name),
                    Password = requestAccount.Password,
                    Roles = new List<GetRoleDTO>() { 
                        new GetRoleDTO{ RoleName = foundAccountRole.Role.Name} 
                    }
                };
                response.Message = "Login successfully";
                response.ServerMessage = "Successful Accessed";
                response.Status = ResponseStatus.Successed;
                response.StatusCode = 200;
            }
            catch (ServiceResponseException<ResponseStatus> e)
            {
                // 1. Define the failed response;
                response.Data = e.Value != ResponseStatus.NoActivatedAccount ? null : new GetAccountInfoDTO()
                {
                    AccountId = (int)e.AdditionalValue
                }; 
                response.ServerMessage = e.Message;
                response.Message = e.Message;
                response.Status = e.Value;
                response.StatusCode = e.StatusCode;
            }

            return response;
        }
        public async Task<ServiceResponse<GetRoleDTO>> AssignRole(AssignRoleToAccountDTO requestAccount)
        {
            var response = new ServiceResponse<GetRoleDTO>();
            try
            {
                // 1. Validate request
                if(requestAccount == null)
                {
                    throw new Exception("Your request is empty now!");
                }


                // 2. Search out the account
                Account account = await _context.Accounts
                    .Include(acc => acc.AccountRoles).ThenInclude(accountRole => accountRole.Role)
                    .FirstOrDefaultAsync(acc => acc.Id == requestAccount.AccountId);
                if(account== null)
                {
                    throw new Exception("Your account wasn't found!");
                }
                // 3. Search out the role
                Role role = await _context.Roles.FirstOrDefaultAsync(role => role.Id == requestAccount.RoleId);
                if (role == null)
                {
                    throw new Exception("This role wasn't included!");
                }
                // 4. Create an "AccountRole" instance
                AccountRole accountRole = new AccountRole()
                {
                    Account = account,
                    Role = role,
                };

                await _context.AccountRoles.AddAsync(accountRole);
                await _context.SaveChangesAsync();

                // 3. Define the response
                response.Data = new GetRoleDTO
                {
                     RoleName = accountRole.Role.Name
                };
                response.ServerMessage = "Assigned successfully";
                response.Status = ResponseStatus.Successed;
                response.Message = "Assigned successfully";

            }
            catch (Exception e)
            {
                // 1. Define the error resposne 
                response.Data = null;
                response.ServerMessage = e.Message;
                response.Status = ResponseStatus.Failed;
                response.Message = "Assigned failed!";
            }

            return response;
        }
        public async Task<ServiceResponse<bool>> ConfirmEmail(int accountId)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(acc => acc.Id == accountId);
                if(account == null)
                {
                    throw new Exception("Cannot find yout accountId");
                }
                account.IsActive = true;
                await _context.SaveChangesAsync();

                response.Data = true;
                response.ServerMessage = accountId + " has been confirmed";
                response.Message = "Account activated successfully";
                response.Status = ResponseStatus.Successed;
            }
            catch (Exception e)
            {     
                response.Data = false;
                response.ServerMessage = e.Message;
                response.Message = "Account activated failed!";
                response.Status = ResponseStatus.Failed;
            }
            return response;
        }
        public async Task<bool> EmailExists(string email)
        {
            return await _context.Accounts.AnyAsync(account => account.Email.ToLower() == email.ToLower());
        }














        // Authentication
        private void CreateHashedPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac= new System.Security.Cryptography.HMACSHA512()){
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPassword(Account account,string password) 
        { 
            using (var hmac =new System.Security.Cryptography.HMACSHA512(account.PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != account.PasswordHash[i])
                    {
                        return false;
                    }
                }
                return true;
            }  
        }
        private async Task GenerateEmailConfirmation(GetAccountInfoDTO account) {
            /*var confirmationlink = "https://localhost:44379/api/Account/ConfirmEmailLink?token=" + token + "&email=" + user.Email;*/

        }
     
        
        // Application token
        private string CreateToken(int accountId, string email, string role)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, accountId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value)
            );
                
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddSeconds(30),
                SigningCredentials = credentials
            };


            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
