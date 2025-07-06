using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniETicaret.Application.DTOs.RoleDtos;
using MiniETicaret.Application.Shared;

namespace MiniETicaret.Application.Abstracts.Services;

public interface IRoleService
{
    Task<BaseResponse<string?>> CreateRole(RoleCreateDto dto);
    Task<BaseResponse<string?>> UpdateRole(RoleUpdateDto dto);
    Task<BaseResponse<List<RoleWithPermissionsDto>>> GetAllRolesWithPermissionsAsync();

    Task<BaseResponse<string?>> DeleteRoleAsync(string roleName);




}
