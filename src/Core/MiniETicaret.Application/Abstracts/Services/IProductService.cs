using MiniETicaret.Application.DTOs.ImageDtos;
using MiniETicaret.Application.DTOs.ProductDtos;
using MiniETicaret.Application.Shared;

namespace MiniETicaret.Application.Abstracts.Services;

public interface IProductService
{
    Task<BaseResponse<Guid>> CreateAsync(ProductCreateDto dto, string sellerId);
    Task<BaseResponse<string>> UpdateAsync(ProductUpdateDto dto, string sellerId);
    Task<BaseResponse<string>> DeleteAsync(Guid id, string sellerId);
    Task<BaseResponse<ProductGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<List<ProductGetDto>>> GetAllAsync(ProductFilterDto filter = null);
    Task<BaseResponse<List<ProductGetDto>>> GetMyProductsAsync(string sellerId);
    Task<BaseResponse<string>> AddImageAsync(Guid productId, ImageAddDto dto, string sellerId);
    Task<BaseResponse<string>> DeleteImageAsync(Guid productId, Guid imageId, string sellerId);

}
