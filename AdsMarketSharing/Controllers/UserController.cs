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
        [Authorize]
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
        [Authorize]
        [HttpPost("avatar")]
        public async Task<IActionResult> ChangeAvatar([FromForm] UploadFileDTO request)
        {
            // 1. Create folder on cloudinary
            string usernameStr = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.Email).Value;
            var createFolderAction = await _fileStorageService.CreateFolder(usernameStr);

            if (createFolderAction.Status != ResponseStatus.Successed)
            {
                return StatusCode(createFolderAction.StatusCode, createFolderAction);
            }
            // 2. Save to that specific folder
            var saveImageAction = await _fileStorageService.SaveImage(request.File.FileName, request.File.OpenReadStream(), createFolderAction.Data);
            if (saveImageAction.Status != ResponseStatus.Successed)
            {
                return StatusCode(saveImageAction.StatusCode, saveImageAction);
            }

            // 3. Upload avatar
            // 3.1. Save attachment
            var attachmentRecord = await _unitOfWork.AttachmentRepository.Add(_mapper.Map<AttachmentResponseDTO, Attachment>(saveImageAction.Data));
            if (attachmentRecord == null)
            {
                return StatusCode(500, "Something got wrong in server");
            }
            await _unitOfWork.CompleteAsync();
            // 3.2. Update to User
            string nameIdentifierStr = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var userRecords = (await _unitOfWork.UserRepository.Find(user => user.AccountId.ToString() == nameIdentifierStr)).ToList();

            Entities.User userRecord = new Entities.User();
            if (userRecords.Count != 0)
            {
                userRecord = userRecords[0];
            }
            userRecord.AttachmentId = attachmentRecord.Id;
            int accountId;
            if (int.TryParse(nameIdentifierStr, out accountId))
            {
                userRecord.AccountId = accountId;
            }
            var newUserRecord = await _unitOfWork.UserRepository.Upsert(userRecord);
            if (newUserRecord == null)
            {
                return StatusCode(500, "Something got wrong in server");
            }
            await _unitOfWork.CompleteAsync();
            return StatusCode(saveImageAction.StatusCode, new { NewAvatar = attachmentRecord.PublicPath });
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
        [Authorize]
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
        [Authorize]
        [HttpPost("upload/banner")]
        public async Task<IActionResult> UploadAds()
        {
            return Ok();
        }


        // 2. Address Visitor 
        [Authorize]
        [HttpGet("addresses")]
        public async Task<IActionResult> GetAddresses([FromQuery] int userId, int addressType) {
            if(userId == null)
            {
                return BadRequest("Specify your user identifier");
            }
            var addressLst = await _unitOfWork.ReceiverAddressRepository.Find(address => address.UserId == userId && address.AddressType == addressType);
            return Ok(addressLst);
        }
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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

    }
}
