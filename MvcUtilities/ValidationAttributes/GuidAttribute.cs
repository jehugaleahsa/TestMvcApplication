using System;
using System.ComponentModel.DataAnnotations;

namespace MvcUtilities.ValidationAttributes
{
    public class GuidAttribute : ValidationAttribute
    {
        public GuidAttribute()
            : base("{0} must be a valid GUID.")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string asString = value as String;
            if (asString != null)
            {
                Guid guid;
                bool isValid = Guid.TryParse(asString, out guid);
                if (isValid)
                {
                    return null;
                }
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return base.IsValid(value, validationContext);
        }
    }
}
