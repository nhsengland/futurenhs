namespace MvcForum.Core.Models.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateFileLengthAttribute : ValidationAttribute
    {
        private const string ZERO_CONTENT_LENGTH_ERROR = "The file size must be greater than zero bytes.";

        // max file length, defaults to 500kb (512000 bytes)
        private readonly long _maxLengthBytes = 5 * 1024 * 100;

        public ValidateFileLengthAttribute(long maxLengthBytes)
        {
            _maxLengthBytes = maxLengthBytes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is HttpPostedFileBase file)
            {
                if (file.ContentLength == 0)
                {
                    return new ValidationResult(ZERO_CONTENT_LENGTH_ERROR);
                }

                if (file.ContentLength > _maxLengthBytes)
                {
                    return new ValidationResult(ErrorMessageString);
                }
            }

            return ValidationResult.Success;
        }
    }
}