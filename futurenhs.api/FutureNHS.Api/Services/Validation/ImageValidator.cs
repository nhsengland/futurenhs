using FluentValidation;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.Helpers;

namespace FutureNHS.Api.Services.Validation
{
    public sealed class ImageValidator : AbstractValidator<ImageDto>
    {
        public ImageValidator(long maxImageSize)
        {
            RuleFor(model => model.FileSizeBytes)
                .LessThanOrEqualTo(maxImageSize)
                .WithMessage($"Image must be smaller than {BytesToReadable.BytesToString(maxImageSize)}");
        }
    }
}
