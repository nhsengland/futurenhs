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
                .WithMessage("Enter the file title");

            RuleFor(model => model.Title)
                .MaximumLength(45)
                .WithMessage("Enter 45 or fewer characters");

            RuleFor(model => model.Description)
                .MaximumLength(150)
                .WithMessage("Enter 150 or fewer characters");

            RuleFor(model => model.FileSizeBytes)
                .LessThanOrEqualTo(262144000)
                .WithMessage("The file must be smaller than 250MB");

            RuleFor(model => model.FileSizeBytes)
                .GreaterThan(0)
                .WithMessage("Add a file");            
        }
    }
}
