using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MiniETicaret.Application.Abstracts.Services;
using MiniETicaret.Application.DTOs.UserDtos;
using MiniETicaret.Application.Shared;
using MiniETicaret.Application.Shared.Settings;
using MiniETicaret.Domain.Entities;

namespace MiniETicaret.Persistence.Services;

public class UserService : IUserService
{
    private UserManager<AppUser> _userManager { get;  }
    private readonly SignInManager<AppUser> _signInManager;
    private readonly JwtSettings _jwtSetting;
    public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, JwtSettings jwtSetting)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtSetting = jwtSetting;
    }

    public async Task<BaseResponse<string>> Register(UserRegisterDto dto)
    {
        var existedEmail = await _userManager.FindByIdAsync(dto.Email);
        if (existedEmail is not null)
        {
            return new BaseResponse<string>("This account already exist", HttpStatusCode.BadRequest);
        }
        AppUser newUser = new()
        {
            Email = dto.Email,
            Fullname = dto.Fullname,
            UserName=dto.Email,

        };

        IdentityResult identityResult = await _userManager.CreateAsync(newUser, dto.Password);
        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors;
            StringBuilder errorMassege = new();
            foreach (var error in errors)
            {
                errorMassege.Append(error.Description + ";");
            }
            return new(errorMassege.ToString(), HttpStatusCode.BadRequest);
        }
        return new("Succesfuly Created",HttpStatusCode.Created);
    }

    public async Task<BaseResponse<TokenResponse>> Login(UserLoginDto dto)
    {

        var existedEmail = await _userManager.FindByEmailAsync(dto.Email);
        if (existedEmail is null)
        {
            return new("Email or password os wrong.", HttpStatusCode.NotFound);
        }
    

        SignInResult signInResult = await _signInManager.PasswordSignInAsync
            (dto.Email, dto.Password, true, true);
        if (!signInResult.Succeeded)
        {
            return new("Email or password os wrong.", null, HttpStatusCode.NotFound);
        }
        var token = GenerateJwtToken(dto.Email);
        var expires = DateTime.UtcNow.AddMinutes(_jwtSetting.ExpiryMinutes);
        TokenResponse tokenResponse = new()
        {
            Token = token,
            ExpireDate = expires
        };
        return new("Token generated", tokenResponse, HttpStatusCode.OK);
    }



    public string GenerateJwtToken(string userEmail)
    {
        var claims = new[]
        {
        new Claim(ClaimTypes.Email, userEmail),
        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSetting.Issuer,
            audience: _jwtSetting.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSetting.ExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }





}
