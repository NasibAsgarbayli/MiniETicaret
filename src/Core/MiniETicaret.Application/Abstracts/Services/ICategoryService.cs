using MiniETicaret.Application.DTOs.CategoryDtos;
using MiniETicaret.Application.Shared;

namespace MiniETicaret.Application.Abstracts.Services;

public interface ICategoryService
{
    Task<BaseResponse<string>> AddAsync(CategoryCreateDto dto, string userId);
    Task<BaseResponse<List<CategoryGetDto>>> GetAllAsync();
    Task<BaseResponse<string>> UpdateAsync(CategoryUpdateDto dto);
    Task<BaseResponse<string>> DeleteAsync(Guid id);

}
