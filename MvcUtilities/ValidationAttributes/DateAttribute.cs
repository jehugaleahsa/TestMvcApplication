using System;
using System.ComponentModel.DataAnnotations;

namespace MvcUtilities.ValidationAttributes
{
    public class DateAttribute : ValidationAttribute
    {
        public DateAttribute()
            : base("{0} must be a valid date.")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string asString = value as String;
            if (asString != null)
            {
                DateTime date;
                if (!DateTime.TryParse(asString, out date))
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }                
            }
            return null;
        }
    }
}
