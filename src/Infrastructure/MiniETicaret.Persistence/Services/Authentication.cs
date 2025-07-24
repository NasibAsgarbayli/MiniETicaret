using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Text;
using MiniETicaret.Application.Abstracts.Services;
using MiniETicaret.Application.DTOs.AuthenticationDtos;
using MiniETicaret.Application.DTOs.UserDtos;
using MiniETicaret.Application.Shared;
using MiniETicaret.Domain.Entities;
using MiniETicaret.Infrastructure.Services;
using Microsoft.IdentityModel.Tokens;
using MiniETicaret.Application.Shared.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Web;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using MiniETicaret.Application.DTOs.ProductDtos;
using MiniETicaret.Persistence.Contexts;
using Hangfire;
using Microsoft.AspNetCore.Http;

namespace MiniETicaret.Persistence.Services;

public class Authentication : IAuthentication

{

    private UserManager<AppUser> _userManager { get; }
    private readonly SignInManager<AppUser> _signInManager;
    private readonly JwtSettings _jwtSetting;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEmailService _mailService;
    private readonly MiniETicaretDbContext _context;
    private readonly IJobService _jobService;
    private readonly IPhotoService _photoService;

    public Authentication(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IOptions<JwtSettings> jwtSetting, RoleManager<IdentityRole> roleManager, IEmailService mailService, MiniETicaretDbContext context, IJobService jobService,IPhotoService photoService)

    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtSetting = jwtSetting.Value;
        _roleManager = roleManager;
        _mailService = mailService;
        _context = context;
        _jobService = jobService;
        _photoService = photoService;
    }

    public async Task<BaseResponse<ProfilInfoDto>> GetProfileAsync(ClaimsPrincipal userPrincipal)
    {
        var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return new("Unauthorized", null, HttpStatusCode.Unauthorized);

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new("User not found", null, HttpStatusCode.NotFound);

        var roles = await _userManager.GetRolesAsync(user);

        List<ProductInfoDto>? products = null;
        if (roles.Contains("Seller"))
        {
            products = await _context.Products
                .Where(p => p.UserId == user.Id)
                .Include(p => p.Category)
                .Select(p => new ProductInfoDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Price = p.Price,
                    name = p.Name,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.Name : null
                })
                .ToListAsync();
        }

        var profile = new ProfilInfoDto
        {
            Id = user.Id,
            Fullname = user.Fullname,
            Email = user.Email,
            Roles = roles.ToList(),
            Products = products // seller deyilsə, null qalacaq
        };

        return new("User profile fetched successfully", profile, HttpStatusCode.OK);
    }


    public async Task<BaseResponse<TokenResponse>> Login(LoginDto dto)
    {

        var existedUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existedUser is null)
        {
            return new("Email or password os wrong.", HttpStatusCode.NotFound);
        }
        if (!existedUser.EmailConfirmed)
        {
            return new("Please Confirm your email", HttpStatusCode.BadRequest);
        }

        SignInResult signInResult = await _signInManager.PasswordSignInAsync
            (dto.Email, dto.Password, true, true);
        if (!signInResult.Succeeded)
        {
            return new("Email or password os wrong.", null, HttpStatusCode.NotFound);
        }
        var token = await GenerateTokensAsync(existedUser);
        return new("Token generated", token, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> Register(RegisterDto dto)
    {

        var existedEmail = await _userManager.FindByEmailAsync(dto.Email);
        if (existedEmail is not null)
        {
            return new BaseResponse<string>("This account already exist", HttpStatusCode.BadRequest);
        }
        AppUser newUser = new()
        {
            Email = dto.Email,
            Fullname = dto.Fullname,
            UserName = dto.Email,

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
        var roleName = dto.Role.ToString();
        await _userManager.AddToRoleAsync(newUser, roleName);
        string confirmEmailLink = await GetEmailConfirmLink(newUser);

        BackgroundJob.Enqueue<IJobService>(job =>
                job.SendEmailAsync(
         new List<string> { newUser.Email },
         "Email Confirmation",
         confirmEmailLink
                )
        );





        return new("Succesfuly Created", HttpStatusCode.Created);
    }

    public async Task<BaseResponse<string>> ConfirmEmail(string userId, string token)
    {
        var existedUser = await _userManager.FindByIdAsync(userId);
        if (existedUser is null)
        {
            return new("Email confirmation failed", HttpStatusCode.NotFound);
        }

        token = HttpUtility.UrlDecode(token);
        var result = await _userManager.ConfirmEmailAsync(existedUser, token);
        if (!result.Succeeded)
        {
            return new("Email confirmation failed", HttpStatusCode.BadRequest);
        }
        return new("Email confirmed successfully", null, HttpStatusCode.OK);
    }


    private async Task<TokenResponse> GenerateTokensAsync(AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSetting.SecretKey);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!)
        };
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var roleName in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleName));

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                var permissionClaims = roleClaims.Where(c => c.Type == "Permission").Distinct();

                foreach (var permissionClaim in permissionClaims)
                {
                    claims.Add(new Claim("Permission", permissionClaim.Value));
                }
            }
        }
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSetting.ExpiryMinutes),
            Issuer = _jwtSetting.Issuer,
            Audience = _jwtSetting.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);


        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiryDate = DateTime.UtcNow.AddHours(2);
        user.RefreshToken = refreshToken;
        user.ExpireDate = refreshTokenExpiryDate;
        await _userManager.UpdateAsync(user);

        return new TokenResponse
        {
            Token = jwt,
            RefreshToken = refreshToken,
            ExpireDate = tokenDescriptor.Expires!.Value,
        };
    }


    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false, // expired token üçün bu false olmalıdır

            ValidIssuer = _jwtSetting.Issuer,
            ValidAudience = _jwtSetting.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.SecretKey))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is JwtSecurityToken jwtSecurityToken &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return principal;
            }
        }
        catch
        {
            return null;
        }

        return null;
    }

    public async Task<BaseResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var principal = GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
            return new("Invalid access token", null, HttpStatusCode.BadRequest);

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userManager.FindByIdAsync(userId!);

        if (user == null)
            return new("User not faund", null, HttpStatusCode.NotFound);



        if (user.RefreshToken is null || user.RefreshToken != request.RefreshToken ||
            user.ExpireDate < DateTime.UtcNow)
            return new("Invalid refresh token", null, HttpStatusCode.BadRequest);


        //Generate new tokens
        var tokenResponse = await GenerateTokensAsync(user);
        return new("Refreshed", tokenResponse, HttpStatusCode.OK);
    }

    private async Task<string> GetEmailConfirmLink(AppUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var link = $"https://localhost:7235/api/Authentication/ConfirmEmail?userId={user.Id}&token={HttpUtility.UrlEncode(token)}";
        Console.WriteLine("Confirm Email Link: " + link);
        return link;

    }


    public async Task<BaseResponse<string>> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return new("User not found", false, HttpStatusCode.NotFound);

        if (!user.EmailConfirmed)
            return new("Please confirm your email", false, HttpStatusCode.BadRequest);

        // Tokeni decode et, çünki linkdə URL-encoded olur
        var decodedToken = HttpUtility.UrlDecode(dto.Token);

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new($"Failed to reset password: {errors}", false, HttpStatusCode.BadRequest);
        }

        return new("Password reset successfully", true, HttpStatusCode.OK);
    }


    public async Task<BaseResponse<string>> SendResetPasswordEmail(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return new BaseResponse<string>("User not found", false, HttpStatusCode.NotFound);
        }

        // Token və link yaratmaq üçün hazır metoddan istifadə
        var resetLink = await GetEmailResetConfirm(user);


        BackgroundJob.Enqueue<IJobService>(job =>
                    job.SendEmailAsync(
           new List<string> { user.Email },
           "Reset Password",
           resetLink
                    )
         );




        return new BaseResponse<string>("Email confirmed successfully", true, HttpStatusCode.OK);
    }
    private async Task<string> GetEmailResetConfirm(AppUser user)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var link = $"https://localhost:7045/api/Accounts/SendResetConfirmEmail?email={user.Email}&token={HttpUtility.UrlEncode(token)}";
        Console.WriteLine("Reset Password Link : " + link);
        return link;
    }

    public async Task<BaseResponse<string>> UploadProfilePhotoAsync(ClaimsPrincipal userPrincipal, IFormFile file)
    {
        var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return new("Unauthorized", HttpStatusCode.Unauthorized);

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new("User not found", HttpStatusCode.NotFound);

        if (file == null || file.Length == 0)
            return new("File is empty", HttpStatusCode.BadRequest);

        var uploadResult = await _photoService.UploadAsync(file, "profiles");


        if (!uploadResult.Success)
            return new(uploadResult.Message, HttpStatusCode.InternalServerError);

        user.ProfileImageUrl = uploadResult.Data;
        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            return new($"Failed to update user: {errors}", HttpStatusCode.InternalServerError);
        }

        return new("Profile photo uploaded!", user.ProfileImageUrl, HttpStatusCode.OK);
    }


    public async Task<BaseResponse<string>> DeleteProfilePhotoAsync(ClaimsPrincipal userPrincipal)
    {
        var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return new("Unauthorized", HttpStatusCode.Unauthorized);

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new("User not found", HttpStatusCode.NotFound);

        if (string.IsNullOrEmpty(user.ProfileImageUrl))
            return new("No profile image to delete", HttpStatusCode.BadRequest);

        var deleteResult = await _photoService.DeleteAsync(user.ProfileImageUrl);
        if (!deleteResult.Success)
            return new(deleteResult.Message, HttpStatusCode.InternalServerError);

        user.ProfileImageUrl = null;
        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            return new($"Failed to update user: {errors}", HttpStatusCode.InternalServerError);
        }

        return new("Profile photo deleted successfully!", (string?)null, HttpStatusCode.OK);
    }



}
