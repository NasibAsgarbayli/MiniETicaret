using System.Security.Claims;
using MiniETicaret.Application.DTOs.UserDtos;
using MiniETicaret.Application.Shared;

namespace MiniETicaret.Application.Abstracts.Services;

public interface IUserService
{
  
    Task<BaseResponse<string>> AddRole(UserAddRoleDto dto);
    Task<BaseResponse<List<UserGetDto>>> GetAllAsync();
    Task<BaseResponse<UserGetDto>> GetByIdAsync(Guid id);



}
