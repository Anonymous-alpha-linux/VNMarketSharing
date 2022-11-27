using AdsMarketSharing.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using AdsMarketSharing.DTOs.File;
namespace AdsMarketSharing.Interfaces
{
    public interface IFileStorageService
    {
        Task<ServiceResponse<string>> CreateFolder(string folderName);
        //Task<ServiceResponse<string>> ReadFolder(string folderName);
        Task<ServiceResponse<AttachmentResponseDTO>> SaveImage(string fileName, Stream fileStream, string folder);
        Task<ServiceResponse<AttachmentResponseDTO>> CreateFolderAndSaveImage(string fileName, Stream fileStream, string newFolderName);
        Task<ServiceResponse<bool>> AddNewFile(IFormFile formFile);
        Task<ServiceResponse<AttachmentResponseDTO>> UpdateExistingFile(string removedPublicId, string newFileName, Stream fileStream, string newFolderName);
        Task<ServiceResponse<bool>> DeleteFile(string id);
        Task<ServiceResponse<bool>> EditFile(IFormFile formFile);
        Task<ServiceResponse<bool>> ReadFile(IFormFile formFile);
        Task<ServiceResponse<bool>> SaveFile(IFormFile formFile);
    }
}
