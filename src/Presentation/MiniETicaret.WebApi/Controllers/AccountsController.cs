﻿using System.Net;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniETicaret.Application.Abstracts.Services;
using MiniETicaret.Application.DTOs.UserDtos;
using MiniETicaret.Application.Shared;



namespace MiniETicaret.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IUserService _userService{ get; }
        public AccountsController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPost("assign-roles")]
        [Authorize(Policy =Permissions.Account.AddRole)]
        [ProducesResponseType(typeof(BaseResponse<TokenResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddRole([FromBody] UserAddRoleDto dto)
        {
            var result = await _userService.AddRole(dto);
            return StatusCode((int)result.StatusCode, result);
        }



        [HttpGet]
        [Authorize(Policy =Permissions.User.GetAll)]
        [ProducesResponseType(typeof(BaseResponse<List<UserGetDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return StatusCode((int)result.StatusCode, result);
        }


        [HttpGet("{id}")]
        [Authorize(Policy =Permissions.User.GetById)]
        [ProducesResponseType(typeof(BaseResponse<UserGetDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _userService.GetByIdAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }

    }
}
