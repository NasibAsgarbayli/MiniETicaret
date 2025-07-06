using FluentValidation;
using MiniETicaret.Application.DTOs.OrderProductDtos;

namespace MiniETicaret.Application.Validations.OrderValidations;

public class OrderProductCreateDtoValidator:AbstractValidator<OrderProductCreateDto>
{
    public OrderProductCreateDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.ProductCount).GreaterThan(0);
    }
}
