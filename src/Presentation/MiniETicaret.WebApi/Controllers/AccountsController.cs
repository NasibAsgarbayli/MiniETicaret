using System.Net;
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
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            var result = await _userService.Register(dto);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<TokenResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var result = await _userService.Login(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<TokenResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest dto)
        {
            var result = await _userService.RefreshTokenAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }


        [HttpPost("assign-roles")]
        [ProducesResponseType(typeof(BaseResponse<TokenResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddRole([FromBody] UserAddRoleDto dto)
        {
            var result = await _userService.AddRole(dto);
            return StatusCode((int)result.StatusCode, result);
        }

    }
}
