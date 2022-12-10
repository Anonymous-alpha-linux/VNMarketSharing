using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AdsMarketSharing.DTOs.File;
using System.Threading.Tasks;
using AdsMarketSharing.Interfaces;
using AdsMarketSharing.Data;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using AdsMarketSharing.Enum;
using AutoMapper;
using AdsMarketSharing.Entities;
using AdsMarketSharing.DTOs.User;
using Microsoft.EntityFrameworkCore;
using AdsMarketSharing.DTOs.UserPage;
using AutoMapper.QueryableExtensions;
using AdsMarketSharing.DTOs.Payment;
using AdsMarketSharing.Services.Payment;

namespace AdsMarketSharing.Controllers
{
    [Route("api/seller")]
    [ApiController]
    [Authorize]
    public class SellerController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly SQLExpressContext _context;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public SellerController(IFileStorageService fileStorageService, SQLExpressContext context, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _fileStorageService = fileStorageService;
            _context = context;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> GetInfo([FromQuery]int userId)
        {
            try
            {
                var foundUserPage = _context.UserPages
                    .Include(up => up.BannerUrl)
                    .Include(up => up.PageAvatar)
                    .FirstOrDefault(p => p.UserId == userId);
                if(foundUserPage == null)
                {
                    return NotFound();
                }    
                var result = _mapper.Map<GetUserPageResponseDTO>(foundUserPage);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("profile")]
        public async Task<IActionResult> CreateOrUpdateUserPage([FromQuery] int userId, UserPageCreationDTO request)
        {
            try
            {
                var foundUserPage = _context.UserPages.Include(up => up.User)
                    .FirstOrDefault(p => p.UserId == userId);

                UserPage userPageEntity = foundUserPage;
                userPageEntity = _mapper.Map(request, foundUserPage);
                userPageEntity.UserId = userId;

                _context.Update(userPageEntity);
                _context.SaveChanges();
                return Ok(userPageEntity);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("avatar")]
        public async Task<IActionResult> ChangeAvatar([FromForm]IFormFile newAvatar, int userId)
        {
            try
            {
                string usernameStr = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.Email).Value;
                var userpage = await _context.UserPages.Include(p => p.PageAvatar).FirstOrDefaultAsync(up => up.UserId == userId);
                if (userpage == null) return StatusCode(404, "You are not allowed");
                AttachmentResponseDTO avatarEntity = null;

                if (userpage == null) userpage = new UserPage();
                
                avatarEntity = userpage.PageAvatar != null 
                    ? _fileStorageService.UpdateExistingFile(userpage.PageAvatar.Name, newAvatar.FileName, newAvatar.OpenReadStream(), usernameStr).Result.Data
                    : _fileStorageService.CreateFolderAndSaveImage(newAvatar.FileName, newAvatar.OpenReadStream(), usernameStr).Result.Data;

                var uploadAttachment = _mapper.Map<Attachment>(avatarEntity);

                userpage.UserId = userId;
                userpage.PageAvatar = uploadAttachment;

                _context.UserPages.Update(userpage);
                _context.SaveChanges();
                return Ok("Changed user page avatar");
            }
            catch (System.Exception e)
            {
                return StatusCode(500, e);
            }
        }
        [HttpPut("banner")]
        public async Task<IActionResult> ChangeBanner([FromForm] IFormFile newBanner, int userId)
        {
            string usernameStr = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.Email).Value;
            var userpage = await _context.UserPages.FirstOrDefaultAsync(up => up.UserId == userId);
            if (userpage == null) return StatusCode(404, "You are not allowed");

            var avatarEntity = _fileStorageService.CreateFolderAndSaveImage(newBanner.FileName, newBanner.OpenReadStream(), usernameStr).Result.Data;
            var uploadAttachment = _mapper.Map<Attachment>(avatarEntity);
            userpage.UserId = userId;
            userpage.BannerUrl = uploadAttachment;

            _context.Update(userpage);
            _context.SaveChanges();
            return Ok("Changed user page banner");
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("block")]
        public async Task<IActionResult> BlockUserPage()
        {
            return Ok();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("list")]
        public async Task<IActionResult> GetSellerList()
        {
            var sellerList = _context.UserPages.Include(p => p.BannerUrl).AsQueryable();

            var result = sellerList.ProjectTo<GetUserPageResponseDTO>(_mapper.ConfigurationProvider).ToList();
            return Ok(result);
        }
        [HttpGet("order")]
        public async Task<IActionResult> GetOrderList([FromQuery] int merchantId)
        {
            var orderList = _context.Orders.Where(p => p.MerchantId == merchantId).OrderBy(p => (p.OrderStatus == OrderStatus.Pending) ? 0 :
                                            (p.OrderStatus == OrderStatus.Waiting) ? 1 :
                                            (p.OrderStatus == OrderStatus.Delivering) ? 2 :
                                            (p.OrderStatus == OrderStatus.Delivered) ? 3 :
                                            (p.OrderStatus == OrderStatus.Completed) ? 4 :
                                            (p.OrderStatus == OrderStatus.Cancelled) ? 5 :
                                            (p.OrderStatus == OrderStatus.CustomerNotReceived) ? 6 :
                                    7).AsQueryable();
            var result = orderList.ProjectTo<OrderResponseDTO>(_mapper.ConfigurationProvider).ToList();
            return Ok(result);
        }
        [HttpGet("order/traverse")]
        public async Task<IActionResult> AcceptOrder([FromQuery]OrderRequestDTO request)
        {
            var order = _context.Orders.FirstOrDefault(p => p.Id == request.OrderId);
            if(order == null)
            {
                return BadRequest("Cannot find your order");
            }
            
            order.OrderStatus = request.Status;
            _context.Orders.Update(order);
            _context.SaveChanges();
            return Ok("Updated order");
        }
    }
}
