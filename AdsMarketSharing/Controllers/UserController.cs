using AdsMarketSharing.DTOs.File;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
using AutoMapper.QueryableExtensions;
using System.IdentityModel.Tokens.Jwt;
using AdsMarketSharing.DTOs.Notification;
using AdsMarketSharing.DTOs.Product;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AdsMarketSharing.Controllers
{
    [Authorize]
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly SQLExpressContext _context;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IFileStorageService fileStorageService, SQLExpressContext context, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _fileStorageService = fileStorageService;
            _context = context;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
       
        [HttpGet]
        public async Task<IActionResult> GetInfo()
        {
            string accountIdStr = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            int.TryParse(accountIdStr, out int accountId);
            try
            {
                var user = await _context.Accounts.Include(acc => acc.User)
                    .ThenInclude(user => user.Avatar)
                    .Where(account => account.Id == accountId)
                    .Select(acc => new
                    {
                        UserId = acc.User.Id,
                        Username = !string.IsNullOrEmpty(acc.User.OrganizationName) ? acc.User.OrganizationName : acc.Email,
                        Avatar = acc.User.Avatar.PublicPath
                    })
                    .FirstOrDefaultAsync();
                return Ok(user);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("avatar")]
        public async Task<IActionResult> ChangeAvatar([FromForm] UploadFileDTO request)
        {
            // 1. Get the info of user
            string usernameStr = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.Email).Value;
            string nameIdentifierStr = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

            var userRecord = _context.Users.Include(p => p.Avatar).FirstOrDefault(p => p.AccountId.ToString() == nameIdentifierStr);

            if (userRecord.Avatar == null)
            {
                var serviceResponseNullAvatar = (await _fileStorageService.CreateFolderAndSaveImage(request.File.FileName,request.File.OpenReadStream(), usernameStr));

                userRecord.Avatar = _mapper.Map<Attachment>(serviceResponseNullAvatar.Data);

                _context.Users.Update(userRecord);

                _context.SaveChanges();

                return StatusCode(serviceResponseNullAvatar.StatusCode, new
                {
                    Message = "Added new avatar",
                    NewAvatar = serviceResponseNullAvatar.Data.PublicPath
                });
            }

            var serviceResponseHadAvatar = (await _fileStorageService.UpdateExistingFile(userRecord.Avatar.Name, request.File.FileName, request.File.OpenReadStream(), usernameStr));

            _context.Attachments.Remove(userRecord.Avatar);

            userRecord.Avatar = _mapper.Map<Attachment>(serviceResponseHadAvatar.Data);
            _context.Users.Update(userRecord);

            _context.SaveChanges();

            return StatusCode(serviceResponseHadAvatar.StatusCode, new { 
                Message = "Updated avatar",
                NewAvatar = serviceResponseHadAvatar.Data.PublicPath 
            });
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetUserInfo([FromQuery]int userId)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.Find(user => user.Id == userId);
                return Ok(user.FirstOrDefault());
            }
            catch (System.Exception exception)
            {
                return BadRequest("Cannot find your user information. Please specific it");
            }   
        }

        [HttpPut("updateInfo")]
        public async Task<IActionResult> UpdateInfo(GenerateUserRequestDTO request)
        {
            // 1. Get Authorized Id
            string nameIdentifierStr = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            int.TryParse(nameIdentifierStr, out int accountId);

            // 2. Found exist user or non-existing user
            var foundUser = _context.Users.FirstOrDefault(u => u.AccountId == accountId);

            var userEntity = new Entities.User();

            userEntity = _mapper.Map(request,foundUser);        
            userEntity.AccountId = accountId;

            _context.Update(userEntity);
            _context.SaveChanges();

            return Ok(new
            {
                OrganizationName = userEntity.OrganizationName,
                Message = "Updated successfully"
            });
        }

        [HttpPost("upload/banner")]
        public async Task<IActionResult> UploadAds()
        {
            return Ok();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("list")]
        public async Task<IActionResult> GetUserList()
        {
            var sellerList = _context.Users.Include(p => p.Avatar).Include(p => p.Account).AsQueryable();

            var result = sellerList.ProjectTo<GetUserByAdminDTO>(_mapper.ConfigurationProvider).ToList();
            return Ok(result);
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("block")]
        public async Task<IActionResult> BlockUser(int userId, bool isBlocked)
        {
            string accountIdStr = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            int.TryParse(accountIdStr, out int accountId);

            var foundUser = _context.Accounts
                .Include(p => p.AccountRoles)
                .ThenInclude(p => p.Role)
                .FirstOrDefault(p => p.User.Id == userId);
            if(accountId == foundUser.Id)
            {
                return BadRequest("You cannot block yourself");
            }

            if(foundUser.AccountRoles.Any(p => p.RoleId == 1 || p.Role.Name.ToLower().Contains("admin")))
            {
                return BadRequest("Cannot block this user");
            }

            foundUser.Enabled = !isBlocked;

            _context.Accounts.Update(foundUser);
            _context.SaveChanges();
            return Ok(new
            {
                Message = isBlocked ? "Blocked this user" : "Unlocked this user"
            });
        }

        // 2. Address Visitor 
        [HttpGet("addresses")]
        public async Task<IActionResult> GetAddresses([FromQuery] int userId, int addressType) {
            if(userId == null)
            {
                return BadRequest("Specify your user identifier");
            }
            var addressLst = await _unitOfWork.ReceiverAddressRepository.Find(address => address.UserId == userId && address.AddressType == addressType);
            return Ok(addressLst);
        }

        [HttpGet("address/{addressId}")]
        public async Task<IActionResult> GetSingleAddress([FromRoute]int addressId)
        {
            if (addressId == null)
            {
                return BadRequest("Specify your address identifier");
            }
            var address = await _unitOfWork.ReceiverAddressRepository.GetById(addressId);
            if(address == null)
            {
                return NotFound();
            }
            return Ok(address);
        }

        [HttpPost("createAddress")]
        public async Task<IActionResult> AddAddress([FromBody] AddAddressRequestDTO request) {
            try
            {
                var receiverAddress = await _unitOfWork.ReceiverAddressRepository.Add(_mapper.Map<AddAddressRequestDTO,ReceiverAddress>(request));
                await _unitOfWork.CompleteAsync();
                return Ok(receiverAddress);
            }
            catch (System.Exception e)
            {
                return StatusCode(400, e.Message);
            }
        }

        [HttpPut("updateAddress")]
        public async Task<IActionResult> UpdateAddress(UpdateAddressRequestDTO request,int addressId) {
            try
            {

                var userRecord = await _unitOfWork.ReceiverAddressRepository.GetById(addressId);

                var newRecord = _mapper.Map<UpdateAddressRequestDTO, ReceiverAddress>(request,userRecord);

                var newAddress = await _unitOfWork.ReceiverAddressRepository.Update(addressId,newRecord); 
                if(newAddress == null)
                {
                    return BadRequest("There are no update");
                }
                await _unitOfWork.CompleteAsync();
                return Ok(newAddress);
            }
            catch (System.Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("setdefault")]
        public async Task<IActionResult> UpdateAddressDefault(int addressId, int userId, int type)
        {
            try
            {
                if(addressId != null && userId != null && type != null)
                {
                    var foundAddressLst = _context.ReceiverAddresses
                        .Where(p => p.UserId == userId && p.AddressType == type).ToList();

                     foundAddressLst = foundAddressLst.Select(p => {
                            if (p.Id == addressId)
                            {
                                p.IsDefault = true;
                            }
                            else
                            {
                                p.IsDefault = false;
                            }
                            return p;
                        }).ToList();

                    _context.ReceiverAddresses.UpdateRange(foundAddressLst);
                    _context.SaveChanges();
                    return Ok("Set default success");
                }
                return BadRequest("Your request must have three params: addressId, userId, type");
            }
            catch (System.Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("removeAddress")]
        public async Task<IActionResult> RemoveAddress(int addressId)
        {
            try
            {
                string nameIdentifierStr = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                var foundAddress = await _unitOfWork.ReceiverAddressRepository.GetById(addressId);
                if(foundAddress == null) {
                    return NotFound("No found items"); 
                }
                var foundUser = await _unitOfWork.UserRepository.GetById(foundAddress.UserId);

                var isPermitted = foundUser.AccountId.ToString() == nameIdentifierStr;

                if (!isPermitted) {
                    return StatusCode(403, "You cannot delete this");
                }

                await _unitOfWork.ReceiverAddressRepository.Delete(addressId);
                await _unitOfWork.CompleteAsync();
                return Ok("Deleted");
            }
            catch (System.Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("notifies")]
        public async Task<IActionResult> GetNotifications([FromQuery] FilterNotificationRequestDTO filter)
        {
            var notificationList = _context.Notifytrackers
                .OrderByDescending(p => p.Notification.CreatedAt)
                .Include(p => p.Notification)
                .Include(p => p.ToUser)
                .Where(p => p.UserId == filter.UserId);

            int max = notificationList.Count();

            if(filter.Page > 0 && filter.Take > 0)
            {
                notificationList = notificationList.Skip((filter.Page - 1) * filter.Take).Take(filter.Take);
            }

            var result = notificationList
                    .ProjectTo<NotificationTrackerResponseDTO>(_mapper.ConfigurationProvider)
                    .ToList();

            return Ok(new
            {
                Result = result,
                Amount = result.Count,
                Max = max
            });
        }
    }
}
