﻿using FluentValidation.Results;

namespace FutureNHS.Api.Exceptions
{
    public sealed class ValidationException : ApplicationException
    {
        public Dictionary<string, string> Errors { get; set; }

        public ValidationException(ValidationResult validationResult)
        {
            Errors = new Dictionary<string, string>();

            foreach (var validationError in validationResult.Errors)
            {
                Errors.Add(validationError.PropertyName, validationError.ErrorMessage);
            }
        }
    }
}
