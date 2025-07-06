using MiniETicaret.Application.DTOs.FavouriteDtos;
using MiniETicaret.Application.Shared;

namespace MiniETicaret.Application.Abstracts.Services;

public interface IFavouriteService
{
    Task<BaseResponse<string>> AddAsync(FavouriteAddDto dto, string userId);
    Task<BaseResponse<string>> RemoveAsync(Guid productId, string userId);
    Task<BaseResponse<List<FavouriteGetDto>>> GetAllAsync(string userId);
}
