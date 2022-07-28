using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using AdsMarketSharing.Data;
using AutoMapper;
using NUnit.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using AdsMarketSharing.Repositories;
using AdsMarketSharing.Entities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Primitives;
using Moq;

namespace AdsMarketSharing.Test
{
    [TestFixture]
    public class BaseTests
    {
        protected SQLExpressContext _context;
        protected IMapper _mapper;
        protected IHttpContextAccessor _httpContext; 
        protected IConfiguration _configuration;

        [OneTimeSetUp]
        public void Setup()
        {
            // 1. _configuration
            var configLst = new Dictionary<string, string>()
            {
                {"AppSettings:Token", "Secrete testing adsmarketsharing" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configLst).Build();
            _configuration = configuration;

            // 2. _context
            var options = new DbContextOptionsBuilder<SQLExpressContext>()
                .UseInMemoryDatabase("testDatabase").Options;
            _context = new SQLExpressContext(options);

            // 3. _mapper
            var mapConfig = new MapperConfiguration(options =>
            {
                options.AddProfile(new AutoMapperProfile());
            });
            _mapper = mapConfig.CreateMapper();

            // 4. _httpContext
            var defaultcontext = new DefaultHttpContext();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(req => req.HttpContext).Returns(defaultcontext);
            _httpContext = mockHttpContextAccessor.Object;

            // 5. Seed Role
            _context.Roles.Add(new Role()
            {
                Id = 1,
                Name = "admin"
            });

            _context.SaveChanges();
        }
        [OneTimeTearDown]
        public void CleanUp()
        {
            _context.RemoveRange(_context.Accounts);
            _context.RemoveRange(_context.Roles);
            _context.SaveChanges();
        }

        public AuthRepository BuildAuthReposity()
        {
            var authRepo = new AuthRepository(_mapper, _context, _configuration, _httpContext);
            return authRepo;
        }

        public static IRequestCookieCollection MockRequestCookieCollection(string key, string value)
        {
            var requestFeature = new HttpRequestFeature();
            var featureCollection = new FeatureCollection();

            requestFeature.Headers = new HeaderDictionary();
            requestFeature.Headers.Add(HeaderNames.Cookie, new StringValues(key + "=" + value));

            featureCollection.Set<IHttpRequestFeature>(requestFeature);

            var cookiesFeature = new RequestCookiesFeature(featureCollection);

            return cookiesFeature.Cookies;
        }
    }
}
