using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MiniETicaret.Application.Abstracts.Services;
using MiniETicaret.Application.DTOs.UserDtos;
using MiniETicaret.Application.Shared;
using MiniETicaret.Application.Shared.Settings;
using MiniETicaret.Domain.Entities;

namespace MiniETicaret.Persistence.Services;

public class UserService : IUserService

{
    private UserManager<AppUser> _userManager { get; }

    private readonly RoleManager<IdentityRole> _roleManager;
    public UserService(UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager)
    
    {
        _userManager = userManager;
        _roleManager = roleManager;

    }

    public async Task<BaseResponse<string>> AddRole(UserAddRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
        if (user is null)
        {
            return new BaseResponse<string>("User not found", HttpStatusCode.NotFound);
        }

        var roleNames = new List<string>();

        foreach (var roleId in dto.RoleId.Distinct())
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role is null)
            {
                return new BaseResponse<string>($"Role With Id'{roleId}' not found", HttpStatusCode.NotFound);
            }

            if (!await _userManager.IsInRoleAsync(user, role.Name!))
            {
                var result = await _userManager.AddToRoleAsync(user, role.Name!);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new BaseResponse<string>($"Failed to add role '{role.Name}'to user:{errors}", HttpStatusCode.BadRequest);
                }
                roleNames.Add(role.Name!);
            }


        }
        return new BaseResponse<string>($"Succesfuly added roles:{string.Join(", ", roleNames)}", HttpStatusCode.OK);

    }


    public async Task<BaseResponse<List<UserGetDto>>> GetAllAsync()
    {
        var users = _userManager.Users.Select(u => new UserGetDto
        {
            Id = u.Id,
            FullName = u.Fullname,
            Email = u.Email,
          
        }).ToList();

        return new BaseResponse<List<UserGetDto>>("All Users:",users, HttpStatusCode.OK);

    }

    public async Task<BaseResponse<UserGetDto>> GetByIdAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            return new BaseResponse<UserGetDto>("User not found", HttpStatusCode.NotFound);

        var dto = new UserGetDto
        {
            Id = user.Id,
            FullName = user.Fullname,
            Email = user.Email,
      
        };

        return new BaseResponse<UserGetDto>("User Info:", dto, HttpStatusCode.OK);
    }




}
