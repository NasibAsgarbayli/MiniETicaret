using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniETicaret.Application.Abstracts.Services;
using MiniETicaret.Application.DTOs.ProductDtos;
using MiniETicaret.Application.Shared;
using MiniETicaret.Application.DTOs.ImageDtos;

namespace MiniETicaret.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(BaseResponse<List<ProductGetDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll([FromQuery] Guid? categoryId, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] string search = null)
        {
            var filter = new ProductFilterDto
            {
                CategoryId = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Search = search
            };
            var result = await _service.GetAllAsync(filter);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(BaseResponse<ProductGetDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "Seller,Admin,Moderator")]
        [ProducesResponseType(typeof(BaseResponse<Guid>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.CreateAsync(dto, userId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Seller,Admin,Moderator")]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductUpdateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            dto.Id = id;
            var result = await _service.UpdateAsync(dto, userId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller,Admin,Moderator")]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.DeleteAsync(id, userId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Seller,Admin,Moderator")]
        [ProducesResponseType(typeof(BaseResponse<List<ProductGetDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMy()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.GetMyProductsAsync(userId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("{productId}/images")]
        [Authorize(Roles = "Seller,Admin,Moderator")]
        public async Task<IActionResult> AddImage(Guid productId, [FromBody] ImageAddDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.AddImageAsync(productId, dto, userId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{productId}/images/{imageId}")]
        [Authorize(Roles = "Seller,Admin,Moderator")]
        public async Task<IActionResult> DeleteImage(Guid productId, Guid imageId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.DeleteImageAsync(productId, imageId, userId);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
