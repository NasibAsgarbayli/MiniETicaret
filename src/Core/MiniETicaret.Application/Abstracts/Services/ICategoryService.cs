using MiniETicaret.Application.DTOs.CategoryDtos;
using MiniETicaret.Application.Shared;

namespace MiniETicaret.Application.Abstracts.Services;

public interface ICategoryService
{

    Task<BaseResponse<List<CategoryGetDto>>> GetAllAsync();
    Task<BaseResponse<CategoryGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<string>> CreateAsync(CategoryCreateDto dto);
    Task<BaseResponse<string>> UpdateAsync(CategoryUpdateDto dto);
    Task<BaseResponse<string>> DeleteAsync(Guid id);

}
