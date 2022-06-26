using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using AdsMarketSharing.Data;
using AdsMarketSharing.DTOs.Account;
using AdsMarketSharing.Interfaces;
using AdsMarketSharing.Models;
using AdsMarketSharing.Models.Token;
using AdsMarketSharing.Enum;
using AdsMarketSharing.Models.Auth;
using AdsMarketSharing.DTOs.Token;
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

                // 5. Defne the refresh token; 
                // Mainly release the last oupt of response is token
                var tokenResponse = await GenerateJWTToken(foundAccountRole, DateTime.UtcNow.AddMinutes(1),DateTime.UtcNow.AddDays(1));

                // 6. Define the response
                response.Data = new GetAccountInfoDTO
                {
                    AccountId = foundAccountRole.AccountId,
                    Email = foundAccountRole.Account.Email,
                    AccessToken = tokenResponse.Data.JWTToken,
                    RefreshToken = tokenResponse.Data.RefreshToken,
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
        public async Task<ServiceResponse<AuthTokenResponse>> RefreshToken(AuthTokenRequest tokenRequest)
        {
            var response = new ServiceResponse<AuthTokenResponse>();
            try
            {            
                var isValidTokenResponse = await ValidateJWTToken(tokenRequest);
                if (!isValidTokenResponse.Status.Equals(ResponseStatus.Successed))
                {
                    throw new ServiceResponseException<ResponseStatus>(isValidTokenResponse.StatusCode, isValidTokenResponse.Status, isValidTokenResponse.ServerMessage);
                }

                // Generate new token
                int accountId = int.Parse(GetClaimValue(tokenRequest.Token, "nameid"));
                var dbAccount = await _context.AccountRoles
                    .Include(a => a.Account)
                    .Include(a => a.Role)
                    .FirstOrDefaultAsync(a => a.AccountId == accountId);

                // Define the response properties
                response = await GenerateJWTToken(dbAccount, DateTime.UtcNow.AddMinutes(1), DateTime.UtcNow.AddDays(1));   
            }
            catch (ServiceResponseException<ResponseStatus> e)
            {
                // Define the response properties
                response.Data = null;
                response.Status = e.Value;
                response.StatusCode = e.StatusCode;
                response.Message = "The token cannot resolve";
                response.ServerMessage = e.Message;
            }
            return response;
        }
        public Task<ServiceResponse<string>> RefreshToken(string token)
        {
            throw new NotImplementedException();
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
        
        // Application token
        private async Task<ServiceResponse<AuthTokenResponse>> GenerateJWTToken(AccountRole identityAccount,DateTime jwtExpiryTime, DateTime refreshjwtExpiryTime)
        {
            var response = new ServiceResponse<AuthTokenResponse>();
            try
            {
                // 1. Setting up the token description
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, identityAccount.Account.Id.ToString()),
                    new Claim(ClaimTypes.Email, identityAccount.Account.Email),
                    new Claim(ClaimTypes.Role, identityAccount.Role.Name),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                SymmetricSecurityKey key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value)
                );
                SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
                SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = jwtExpiryTime,
                    SigningCredentials = credentials
                };
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

                // 2. Generate new refreshtoken
                RefreshToken newRefreshToken = new RefreshToken()
                {
                    Token = RandomString(13) + Guid.NewGuid(),
                    JwtId = token.Id,
                    AccountId = identityAccount.AccountId,
                    CreatedTime = DateTime.UtcNow,
                    ExpireTime = refreshjwtExpiryTime,
                    IsUsed = false,
                    IsRevoked = false
                };
         
                await _context.RefreshTokens.AddAsync(newRefreshToken);
                await _context.SaveChangesAsync();

                // 3. Define the response properties
                response.Data = new AuthTokenResponse()
                {
                    Token = token,
                    RefreshToken = newRefreshToken.Token,
                    JWTToken = tokenHandler.WriteToken(token)
                };
                response.Status = ResponseStatus.Successed;
                response.StatusCode = 200;
                response.Message = "Generated new token successfully";
                response.ServerMessage = "Completed service";
            }
            catch (Exception e)
            {
                response.Data = null;
                response.Status = ResponseStatus.Failed;
                response.StatusCode = 500;
                response.Message = "Failed to generate new authorization token";
                response.ServerMessage = e.Message;
            }

            return response;
        }
        private async Task<ServiceResponse<bool>> ValidateJWTToken(AuthTokenRequest authTokenRequest)
        {
            var response = new ServiceResponse<bool>();
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;
            try
            {
                // Validate 1 - Validate JWT token format
                var principal = tokenHandler.ValidateToken(authTokenRequest.Token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    // Clock skew compensates for server time drift.
                    // We recommend 5 minutes or less:
                    ClockSkew = TimeSpan.Zero,
                    // Ensure the token hasn't expired:
                    RequireExpirationTime = true,
                    ValidateLifetime = false,
                    // Ensure the token audience matches our audience value (default true):
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    // Specify the key used to sign the token:
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Token").Value)),
                    RequireSignedTokens = true
                }, out validatedToken);

                // Validate 2 - Validate encryption alg
                if(validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                    if (result == false) throw new ServiceResponseException<ResponseStatus>(403, ResponseStatus.NotAcceptableToken,"Your token is invalid");
                }

                // Validate 3 - Validate expiry time
                var utcExpiryTime = long.Parse(principal.Claims.FirstOrDefault(x=> x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expiryDate = UnixTimeStampToDateTime(utcExpiryTime);
                if(expiryDate > DateTime.UtcNow)
                {
                    throw new ServiceResponseException<ResponseStatus>(403, ResponseStatus.NotAcceptableToken, "Your token has not yet expired");
                }

                // Validate 4 - Validate the existence of token
                var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == authTokenRequest.RefreshToken);
                if(storedToken == null)
                {
                    throw new ServiceResponseException<ResponseStatus>(403, ResponseStatus.NotAcceptableToken, "Refresh token isn't existed");
                }

                // Validate 5 - Validate if it's used
                if (storedToken.IsUsed)
                {
                    throw new ServiceResponseException<ResponseStatus>(400, ResponseStatus.NotAcceptableToken, "Refresh token cannot be used");
                }

                // Validate 6 - Validate if revoked
                if (storedToken.IsRevoked)
                {
                    throw new ServiceResponseException<ResponseStatus>(400, ResponseStatus.NotAcceptableToken, "Token has been revoked");
                }

                // Validate 7 - Validate the id
                if(storedToken.JwtId != principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value)
                {
                    throw new ServiceResponseException<ResponseStatus>(400, ResponseStatus.NotAcceptableToken, "Token doesn't match");
                }

                // Validate 8 - Validate stored token expiry time
                if(storedToken.ExpireTime < DateTime.UtcNow)
                {
                    throw new ServiceResponseException<ResponseStatus>(400, ResponseStatus.NotAcceptableToken, "Refresh token has expired");
                }

                // update the token
                storedToken.IsUsed = true;
                _context.RefreshTokens.Update(storedToken);
                await _context.SaveChangesAsync();

                // Define the response
                response.Data = true;
                response.Message = "Token is valid";
                response.ServerMessage = "Completed service";
                response.Status = ResponseStatus.Successed;
                response.StatusCode = 200;
            }
            catch (ServiceResponseException<ResponseStatus> e)
            {
                response.Data = false;
                response.Message = "The token is detected, is incorrect";
                response.ServerMessage = e.Message;
                response.Status = ResponseStatus.NotAcceptableToken;
                response.StatusCode = e.StatusCode != null ? e.StatusCode : 500;
            }
            return response;
        }
        private string GetClaimValue(string token, string claimType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.ReadJwtToken(token) as JwtSecurityToken;
            string stringClaimValue = jwtSecurityToken.Claims.First(claim => claim.Type == claimType).Value;

            return stringClaimValue;
        }
        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();

            return dateTimeVal;
        }
        private string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(x => x[random.Next(x.Length)]).ToArray());
        }

    }
}
