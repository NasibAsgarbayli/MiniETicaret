using FluentValidation;
using MiniETicaret.Application.DTOs.ProductDtos;

namespace MiniETicaret.Application.Validations.ProductValidtions;

public class ProductFilterDtoValidator:AbstractValidator<ProductFilterDto>
{
    public ProductFilterDtoValidator()
    {
        RuleFor(f => f.MinPrice)
        .GreaterThanOrEqualTo(0).When(f => f.MinPrice.HasValue);

        RuleFor(f => f.MaxPrice)
            .GreaterThanOrEqualTo(0).When(f => f.MaxPrice.HasValue);

        RuleFor(f => f.Search)
            .MaximumLength(100).When(f => !string.IsNullOrEmpty(f.Search));

        RuleFor(f => f)
         .Must(f => !f.MinPrice.HasValue || !f.MaxPrice.HasValue || f.MinPrice <= f.MaxPrice)
         .WithMessage("MinPrice MaxPrice-dan böyük ola bilməz.");
    }
}
