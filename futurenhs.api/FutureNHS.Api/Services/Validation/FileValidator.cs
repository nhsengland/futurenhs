using FluentValidation;
using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.Services.Validation
{
    public sealed class FileValidator : AbstractValidator<FileDto>
    {
        public FileValidator()
        {
            RuleFor(model => model.Title)
                .NotEmpty()
                .WithMessage("Enter a file title");

            RuleFor(model => model.Title)
                .MaximumLength(45)
                .WithMessage("The file title must be 45 characters or fewer");

            RuleFor(model => model.Description)
                .MaximumLength(150)
                .WithMessage("The file description must be 150 characters or fewer");

            RuleFor(model => model.FileSizeBytes)
                .LessThanOrEqualTo(262144000)
                .WithMessage("The file must be smaller than 250MB");
        }
    }
}
