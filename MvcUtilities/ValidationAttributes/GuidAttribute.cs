﻿using System;
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
                if (!Guid.TryParse(asString, out guid))
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            return null;
        }
    }
}
