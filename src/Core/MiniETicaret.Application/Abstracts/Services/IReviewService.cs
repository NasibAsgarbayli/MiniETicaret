using MiniETicaret.Application.DTOs.ReviewDtos;
using MiniETicaret.Application.Shared;

namespace MiniETicaret.Application.Abstracts.Services;

public interface IReviewService
{
    Task<BaseResponse<string>> CreateAsync(ReviewCreateDto dto, string userId);
    Task<BaseResponse<List<ReviewGetDto>>> GetAllByProductIdAsync(Guid productId);
    Task<BaseResponse<string>> DeleteAsync(Guid id);
}
