namespace MvcForum.Core.Models.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateFileLengthAttribute : ValidationAttribute
    {
        // max file length, defaults to 500kb (512000 bytes)
        private readonly long MAX_LENGTH_BYTES = 5 * 1024 * 100;

        public ValidateFileLengthAttribute(long maxLengthBytes)
        {
            this.MAX_LENGTH_BYTES = maxLengthBytes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is HttpPostedFileBase file && file.ContentLength > this.MAX_LENGTH_BYTES)
            {
                return new ValidationResult(ErrorMessageString);
            }

            return ValidationResult.Success;
        }
    }
}