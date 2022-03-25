namespace MvcForum.Core.Models.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidateFileTypeAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "Only the following file types are allowed: {0}";

        private IEnumerable<string> _validTypes { get; set; }

        public ValidateFileTypeAttribute(string validTypes)
        {
            _validTypes = validTypes.Split(',').Select(s => s.Trim().ToLower());
            ErrorMessage = string.Format(_defaultErrorMessage, string.Join(" or ", _validTypes));
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is HttpPostedFileBase file && !_validTypes.Any(e => file.FileName.EndsWith(e, StringComparison.OrdinalIgnoreCase)))
            {
                return new ValidationResult(ErrorMessageString);
            }

            return ValidationResult.Success;
        }
    }
}