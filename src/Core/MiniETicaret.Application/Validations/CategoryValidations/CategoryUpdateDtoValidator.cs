using FluentValidation;
using MiniETicaret.Application.DTOs.CategoryDtos;

namespace MiniETicaret.Application.Validations.CategoryValidations;

public class CategoryUpdateDtoValidator:AbstractValidator<CategoryUpdateDto>
{
	public CategoryUpdateDtoValidator()
	{
        RuleFor(c => c.Id)
           .NotEmpty().WithMessage("Id is required.");

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name can not be empty.")
            .MinimumLength(3).WithMessage("Name should be at least 3 characters.");

        RuleFor(c => c.Description)
            .MaximumLength(200)
            .When(c => !string.IsNullOrWhiteSpace(c.Description))
            .WithMessage("Description must be max 200 characters.");
    }
}
