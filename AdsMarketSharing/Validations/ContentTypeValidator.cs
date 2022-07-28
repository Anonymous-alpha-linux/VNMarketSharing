using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AdsMarketSharing.Validations
{
    public class ContentTypeValidator: ValidationAttribute
    {
        private readonly string[] _allowedContentTypes;
        private readonly string[] _allowedImageTypes = { "image/jpeg", "image/png", "image/gif" };
        private readonly string[] _allowedRawTypes = { "image/jpeg", "image/png", "image/gif" };
        private readonly string[] _allowedVideoTypes = { "image/jpeg", "image/png", "image/gif" };
        private readonly string[] _allowedApplicationTypes = { "image/jpeg", "image/png", "image/gif" };

        public ContentTypeValidator(string[] allowedContentTypes)
        {
            _allowedContentTypes = allowedContentTypes;
        }
        public ContentTypeValidator(ContentTypeGroup contentTypeGroup)
        {
            switch (contentTypeGroup)
            {
                case ContentTypeGroup.Image:
                    _allowedContentTypes = _allowedImageTypes;
                    break;
            }
        }

        private ValidationResult ValidateImageCollectionDefault() {

            return ValidationResult.Success;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value == null) { return ValidationResult.Success; }
            var formFile = value as IFormFile;
            if(formFile == null) { return ValidationResult.Success; }

            if (!_allowedContentTypes.Contains(formFile.ContentType))
            {
                return new ValidationResult($"Content-Type should be one of the following types: {string.Join(", ", _allowedContentTypes)}");
            }

            return ValidationResult.Success;
        }
    }
    public enum ContentTypeGroup
    {
        Image,
        Raw
    }
}
