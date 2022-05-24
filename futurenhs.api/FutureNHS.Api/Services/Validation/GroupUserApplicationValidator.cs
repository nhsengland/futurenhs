using FluentValidation;
using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.Services.Validation
{
    public sealed class GroupUserApplicationValidator : AbstractValidator<GroupUserDto>
    {
        public GroupUserApplicationValidator()
        {
            RuleFor(model => model.Approved)  
                .Equal(false)
                .WithMessage("This user has already been approved");

            RuleFor(model => model.Rejected)
                .Equal(false)
                .WithMessage("This user has already been rejected");

            RuleFor(model => model.Banned)
                .Equal(false)
                .WithMessage("This user is banned");

            RuleFor(model => model.Locked)
                .Equal(false)
                .WithMessage("This user is locked");
        }
    }
}
