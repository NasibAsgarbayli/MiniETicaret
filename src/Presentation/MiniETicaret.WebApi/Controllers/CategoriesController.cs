using Microsoft.AspNetCore.Authorization;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniETicaret.Application.DTOs.CategoryDtos;
using MiniETicaret.Application.Shared;
using MiniETicaret.Application.Abstracts.Services;

namespace MiniETicaret.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;
        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        // GET /api/categories
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<List<CategoryGetDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return StatusCode((int)result.StatusCode, result);
        }

        // GET /api/categories/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseResponse<CategoryGetDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }

        // POST /api/categories
        [HttpPost]
        [Authorize(Policy =Permissions.Category.MainCreate)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        // PUT /api/categories
        [HttpPut]
        [Authorize(Policy =Permissions.Category.Update)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Update([FromBody] CategoryUpdateDto dto)
        {
            var result = await _service.UpdateAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        // DELETE /api/categories/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = Permissions.Category.Delete)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
