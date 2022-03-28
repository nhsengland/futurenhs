using FluentValidation;
using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.Services.Validation
{
    public sealed class GroupValidator : AbstractValidator<GroupDto>
    {
        public GroupValidator()
        {
            RuleFor(model => model.Name)
                .NotEmpty()
                .WithMessage("Enter a group name");

            RuleFor(model => model.Name)
                .MaximumLength(255)
                .WithMessage("Group name must be 200 characters or fewer");

            RuleFor(model => model.StrapLine)
                .MaximumLength(1000)
                .WithMessage("Strap line must be 1000 characters or fewer");
        }
    }
}
