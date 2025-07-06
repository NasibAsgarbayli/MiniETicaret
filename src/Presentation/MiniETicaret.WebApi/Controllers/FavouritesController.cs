using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniETicaret.Application.Abstracts.Services;
using MiniETicaret.Application.DTOs.FavouriteDtos;
using MiniETicaret.Application.Shared;
using System.Net;

namespace MiniETicaret.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavouritesController : ControllerBase
    {
        private readonly IFavouriteService _service;

        public FavouritesController(IFavouriteService service)
        {
            _service = service;
        }

     
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Add([FromBody] FavouriteAddDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.AddAsync(dto, userId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{productId}")]
        [Authorize]
        public async Task<IActionResult> Remove(Guid productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.RemoveAsync(productId, userId);
            return StatusCode((int)result.StatusCode, result);
        }

      
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(BaseResponse<List<FavouriteGetDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.GetAllAsync(userId);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
