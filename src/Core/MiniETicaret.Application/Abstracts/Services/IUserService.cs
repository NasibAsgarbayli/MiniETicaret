using MiniETicaret.Application.DTOs.UserDtos;
using MiniETicaret.Application.Shared;

namespace MiniETicaret.Application.Abstracts.Services;

public interface IUserService
{
    Task<BaseResponse<string>> Register(UserRegisterDto dto);
    Task<BaseResponse<TokenResponse>> Login(UserLoginDto dto);
    Task<BaseResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);


}
