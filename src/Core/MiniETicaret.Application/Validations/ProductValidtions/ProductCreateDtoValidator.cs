using FluentValidation;
using MiniETicaret.Application.DTOs.ProductDtos;

namespace MiniETicaret.Application.Validations.ProductValidtions;

public class ProductCreateDtoValidator:AbstractValidator<ProductCreateDto>
{
    public ProductCreateDtoValidator()
    {
       RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Image).NotEmpty();
    }

}
