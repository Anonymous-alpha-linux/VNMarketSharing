using AdsMarketSharing.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using AdsMarketSharing.Repositories;
using AdsMarketSharing.DTOs.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using AdsMarketSharing.Services.FileUpload;
using Castle.Core.Configuration;

namespace AdsMarketSharing.Test.UnitTests
{
    [TestFixture]
    public class AuthRepositoryTests : BaseTests
    {
        private IAuthRepository _authRepository;

        // 1. Register
        [Test]
        public async Task Register_NewAccount_ReturnCode201()
        {
            // Arrange
            var configLst = new Dictionary<string, string>()
            {
                {"AppSettings:Token", "Secrete testing" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configLst).Build();
            _authRepository = new AuthRepository(_mapper, _context, configuration, _httpContext, new CloudinaryStorageService(configuration));
            var account = new RegisterNewAccountDTO()
            {
                Email = "test@email.com",
                Password = "12345678901234567890",
                ConfirmPassword = "12345678901234567890",
                RoleId = 1,
            };
            // Act
            var result = await _authRepository.Register(account);
            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(201));
        }
        [Test]
        public async Task Register_ExistedEmail_ReturnCode400()
        {
            // Arrange
            var configLst = new Dictionary<string, string>()
            {
                {"AppSettings:Token", "Secrete testing" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configLst).Build();
            _authRepository = new AuthRepository(_mapper, _context, configuration, _httpContext, new CloudinaryStorageService(configuration));
            var account = new RegisterNewAccountDTO()
            {
                Email = "test1@email.com",
                Password = "12345678901234567890",
                ConfirmPassword = "12345678901234567890",
                RoleId = 1,
            };
            // Act
            var preRegistered = await _authRepository.Register(account);
            var result = await _authRepository.Register(account);
            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }
        [Test]
        public async Task Register_UnavalableRole_Return404()
        {
            // Arrange
            var configLst = new Dictionary<string, string>()
            {
                {"AppSettings:Token", "Secrete testing" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configLst).Build();
            _authRepository = new AuthRepository(_mapper, _context, configuration, _httpContext, new CloudinaryStorageService(configuration));
            var account = new RegisterNewAccountDTO()
            {
                Email = "test2@email.com",
                Password = "12345678901234567890",
                ConfirmPassword = "12345678901234567890",
                RoleId = 3,
            };
            // Act
            var result = await _authRepository.Register(account);
            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }


        // 2. Login
        [Test]
        public async Task Login_ExistingAccount_Return200() {
            // Arrange
            var _authRepository = new AuthRepository(_mapper, _context, _configuration, _httpContext, new CloudinaryStorageService(_configuration));

            // Seed account data
            var account = new RegisterNewAccountDTO()
            {
                Email = "testlogin@email.com",
                Password = "12345678901234567890",
                ConfirmPassword = "12345678901234567890",
                RoleId = 1,
            };
            var pre = await _authRepository.Register(account);
            var foundAccount = await _context.Accounts
                .FirstOrDefaultAsync(acc => acc.Id == pre.Data.AccountId);
            foundAccount.IsActive = true;
            _context.Accounts.Update(foundAccount);
            await _context.SaveChangesAsync();

            var loginAccount = new LoginAccountDTO()
            {
                Email = "testlogin@email.com",
                Password = "12345678901234567890"
            };
            // Act
            var result = await _authRepository.Login(loginAccount);
            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }
        [Test]
        public async Task Login_NotExistingAccount_Return404()
        {
            // Arrange
            var _authRepository = new AuthRepository(_mapper, _context, _configuration, _httpContext, new CloudinaryStorageService(_configuration));
            var loginAccount = new LoginAccountDTO()
            {
                Email = "unknown@email.com",
                Password = "12345678901234567890",
            };
            // Act
            var result = await _authRepository.Login(loginAccount);
            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }
        [Test]
        public async Task Login_SignedInAlready_Return401()
        {
            // Arrange
            var _authRepository = new AuthRepository(_mapper, _context, _configuration, _httpContext, new CloudinaryStorageService(_configuration));

            // Seed account data
            var account = new RegisterNewAccountDTO()
            {
                Email = "signedinalready@email.com",
                Password = "12345678901234567890",
                ConfirmPassword = "12345678901234567890",
                RoleId = 1,
            };
            var pre = await _authRepository.Register(account);
            var foundAccount = await _context.Accounts
                .FirstOrDefaultAsync(acc => acc.Id == pre.Data.AccountId);
            foundAccount.IsActive = true;
            _context.Accounts.Update(foundAccount);
            await _context.SaveChangesAsync();

            var loginAccount = new LoginAccountDTO()
            {
                Email = "signedinalready@email.com",
                Password = "12345678901234567890",
            };
            // Act
            var preresult = await _authRepository.Login(loginAccount);
            var result = await _authRepository.Login(loginAccount);
            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(401));
        }
        [Test]
        public async Task Login_IncorrectPassword_Return400()
        {
            // Arrange
            var _authRepository = new AuthRepository(_mapper, _context, _configuration, _httpContext, new CloudinaryStorageService(_configuration));
            // Seed account data
            var account = new RegisterNewAccountDTO()
            {
                Email = "testincorrectaccount@email.com",
                Password = "12345678901234567890",
                ConfirmPassword = "12345678901234567890",
                RoleId = 1,
            };
            var pre = await _authRepository.Register(account);
            var foundAccount = await _context.Accounts
                .FirstOrDefaultAsync(acc => acc.Id == pre.Data.AccountId);
            foundAccount.IsActive = true;
            _context.Accounts.Update(foundAccount);
            await _context.SaveChangesAsync();

            var loginAccount = new LoginAccountDTO()
            {
                Email = "testincorrectaccount@email.com",
                Password = "wrongpassword",
            };
            // Act
            var result = await _authRepository.Login(loginAccount);
            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }
        [Test]
        public async Task Login_NotActiveAccount_Return403()
        {
            // Arrange

            var _authRepository = new AuthRepository(_mapper, _context, _configuration, _httpContext, new CloudinaryStorageService(_configuration));
            // Seed account data
            var account = new RegisterNewAccountDTO()
            {
                Email = "testnoactiveaccount@email.com",
                Password = "12345678901234567890",
                ConfirmPassword = "12345678901234567890",
                RoleId = 1,
            };
            var pre = await _authRepository.Register(account);
            await _context.SaveChangesAsync();

            var loginAccount = new LoginAccountDTO()
            {
                Email = "testnoactiveaccount@email.com",
                Password = "12345678901234567890",
            };

            // Act
            var result = await _authRepository.Login(loginAccount);
            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(403));
        }


        // 3. Logout
        [Test]
        public async Task Logout_SignedInAlreadry_Return200() {
            // Arrange
            _httpContext.HttpContext.Request.Cookies = MockRequestCookieCollection("jwt", "tokenabc");
            var _authRepository = new AuthRepository(_mapper, _context, _configuration, _httpContext, new CloudinaryStorageService(_configuration)  );
            // Seed account data
            var account = new RegisterNewAccountDTO()
            {
                Email = "testnoactiveaccount@email.com",
                Password = "12345678901234567890",
                ConfirmPassword = "12345678901234567890",
                RoleId = 1,
            };
            var pre = await _authRepository.Register(account);
            await _context.SaveChangesAsync();

            var loginAccount = new LoginAccountDTO()
            {
                Email = "testnoactiveaccount@email.com",
                Password = "12345678901234567890",
            };
            // Act         
            var result = _authRepository.Logout();
            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }
        [Test]
        public async Task Logout_NotSignedInYet_Return404() { }

        // 4. RefreshToken
        [Test]
        public async Task RefreshToken_ExpiredToken_Return200() { }
        [Test]
        public async Task RefreshToken_NonExpiredToken_Return403(){}
        
    }
}
