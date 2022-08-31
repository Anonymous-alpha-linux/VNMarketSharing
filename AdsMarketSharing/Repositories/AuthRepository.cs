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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using AdsMarketSharing.Data;
using AdsMarketSharing.DTOs.Account;
using AdsMarketSharing.Interfaces;
using AdsMarketSharing.Models;
using AdsMarketSharing.Enum;
using AdsMarketSharing.DTOs.Token;
using AutoMapper;
using AdsMarketSharing.Entities;

namespace AdsMarketSharing.Repositories
{
    public class AuthRepository: IAuthRepository
    {   
        private readonly IMapper _mapper;
        private readonly SQLExpressContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly CookieOptions _cookieOptions;
        public AuthRepository(IMapper mapper, SQLExpressContext context,IConfiguration configuration, IHttpContextAccessor httpContext)
        {
            _mapper = mapper;
            _context = context;
            _configuration = configuration;
            _httpContext = httpContext;
            _cookieOptions = new CookieOptions()
            {
                Expires = DateTime.UtcNow.AddDays(1),
                HttpOnly = false,
                Secure = true,
                IsEssential = true,
                Path = "/",
                Domain = _configuration.GetSection("AppSettings:CookieDomain").Value,
                SameSite = SameSiteMode.None,
            };
        }
        public async Task<ServiceResponse<GetRegisterInfo>> Register(RegisterNewAccountDTO requestAccount)
        {
            var response = new ServiceResponse<GetRegisterInfo>();
            try
            {
                // Condition 1. Validate the existence of username or email within account
                bool isExistedEmail = await EmailExists(requestAccount.Email);
                if (isExistedEmail)
                {
                    throw new ServiceResponseException<ResponseStatus>(400, ResponseStatus.Failed, "User has been existed!");
                }
                // Hashing password before getting the register
                CreateHashedPassword(requestAccount.Password, out byte[] passwordHash, out byte[] passwordSalt);
                // Condition 2. Search out role to assign
                Role foundRole = await _context.Roles.FirstOrDefaultAsync(role => role.Id == requestAccount.RoleId); 
                if(foundRole == null)
                {
                    throw new ServiceResponseException<ResponseStatus>(404,ResponseStatus.Failed,"Role didn't exist to assign");
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
        public async Task<ServiceResponse<GetAccountInfoDTO>> Login(LoginAccountDTO requestAccount)
        {
            var response = new ServiceResponse<GetAccountInfoDTO>();
            try
            {
                // Condition 1. Check if request had its token already
                bool hasExistedJWT = _httpContext.HttpContext.Request.Cookies["jwt"] != null;
                if (hasExistedJWT)
                {
                    throw new ServiceResponseException<ResponseStatus>(400, ResponseStatus.Failed, "Your have already logged in to system");
                }
                // 2. Find account that corresspoding with request (included Email/Username);               
                var foundAccountRole = _context.AccountRoles
                    .Include(ar => ar.Account)
                    .ThenInclude(acc => acc.User)
                    .Include(ar => ar.Role)
                    .Where(ar => ar.Account.Email == requestAccount.Email)
                    .Select(ar => new {
                        Account = ar.Account,
                        Email = ar.Account.Email,
                        Role = ar.Role.Name
                    })
                    .First();
                if(foundAccountRole == null)
                {
                    throw new ServiceResponseException<ResponseStatus>(404,ResponseStatus.Failed,"Your account doesn't currently exist. Please register to use");
                }

                // 3. Verify password;
                if(!VerifyPassword(foundAccountRole.Account, requestAccount.Password))
                {
                    throw new ServiceResponseException<ResponseStatus>(400,ResponseStatus.Failed,"Your password is incorrectly");
                }

                // 4. Verify "IsActive" account;
                if (!foundAccountRole.Account.IsActive)
                {
                    throw new ServiceResponseException<ResponseStatus>(403, ResponseStatus.NoActivatedAccount, foundAccountRole.Account.Id, "Your account are not activated");
                }

                // 5. Defne the refresh token; 
                // Mainly release the last oupt of response is token
                var tokenResponse = await GenerateJWTToken(foundAccountRole.Account,foundAccountRole.Role, DateTime.UtcNow.AddHours(10),DateTime.UtcNow.AddDays(1));
                if(tokenResponse.Status != ResponseStatus.Successed)
                {
                    throw new ServiceResponseException<ResponseStatus>(tokenResponse.StatusCode, tokenResponse.Status, tokenResponse.ServerMessage);
                }

                // 6. Settup cookie
                string cookieHost = _httpContext.HttpContext.Request.Host.Value;

                _httpContext.HttpContext.Response.Cookies.Append("jwt", tokenResponse.Data.JWTToken, _cookieOptions);
                _httpContext.HttpContext.Response.Cookies.Append("r_jwt", tokenResponse.Data.RefreshToken, _cookieOptions);

                // 7. Define the response
                response.Data = new GetAccountInfoDTO()
                {
                    AccountId = foundAccountRole.Account.Id,
                    Email = foundAccountRole.Account.Email,
                    AccessToken = tokenResponse.Data.JWTToken,
                    RefreshToken = tokenResponse.Data.RefreshToken,
                    Roles = new List<GetRoleDTO>() { 
                        new GetRoleDTO{ RoleName = foundAccountRole.Role} 
                    }
                };
                response.Message = "Login successfully";
                response.ServerMessage = "Successful accessed";
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
                response.Message = "Failed to join system now!";
                response.Status = e.Value;
                response.StatusCode = e.StatusCode;
            }

            return response;
        }
        public async Task<ServiceResponse<GetUserAccountDTO>> GetUser()
        {
      
            var response = new ServiceResponse<GetUserAccountDTO>();
            try
            {
                string token = _httpContext.HttpContext.Request.Cookies["jwt"];
                string refreshToken = _httpContext.HttpContext.Request.Cookies["r_jwt"];
                // Condition 1 - token doesn't exist
                if (token == null || refreshToken == null)
                {
                    throw new ServiceResponseException<ResponseStatus>(401, ResponseStatus.NotAcceptableToken, "You have not signed in yet");
                }
                // Condition 2 - validate jwt token properties
                var isValidTokenResponse = await ValidateJWTToken(new AuthTokenRequest()
                {
                    Token = token,
                    RefreshToken = refreshToken
                });

                if (!isValidTokenResponse.StatusCode.Equals(200) && !isValidTokenResponse.StatusCode.Equals(304))
                {
                    throw new ServiceResponseException<ResponseStatus>(isValidTokenResponse.StatusCode, isValidTokenResponse.Status, isValidTokenResponse.ServerMessage);
                }
               
                string emailClaim = GetClaimValue(token, JwtRegisteredClaimNames.Email);
                string accountIdClaim = GetClaimValue(token, JwtRegisteredClaimNames.NameId);
                string roleClaim = GetClaimValue(token, JwtRegisteredClaimNames.Amr);

                response.Data = new GetUserAccountDTO()
                {
                    Email = emailClaim,
                    AccountId = accountIdClaim,
                    Roles = new List<GetRoleDTO>()
                    {
                        new GetRoleDTO
                        {
                            RoleName = roleClaim
                        }
                    }
                };
                response.Status = ResponseStatus.Successed;
                response.StatusCode = 200;
                response.Message = "You have got account info successfully";
                response.ServerMessage = "Completed service";

            }
            catch (ServiceResponseException<ResponseStatus> e)
            {
                response.Data = null;
                response.Status = e.Value;
                response.StatusCode = e.StatusCode;
                response.Message = "Failed to get user";
                response.ServerMessage = e.Message;
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
                    Logout();
                    throw new ServiceResponseException<ResponseStatus>(isValidTokenResponse.StatusCode, isValidTokenResponse.Status, isValidTokenResponse.ServerMessage);
                }

                // update the token
                var _contextRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == tokenRequest.RefreshToken);
                _contextRefreshToken.IsUsed = true;
                _context.RefreshTokens.Update(_contextRefreshToken);
                await _context.SaveChangesAsync();

                // Generate new token
                int accountId = int.Parse(GetClaimValue(tokenRequest.Token, JwtRegisteredClaimNames.NameId));
                var dbAccount = await _context.AccountRoles
                    .Include(ar => ar.Account)
                    .Include(ar => ar.Role)
                    .Where(ar => ar.AccountId == accountId)
                    .Select(ar => new
                    {
                        Account = ar.Account,
                        Role = ar.Role.Name,
                    })
                    .FirstAsync();

                // Define the response properties
                response = await GenerateJWTToken(dbAccount.Account,dbAccount.Role, DateTime.UtcNow.AddHours(10), DateTime.UtcNow.AddDays(1));
                _httpContext.HttpContext.Response.Cookies.Append("jwt", response.Data.JWTToken, _cookieOptions);
                _httpContext.HttpContext.Response.Cookies.Append("r_jwt", response.Data.RefreshToken, _cookieOptions);
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
        public ServiceResponse<bool> Logout()
        {
            var response = new ServiceResponse<bool>();
            try
            {
                if (_httpContext.HttpContext.Request.Cookies["r_jwt"] == null && _httpContext.HttpContext.Request.Cookies["r_jwt"] == null)
                {
                    throw new ServiceResponseException<ResponseStatus>(404,ResponseStatus.NotAcceptableToken,"You are not signed in");
                }
                _httpContext.HttpContext.Response.Cookies.Delete("jwt",_cookieOptions);
                _httpContext.HttpContext.Response.Cookies.Delete("r_jwt", _cookieOptions);

                response.Data = true;
                response.Status = ResponseStatus.Successed;
                response.StatusCode = 200;
                response.Message = "You are signed out";
                response.ServerMessage = "Cookie has been reset";
            }
            catch (ServiceResponseException<ResponseStatus> e)
            {
                response.Data = false;
                response.Status = ResponseStatus.Failed;
                response.StatusCode = e.StatusCode;
                response.Message = "Failed to leave now";
                response.ServerMessage = e.Message;
            }
            return response;
        }
        public async Task<ServiceResponse<GetAccountInfoDTO>> ConfirmEmail(int accountId)
        {
            var response = new ServiceResponse<GetAccountInfoDTO>();

            try
            {
                var accountRole = await _context.AccountRoles.Include(acc => acc.Account)
                                                        .Include(acc => acc.Role)
                                                        .Where(acc => acc.AccountId == accountId)
                                                        .Select(acc => new
                                                        {
                                                            Account = acc.Account,
                                                            Role = acc.Role.Name
                                                        })
                                                        .FirstAsync();
                if (accountRole == null)
                {
                    throw new Exception("Cannot find your accountId");
                }
                accountRole.Account.IsActive = true;
                await _context.SaveChangesAsync();

                var tokenResponse = await GenerateJWTToken(accountRole.Account,accountRole.Role, DateTime.UtcNow.AddHours(10), DateTime.UtcNow.AddDays(1));
                if (!tokenResponse.Status.Equals(ResponseStatus.Successed))
                {
                    throw new ServiceResponseException<ResponseStatus>(tokenResponse.StatusCode,tokenResponse.Status,tokenResponse.Message);
                }

                _httpContext.HttpContext.Response.Cookies.Append("jwt", tokenResponse.Data.JWTToken, _cookieOptions);
                _httpContext.HttpContext.Response.Cookies.Append("r_jwt", tokenResponse.Data.RefreshToken, _cookieOptions);

                response.Data = new GetAccountInfoDTO()
                {
                    AccessToken = tokenResponse.Data.JWTToken,
                    RefreshToken = tokenResponse.Data.RefreshToken,
                    AccountId = accountId,
                    Email = accountRole.Account.Email,
                    Roles = new List<GetRoleDTO> { new GetRoleDTO() { RoleName= accountRole.Role } }
                };
                response.ServerMessage = accountRole.Account.Email + " has been confirmed";
                response.Message = "Account activated successfully";
                response.Status = ResponseStatus.Successed;
            }
            catch (ServiceResponseException<ResponseStatus> e)
            {
                response.Data = null;
                response.ServerMessage = e.Message;
                response.Message = "Account activated failed!";
                response.Status = ResponseStatus.Failed;
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
        public async Task<ServiceResponse<bool>> ChangePassword(ChangePasswordDTO requestAccount)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(acc => acc.Email == requestAccount.Email);
            var response = new ServiceResponse<bool>();

            try
            {
                if(account == null)
                {
                    throw new ServiceResponseException<ResponseStatus>(400, ResponseStatus.Failed, "Cannot find your account, please make sure your email is correctly");
                }
                CreateHashedPassword(requestAccount.Password, out byte[] passwordHash, out byte[] passwordSalt);
                account.PasswordHash = passwordHash;
                account.PasswordSalt = passwordSalt;

                await _context.SaveChangesAsync();

                response.Data = true;
                response.Status = ResponseStatus.Successed;
                response.StatusCode = 200;
                response.Message = "Changed password successfully";
                response.ServerMessage = "Completed service";
            }
            catch (ServiceResponseException<ResponseStatus> exception)
            {
                response.Data = true;
                response.Status = exception.Value;
                response.StatusCode = exception.StatusCode;
                response.Message = "";
                response.ServerMessage = exception.Message;
            }

            return response;
        }
        public async Task<bool> EmailExists(string email)
        {
            return await _context.Accounts.AnyAsync(account => account.Email.ToLower() == email.ToLower());
        }
        public async Task<ServiceResponse<GetUserAccountDTO>> GetUserByEmail(string email)
        {
            var response = new ServiceResponse<GetUserAccountDTO>();
            try
            {
                var account = await _context.Accounts.Select(acc => new
                {
                    Email = acc.Email,
                    AccountId = acc.Id
                }).FirstOrDefaultAsync(acc => acc.Email == email);

                if(account == null)
                {
                    throw new ServiceResponseException<ResponseStatus>(400, ResponseStatus.Failed, "Your email doesn't exist");
                }
                response.Data = new GetUserAccountDTO()
                {
                    AccountId = account.AccountId.ToString(),
                    Email = account.Email
                };
                response.Status = ResponseStatus.Successed;
                response.StatusCode = 200;
                response.Message = "";
                response.ServerMessage = "";
            }
            catch (ServiceResponseException<ResponseStatus> exception)
            {
                response.Data = null;
                response.Message = "Failed to get user by email";
                response.ServerMessage = exception.Message;
                response.Status = exception.Value;
                response.StatusCode = exception.StatusCode;
            }
            return response;        
        }

        // Authorization Helpers
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
        private async Task<ServiceResponse<AuthTokenResponse>> GenerateJWTToken(Account identityAccount,string roleName,DateTime jwtExpiryTime, DateTime refreshjwtExpiryTime)
        {
            var response = new ServiceResponse<AuthTokenResponse>();
            try
            {
                // 1. Setting up the token description
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(JwtRegisteredClaimNames.NameId, identityAccount.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, identityAccount.Email),
                    new Claim(JwtRegisteredClaimNames.Amr, roleName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                string internalKey = _configuration.GetSection("AppSettings:Token").Value;
                SymmetricSecurityKey key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(internalKey)
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
                    AccountId = identityAccount.Id,
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
                var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == authTokenRequest.RefreshToken);

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
                    throw new ServiceResponseException<ResponseStatus>(304, ResponseStatus.NonExpiredAccessToken, "Your token has not yet expired");
                }

                // Validate 4 - Validate the existence of token
                if(storedToken == null)
                {
                    throw new ServiceResponseException<ResponseStatus>(403, ResponseStatus.NotAcceptableToken, "Refresh token isn't existed");
                }

                // Validate 5 - Validate if it's used
                if (storedToken.IsUsed)
                {
                    throw new ServiceResponseException<ResponseStatus>(400, ResponseStatus.NotAcceptableToken, "Refresh token cannot be used anymore");
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
            string stringClaimValue;
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = tokenHandler.ReadJwtToken(token) as JwtSecurityToken;
                stringClaimValue = jwtSecurityToken.Claims.First(claim => claim.Type == claimType).Value;
            }
            catch (Exception e)
            {
                throw e;
            }

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
