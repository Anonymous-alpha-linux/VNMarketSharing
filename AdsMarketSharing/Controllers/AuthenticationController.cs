using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;

using AdsMarketSharing.Enum;
using AdsMarketSharing.Models;
using AdsMarketSharing.DTOs.Account;
using AdsMarketSharing.Interfaces;
using AdsMarketSharing.Models.Email;
using AdsMarketSharing.Models.Token;
using AdsMarketSharing.DTOs.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cors;

namespace AdsMarketSharing.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [EnableCors(PolicyName = "AllowAPIRequestIO")]
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthRepository _authenticationService;
        private readonly IMailService _mailService;
        private readonly IToken _tokenService;

        public AuthenticationController(IAuthRepository authenticationService, IMailService mailService, IToken tokenService, IConfiguration configuration)
        {
            _configuration = configuration;
            _authenticationService = authenticationService;
            _mailService = mailService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterNewAccountDTO addAccountInfoDTO,string returnUrl)
        {
            if(!ModelState.IsValid) {
                return BadRequest("Please fulfill your field");
            }

            // 1. Register new account
            var registerResponse = await _authenticationService.Register(addAccountInfoDTO);
            if(registerResponse.Status != ResponseStatus.Successed)
            {
                return StatusCode(registerResponse.StatusCode, registerResponse);
            }
            // 2. 
            var claims = new List<Claim>()
            {
                new Claim("TokenType",TokenType.EmailConfirm.ToString())
            };
            var tokenConfig = new TokenConfiguration<string, TokenType>() {
                ExpiresTime = DateTime.UtcNow.AddDays(1),
                TokenKey = _configuration.GetSection("AppSettings:MailToken").Value,
                TokenData = TokenType.EmailConfirm.ToString()
            };
            var token = _tokenService.GenerateMailToken(claims,tokenConfig);
            var requestOrigin = Request.Headers["Origin"].ToString();
            var emailSendingResponse = await _mailService.SendGmailAsync(new MailContent
            {
                DisplayName = "Ads Service Sharing Admin.",
                To = registerResponse.Data.Email,
                Subject = "Thanking letter",
                IsHtmlBody = true,
                Body = "Thanks",
                HtmlBody = $"<h1>Thanks for becomming our member</h1>" +
                $"<p>All steps are completed:<span>" +
                $"<a href=\"{requestOrigin}/auth/confirmEmail/redirect?userId={registerResponse.Data.AccountId}&token={token}&returnURL={returnUrl}\">\"Activate your account\"</a>" +
                $"</span> to confirm your email</p>"
            });
            if (emailSendingResponse.Status != ResponseStatus.Successed) 
            {
                return StatusCode(emailSendingResponse.StatusCode,emailSendingResponse);
            }

            return StatusCode(registerResponse.StatusCode,registerResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginAccountDTO loginAccount, string returnURL)
        {
            bool hasExistedRefreshJWT = Request.Cookies["r_jwt"] != null;
            var loginResponse = await _authenticationService.Login(loginAccount);
            if (loginResponse.Status == ResponseStatus.NoActivatedAccount)
            {
                var claims = new List<Claim>()
                {
                    new Claim("TokenType",TokenType.EmailConfirm.ToString())
                };
                var tokenConfig = new TokenConfiguration<string, TokenType>()
                {
                    ExpiresTime = DateTime.UtcNow.AddDays(1),
                    TokenKey = _configuration.GetSection("AppSettings:MailToken").Value,
                    TokenData = TokenType.EmailConfirm.ToString()
                };
                var token = _tokenService.GenerateMailToken(claims, tokenConfig);
                var requestOrigin = Request.Headers["Origin"].ToString();
                var emailSendingResponse = await _mailService.SendGmailAsync(new MailContent
                {
                    DisplayName = "Ads Service Sharing Admin.",
                    To = loginAccount.Email,
                    Subject = "Thanking letter",
                    IsHtmlBody = true,
                    Body = "Thanks",
                    HtmlBody = $"<h1>Thanks for becomming our member</h1>" +
                    $"<p>All steps are completed:<span>" +
                    $"<a href=\"{requestOrigin}/auth/confirmEmail/redirect?userId={loginResponse.Data.AccountId}&token={token}&returnURL={returnURL}\">\"Activate your account\"</a>" +
                    $"</span> to confirm your email</p>",
                });
                if (emailSendingResponse.Status != ResponseStatus.Successed)
                {
                    return StatusCode(emailSendingResponse.StatusCode, emailSendingResponse);
                }
                if(emailSendingResponse.Status == ResponseStatus.Successed)
                {
                    emailSendingResponse.Message = "Please check your email to confirm account";
                    return StatusCode(emailSendingResponse.StatusCode, emailSendingResponse);
                }
            }    
            
            return StatusCode(loginResponse.StatusCode, loginResponse);
            
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            var response = _authenticationService.Logout();
            return StatusCode(response.StatusCode,response);
        }

        [Authorize]
        [HttpGet("account")]
        public async Task<IActionResult> GetUser()
        {
            var host = Request.Host;
            var response = await _authenticationService.GetUser();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ActivateAccount(int userId, string token)
        {

            if(!(await _tokenService.ValidateMailToken(token)))
            {
                return StatusCode(400, "Token is expired");
            }

            var tokenValue = await _tokenService.GetClaimValue(token, "TokenType");
            if (tokenValue != TokenType.EmailConfirm.ToString())
            {
                return StatusCode(400, "The type of token is invalid");
            }

            var response = await _authenticationService.ConfirmEmail(userId);
            if(response.Status == ResponseStatus.Failed)
            {
                return StatusCode(400,response);
            }
            return Ok(response);
        }

        [HttpPost("confirm/changePassword")]
        public async Task<IActionResult> SendEmailToChangePassword(string email,string returnUrl)
        {
            // 1. Get User Response
            var getUserResponse = await _authenticationService.GetUserByEmail(email);
            if(getUserResponse.Status != ResponseStatus.Successed)
            {
                return StatusCode(getUserResponse.StatusCode, getUserResponse);
            }

            // 2. Create response 
            var claims = new List<Claim>()
            {
                new Claim("TokenType",TokenType.ChangePassword.ToString())
            };
            var tokenConfig = new TokenConfiguration<string, TokenType>()
            {
                ExpiresTime = DateTime.UtcNow.AddDays(1),
                TokenKey = _configuration.GetSection("AppSettings:MailToken").Value,
                TokenData = TokenType.ChangePassword.ToString()
            };
            var token = _tokenService.GenerateMailToken(claims, tokenConfig);
            var requestOrigin = Request.Headers["Origin"].ToString();
            var emailSendingResponse = await _mailService.SendGmailAsync(new MailContent
            {
                DisplayName = "Ads Service Sharing Admin.",
                To = getUserResponse.Data.Email,
                Subject = "You wanna change password",
                IsHtmlBody = true,
                Body = "Thanks",
                HtmlBody = $"<h1>Thank for using our service</h1>" +
                $"<h3>Click on button to change your password</h3><br/>" +
                $"<a href=\"{requestOrigin}/auth/changePassword?email={getUserResponse.Data.Email}&returnURL={returnUrl}&token={token}\"><button style=\"color:green\">Click here</button></a>"
            });
            if (emailSendingResponse.Status != ResponseStatus.Successed)
            {
                return StatusCode(emailSendingResponse.StatusCode, emailSendingResponse);
            }

            return StatusCode(200, new {
                Token = token,
                RequestOrigin = requestOrigin,
                Email = getUserResponse.Data.Email,
                ReturnUrl = returnUrl
            });;
        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO request)
        {
            if (!(await _tokenService.ValidateMailToken(request.Token)))
            {
                return StatusCode(400,"Token is expired");
            }

            var tokenValue = await _tokenService.GetClaimValue(request.Token, "TokenType");
            if(tokenValue != TokenType.ChangePassword.ToString())
            {
                return StatusCode(400, "Token is incorrect type");
            }
            var changePassResponse = await _authenticationService.ChangePassword(request);

            return StatusCode(changePassResponse.StatusCode,changePassResponse);
        }

        [HttpGet("refreshtoken")]
        public async Task<IActionResult> RefreshToken()
        {
            var accessToken = Request.Cookies["jwt"];
            var refreshToken = Request.Cookies["r_jwt"];
            if(refreshToken == null)
            {
                var response = new ServiceResponse<string>() { Data = null, Status = ResponseStatus.NotAcceptableToken, Message = "Please signin first", ServerMessage = "Incompleted service", StatusCode = 400 };
                return StatusCode(response.StatusCode, response);
            }
            var refreshTokenResponse = await _authenticationService.RefreshToken(new AuthTokenRequest()
            {
                Token = accessToken,
                RefreshToken = Request.Cookies["r_jwt"]
            });
            return StatusCode(refreshTokenResponse.StatusCode, refreshTokenResponse);  
        }
    }
}
