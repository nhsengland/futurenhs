using FluentValidation;
using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.Services.Validation
{
    public class UserValidator : AbstractValidator<MemberDto>
    {
        public UserValidator()
        {

            RuleFor(model => model.FirstName)
                    .NotEmpty()
                    .WithMessage("Enter the first name");

            RuleFor(model => model.Surname)
                    .NotEmpty()
                    .WithMessage("Enter the last name");
        }
    }
}

