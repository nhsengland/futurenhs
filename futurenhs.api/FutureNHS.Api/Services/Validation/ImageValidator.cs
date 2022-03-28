using FluentValidation;
using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.Services.Validation
{
    public sealed class ImageValidator : AbstractValidator<ImageDto>
    {
        public ImageValidator()
        {
            RuleFor(model => model.FileSizeBytes)
                .LessThanOrEqualTo(500000)
                .WithMessage("Group logo must be smaller than 500KB");
        }
    }
}
