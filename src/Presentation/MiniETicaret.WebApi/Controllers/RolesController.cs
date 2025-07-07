using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniETicaret.Application.Abstracts.Services;
using MiniETicaret.Application.DTOs.RoleDtos;
using MiniETicaret.Application.Shared;
using MiniETicaret.Application.Shared.Helpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MiniETicaret.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }


        // GET: api/<RolesController>
        [HttpGet("permissions")]
        [Authorize(Policy = Permissions.Role.GetAllPermissions)]
        public IActionResult GetAllPermissions()
        {
            var permissions = PermissionHelper.GetAllPermissions();
            return Ok(permissions);
        }
        [HttpPost("CreateRole")]
        [Authorize(Policy = Permissions.Role.Create)]
        public async Task<IActionResult> Create(RoleCreateDto dto)
        {
            var result = await _roleService.CreateRole(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Permissions.Role.Update)]
        public async Task<IActionResult> Update(string id, [FromBody] RoleUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var result = await _roleService.UpdateRole(dto);
            return StatusCode((int)result.StatusCode, result);
        }


        [HttpGet("Roles and Their Permissions")]
        [Authorize(Policy = Permissions.Role.GetAllPermissions)]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _roleService.GetAllRolesWithPermissionsAsync();
            return StatusCode((int)result.StatusCode, result);
        }


        [HttpDelete("{roleName}")]
        [Authorize(Policy = Permissions.Role.Delete)]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var result = await _roleService.DeleteRoleAsync(roleName);
            return StatusCode((int)result.StatusCode, result);
        }


    }
}
