using System.Security.Claims;
using MiniETicaret.Application.DTOs.AuthenticationDtos;
using MiniETicaret.Application.DTOs.UserDtos;
using MiniETicaret.Application.Shared;

namespace MiniETicaret.Application.Abstracts.Services;

public interface IAuthentication
{
    Task<BaseResponse<string>> Register(RegisterDto dto);
    Task<BaseResponse<TokenResponse>> Login(LoginDto dto);

    Task<BaseResponse<ProfilInfoDto>> GetProfileAsync(ClaimsPrincipal userPrincipal);

    Task<BaseResponse<string>> ConfirmEmail(string userId, string token);

    Task<BaseResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);

    Task<BaseResponse<string>> SendResetPasswordEmail(string email);

    Task<BaseResponse<string>> ResetPassword(ResetPasswordDto dto);

}
