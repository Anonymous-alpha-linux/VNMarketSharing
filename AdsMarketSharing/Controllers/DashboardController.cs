using AdsMarketSharing.Data;
using AdsMarketSharing.DTOs.Product;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace AdsMarketSharing.Controllers
{
    [Authorize]
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : Controller
    {
        private readonly SQLExpressContext _dbContext;
        private readonly IMapper _mapper;

        public DashboardController(SQLExpressContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet("admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            return Ok();
        }
        [HttpGet("seller")]
        public async Task<IActionResult> GetSellerDashboard(int userId) {
            return Ok();
        }
        [HttpGet("seller/transaction")]
        public async Task<IActionResult> GetByYear([FromQuery]int userId, [FromBody] DateTime time)
        {
            return Ok();
        }
        [HttpGet("product/recent")]
        public async Task<IActionResult> GetRecentProduct()
        {
            var recentProductList = _dbContext.Products
                .AsNoTracking()
                .OrderByDescending(p => p.Orders.Count)
                .Include(p => p.UserPage)
                .Include(p => p.Attachments)
                    .ThenInclude(p => p.Attachment)
                .Skip(0)
                .Take(5)
                .AsQueryable();

            var response = await recentProductList.ProjectTo<RecentProductResponseDTO>(_mapper.ConfigurationProvider).ToListAsync();

            return Ok(response);
        }
        [HttpGet("seller/recent")]
        public async Task<IActionResult> GetRecentSellers()
        {
            return Ok();
        }
    }
}
