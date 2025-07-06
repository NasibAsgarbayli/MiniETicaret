using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniETicaret.Application.Abstracts.Services;
using MiniETicaret.Application.DTOs.ReviewDtos;
using MiniETicaret.Application.Shared;
using System.Net;

namespace MiniETicaret.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _service;

        public ReviewsController(IReviewService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromBody] ReviewCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.CreateAsync(dto, userId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Moderator")]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(BaseResponse<List<ReviewGetDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllByProductId(Guid productId)
        {
            var result = await _service.GetAllByProductIdAsync(productId);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
