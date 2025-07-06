using FluentValidation;
using MiniETicaret.Application.DTOs.ReviewDtos;

namespace MiniETicaret.Application.Validations.ReviewValidations;

public class ReviewCreateDtoValidator:AbstractValidator<ReviewCreateDto>
{
    public ReviewCreateDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .MaximumLength(500).WithMessage("Content cannot exceed 500 characters.");
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
    }
}
