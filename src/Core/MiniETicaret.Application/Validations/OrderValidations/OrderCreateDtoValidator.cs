using FluentValidation;
using MiniETicaret.Application.DTOs.OrderDtos;

namespace MiniETicaret.Application.Validations.OrderValidations;

public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
{
    public OrderCreateDtoValidator()
    {
        RuleFor(x => x.Products).NotNull().NotEmpty();
        RuleForEach(x => x.Products).SetValidator(new OrderProductCreateDtoValidator());
        RuleFor(x => x.DeliveryAddress).NotEmpty();
        RuleFor(x => x.PaymentMethod).NotEmpty();
    }
}
