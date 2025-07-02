using FluentValidation;
using MiniETicaret.Application.DTOs.UserDtos;

namespace MiniETicaret.Application.Validations.UserValidations;

public class UserLoginDtoValidator:AbstractValidator<UserLoginDto>
{
    public UserLoginDtoValidator()
    {

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email cannot be empty.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password cannot be empty.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

    }
}
