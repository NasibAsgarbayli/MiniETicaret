using FluentValidation;
using MiniETicaret.Application.DTOs.FavouriteDtos;

namespace MiniETicaret.Application.Validations.FavuirteValidations;

public class FavouriteAddDtoValidator:AbstractValidator<FavouriteAddDto>
{
    public FavouriteAddDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
    }
}
