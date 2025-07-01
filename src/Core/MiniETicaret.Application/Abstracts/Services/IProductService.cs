using MiniETicaret.Application.DTOs.ProductDtos;
using MiniETicaret.Application.Shared;

namespace MiniETicaret.Application.Abstracts.Services;

public interface IProductService
{
    Task<BaseResponse<string>> AddAsync(ProductCreateDto dto, string userId);
    Task<BaseResponse<string>> DeleteAsync(Guid id, string userId);
    Task<BaseResponse<string>> UpdateAsync(ProductUpdateDto dto, string userId);
    Task<BaseResponse<ProductGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<List<ProductGetDto>>> GetAllAsync();
    Task<BaseResponse<List<ProductGetDto>>> GetAllFilteredAsync(ProductFilterDto filter);
    Task<BaseResponse<List<ProductGetDto>>> GetMyProductsAsync(string userId);

}
