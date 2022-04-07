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
                .WithMessage("Enter the group name");

            RuleFor(model => model.Name)
                .MaximumLength(255)
                .WithMessage("Enter 255 or fewer characters");

            RuleFor(model => model.StrapLine)
                .MaximumLength(1000)
                .WithMessage("Enter 1000 or fewer characters");

            RuleFor(model => model.GroupOwnerId)
                .NotNull()
                .WithMessage("No Group Owner provided");
        }
    }
}
