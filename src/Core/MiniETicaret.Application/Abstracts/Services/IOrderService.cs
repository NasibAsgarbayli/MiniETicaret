using MiniETicaret.Application.DTOs.OrderDtos;
using MiniETicaret.Application.Shared;

namespace MiniETicaret.Application.Abstracts.Services;

public interface IOrderService
{
    Task<BaseResponse<Guid>> CreateAsync(OrderCreateDto dto, string buyerId);
    Task<BaseResponse<List<OrderGetDto>>> GetMyOrdersAsync(string buyerId);
    Task<BaseResponse<List<OrderGetDto>>> GetMySalesAsync(string sellerId);
    Task<BaseResponse<OrderGetDto>> GetByIdAsync(Guid id, string userId, string role);
    Task<BaseResponse<string>> UpdateStatusAsync(Guid id, string status, string userId, string role);
    Task<BaseResponse<string>> DeleteAsync(Guid id, string userId, string role);
    Task<BaseResponse<List<OrderGetDto>>> GetAllAsync(string role);
}
