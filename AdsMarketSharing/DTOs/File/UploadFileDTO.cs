using AdsMarketSharing.Validations;
using Microsoft.AspNetCore.Http;

namespace AdsMarketSharing.DTOs.File
{
    public class UploadFileDTO
    {
        [FileSizeValidator(5)]
        [ContentTypeValidator(ContentTypeGroup.Image)]
        public IFormFile File { get; set; }
    }
}
