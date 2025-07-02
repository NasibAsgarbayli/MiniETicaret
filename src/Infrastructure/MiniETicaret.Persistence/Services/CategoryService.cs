using MiniETicaret.Application.Abstracts.Repositories;
using System.Net;
using MiniETicaret.Application.Abstracts.Services;
using MiniETicaret.Application.DTOs.CategoryDtos;
using MiniETicaret.Application.Shared;
using MiniETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MiniETicaret.Persistence.Services;

public class CategoryService : ICategoryService
{

    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<BaseResponse<string>> AddAsync(CategoryCreateDto dto, string userId)
    {
        var exists = await _categoryRepository
            .GetByFiltered(c => c.Name.Trim().ToLower() == dto.Name.Trim().ToLower())
            .FirstOrDefaultAsync();
        if (exists is not null)
            return new BaseResponse<string>("This category already exists", HttpStatusCode.BadRequest);

        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            ParentCategoryId = dto.ParentCategoryId
        };

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangeAsync();
        return new BaseResponse<string>("Category created", HttpStatusCode.Created);
    }

    public async Task<BaseResponse<List<CategoryGetDto>>> GetAllAsync()
    {
        var categories = await _categoryRepository
            .GetAll()
            .Where(c => c.ParentCategoryId == null)
            .Include(c => c.SubCategories)
            .ToListAsync();

        var dtoList = categories.Select(category => MapCategoryToDtoRecursive(category)).ToList();
        return new BaseResponse<List<CategoryGetDto>>("All categories", dtoList, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> UpdateAsync(CategoryUpdateDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.Id);
        if (category is null)
            return new BaseResponse<string>("Category not found", HttpStatusCode.NotFound);

        var exists = await _categoryRepository
            .GetByFiltered(c => c.Name.Trim().ToLower() == dto.Name.Trim().ToLower() && c.Id != dto.Id)
            .FirstOrDefaultAsync();
        if (exists is not null)
            return new BaseResponse<string>("This category name already exists", HttpStatusCode.BadRequest);

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.ParentCategoryId = dto.ParentCategoryId;

        await _categoryRepository.SaveChangeAsync();
        return new BaseResponse<string>("Category updated successfully", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> DeleteAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category is null)
            return new BaseResponse<string>("Category not found", HttpStatusCode.NotFound);

        if ((category.SubCategories != null && category.SubCategories.Any()) ||
            (category.Products != null && category.Products.Any()))
        {
            return new BaseResponse<string>("You can't delete a category that has subcategories or products!", HttpStatusCode.BadRequest);
        }

        _categoryRepository.Delete(category);
        await _categoryRepository.SaveChangeAsync();
        return new BaseResponse<string>("Category deleted successfully", HttpStatusCode.OK);
    }

    private CategoryGetDto MapCategoryToDtoRecursive(Category category)
    {
        return new CategoryGetDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId,
            SubCategories = category.SubCategories?
                .Select(MapCategoryToDtoRecursive)
                .ToList() ?? new List<CategoryGetDto>()
        };
    }
}
