using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.Helpers
{
    public class ValidatePasswordAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return base.IsValid(value, validationContext);
        }
    }
}
