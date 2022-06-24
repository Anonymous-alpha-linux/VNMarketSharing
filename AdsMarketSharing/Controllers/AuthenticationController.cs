using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

using AdsMarketSharing.Enum;
using AdsMarketSharing.Models;
using AdsMarketSharing.DTOs.Account;
using AdsMarketSharing.Interfaces;
using AdsMarketSharing.Models.Email;
using System.Collections.Generic;
using AdsMarketSharing.Models.Token;
using Microsoft.Extensions.Configuration;

namespace AdsMarketSharing.Controllers
{
    [ApiController]
    [Route("api/auth")]
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
        public async Task<IActionResult> Register(AddAccountInfoDTO addAccountInfoDTO)
        { 
           // 1. Register new account
            var registerResponse = await _authenticationService.Register(addAccountInfoDTO);
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

            string currentHost = Request.Host.ToString();   

            var emailSendingResponse = await _mailService.SendGmailAsync(new MailContent
            {
                DisplayName = "Ads Service Sharing Admin.",
                To = registerResponse.Data.Email,
                Subject = "Thanking letter",
                IsHtmlBody = true,
                Body = "Thanks",
                HtmlBody = $"<h1>Thanks for becomming our member<h1>" +
                $"<p>Please click here:<span>" +
                $"<a href=\"https://{currentHost}/api/auth/confirmEmail?userId={registerResponse.Data.AccountId}&token={token}\">\"https://{currentHost}/\"</a>" +
                $"</span> to confirm your email</p>"
            });

            if (emailSendingResponse.Status != ResponseStatus.Successed) 
            {
                return StatusCode(emailSendingResponse.StatusCode,emailSendingResponse);
            }
            return StatusCode(registerResponse.StatusCode,registerResponse);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginAccount loginAccount)
        {
            var loginResponse = await _authenticationService.Login(loginAccount);

          
            if(loginResponse.Status == ResponseStatus.NoActivatedAccount)
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

                string currentHost = Request.Host.ToString();
                var emailSendingResponse = await _mailService.SendGmailAsync(new MailContent
                {
                    DisplayName = "Ads Service Sharing Admin.",
                    To = loginAccount.Email,
                    Subject = "Thanking letter",
                    IsHtmlBody = true,
                    Body = "Thanks",
                    HtmlBody = $"<h1>Welcome for becomming our member<h1>" +
                    $"<p>Please click here:<span>" +
                    $"<a href=\"https://{currentHost}/api/auth/confirmEmail?userId={loginResponse.Data.AccountId}&token={token}\">\"https://{currentHost}/\"</a>" +
                    $"</span> to confirm your email</p>"
                });

                emailSendingResponse.Message = emailSendingResponse.Status == ResponseStatus.Successed ?
                    "Please check email to activate your account"
                    : emailSendingResponse.Message;

                return StatusCode(emailSendingResponse.StatusCode, emailSendingResponse);
            }
            return StatusCode(loginResponse.StatusCode, loginResponse);
            
        }
        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ActivateEmail(int userId, string token)
        {
            var tokenValue = await _tokenService.GetClaimValue(token, "TokenType");

            if(!(await _tokenService.ValidateMailToken(token)))
            {
                return StatusCode(400, "Token is expired");
            }

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
        /*[HttpGet("refreshToken")]*/
       /* public async Task<IActionResult> RefreshToken()
        {
            return Ok();
        }*/
    }
}
