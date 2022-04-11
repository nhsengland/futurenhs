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
                .WithMessage("Enter the discussion title");

            RuleFor(model => model.Title)
                .MaximumLength(100)
                .WithMessage("Enter 100 or fewer characters");

            RuleFor(model => model.Content)
                .NotEmpty()
                .WithMessage("Enter the discussion comment");

            RuleFor(model => model.Content)
                .MaximumLength(100000)
                .WithMessage("Enter 100,000 or fewer characters");
        }
    }
}
