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
        [HttpGet]
        public async Task<IActionResult> GetInfo()
        {
            string userIdStr = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            int.TryParse(userIdStr, out int userId);
            try
            {
                var user = await _context.Accounts.Include(acc => acc.User)
                    .ThenInclude(user => user.Avatar)
                    .Where(account => account.Id == userId)
                    .Select(acc => new { 
                        Username = !string.IsNullOrEmpty(acc.User.OrganizationName) ? acc.User.OrganizationName : acc.Email, 
                        Avatar = acc.User.Avatar.PublicPath 
                    })
                    .FirstAsync();
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

        [HttpPut("updateInfo")]
        public async Task<IActionResult> UpdateInfo(GenerateUserRequestDTO request)
        {
            string nameIdentifierStr = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var userRecords = (await _unitOfWork.UserRepository.Find(user => user.AccountId.ToString() == nameIdentifierStr)).ToList();
            var userRecord = new Entities.User();
            if(userRecords.Count != 0)
            {
                userRecord = userRecords.First();
            }
            userRecord = _mapper.Map(request,userRecord);
            var response = await _unitOfWork.UserRepository.Upsert(userRecord);
            if(response == null)
            {
                return NotFound("Update user info failed");
            }
            return Ok("Updated successfully");
        }
        [HttpPost("upload/banner")]
        public async Task<IActionResult> UploadAds()
        {
            return Ok();
        }
    }
}
