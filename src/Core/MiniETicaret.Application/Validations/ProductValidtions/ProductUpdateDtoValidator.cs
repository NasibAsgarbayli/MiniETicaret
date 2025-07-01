using FluentValidation;
using MiniETicaret.Application.DTOs.ProductDtos;

namespace MiniETicaret.Application.Validations.ProductValidtions;

public class ProductUpdateDtoValidator:AbstractValidator<ProductUpdateDto>
{
    public ProductUpdateDtoValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("Product Id is required.");

        RuleFor(p => p.Title)
            .NotEmpty().WithMessage("Title can not be empty.")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters.");

        RuleFor(p => p.Description)
            .NotEmpty().WithMessage("Description can not be empty.")
            .MinimumLength(5).WithMessage("Description must be at least 5 characters.");

        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(p => p.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required.");
    }
}
