using FluentValidation;
using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.Services.Validation
{
    public sealed class DiscussionValidator : AbstractValidator<DiscussionDto>
    {
        public DiscussionValidator()
        {
            RuleFor(model => model.Title)
                .NotEmpty()
                .WithMessage("Enter a discussion title");

            RuleFor(model => model.Title)
                .MaximumLength(200)
                .WithMessage("Discussion title must be 200 characters or fewer");

            RuleFor(model => model.Content)
                .NotEmpty()
                .WithMessage("Enter the discussion comment");
        }
    }
}
