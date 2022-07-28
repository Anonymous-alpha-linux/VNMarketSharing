using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Text;

using AdsMarketSharing.DTOs.Account;
using AdsMarketSharing.Data;
using AdsMarketSharing.Controllers;
using AdsMarketSharing.Interfaces;
using AdsMarketSharing.Repositories;

using Moq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace AdsMarketSharing.Test.UnitTests
{
    [TestClass]
    public class AuthenticationControllerTests : BaseTests
    {

        // Naming convention: "UnitOfWork_StateUnderTest_ExpectedBehavior"
        [TestMethod]
        public void AuthController_LoginWithCorrectInfo_SaveRefreshToken()
        {
            // Prepartion
            LoginAccountDTO account1 = new LoginAccountDTO()
            {
                Email = "trungtinh246810f@gmail.com",
                Password = "trungtinh246810"
            };

            // Testing


            // Verification
        }

        [TestMethod]
        public void Register()
        {
            // Preparation
            string databaseName = Guid.NewGuid().ToString();
    

            var accountLst = new List<RegisterNewAccountDTO>()
            {
                new RegisterNewAccountDTO()
                {
                    Email = "trungtinh246810f@gmail.com",
                    Password = "trungtinh246810",
                    ConfirmPassword = "trungtinh246810",
                    RoleId = 1,
                },
                new RegisterNewAccountDTO()
                {
                    Email = "tinhntgcd18753@fpt.edu.vn",
                    Password = "trungtinh246810",
                    ConfirmPassword = "trungtinh246810",
                    RoleId = 2,
                },
                new RegisterNewAccountDTO()
                {
                    Email = "pornhudpremium@gmail.com",
                    Password = "trungtinh246810",
                    ConfirmPassword = "trungtinh246810",
                    RoleId = 3,
                },
                new RegisterNewAccountDTO()
                {
                    Email = "abc@gmail.com",
                    Password = "trungtinh246810",
                    ConfirmPassword = "trungtinh246810",
                    RoleId = 3,
                }
            };
            var configLst = new Dictionary<string, string>()
            {
                {"AppSettings:Token", "Secrete testing" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configLst).Build();

            // Testing
            // Verification

        }

/*        private AuthenticationController BuildAuthController(databaseName)
        {
            cain
        }*/

    }
}
