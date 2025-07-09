using MiniETicaret.Application.Abstracts.Repositories;
using System.Net;
using MiniETicaret.Application.Abstracts.Services;
using MiniETicaret.Application.DTOs.CategoryDtos;
using MiniETicaret.Application.Shared;
using MiniETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MiniETicaret.Persistence.Contexts;

namespace MiniETicaret.Persistence.Services;
public class CategoryService : ICategoryService
{
    private readonly MiniETicaretDbContext _context;
    public CategoryService(MiniETicaretDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<List<CategoryGetDto>>> GetAllAsync()
    {
        var categories = await _context.Categories
            .Where(c => c.ParentCategoryId == null && !c.IsDeleted)
            .Include(c => c.SubCategories.Where(sc => !sc.IsDeleted))
            .ToListAsync();

        var result = categories.Select(MapCategoryToDto).ToList();
        return new BaseResponse<List<CategoryGetDto>>("Category retrieved succesfuly", result, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<CategoryGetDto>> GetByIdAsync(Guid id)
    {
        var category = await _context.Categories
            .Include(c => c.SubCategories.Where(sc => !sc.IsDeleted))
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (category == null)
            return new BaseResponse<CategoryGetDto>("Category not found", HttpStatusCode.NotFound);

        var dto = MapCategoryToDto(category);
        return new BaseResponse<CategoryGetDto>("Category retrieved succesfuly", dto, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> CreateAsync(CategoryCreateDto dto)
    {
        if (dto.ParentCategoryId.HasValue)
        {
            var parent = await _context.Categories.FindAsync(dto.ParentCategoryId.Value);
            if (parent == null)
                return new BaseResponse<string>("Parent category not found", HttpStatusCode.BadRequest);
        }

        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            ParentCategoryId = dto.ParentCategoryId
        };

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return new BaseResponse<string>("Category created", HttpStatusCode.Created);
    }

    public async Task<BaseResponse<string>> UpdateAsync(CategoryUpdateDto dto)
    {
        var category = await _context.Categories.FindAsync(dto.Id);
        if (category == null)
            return new BaseResponse<string>("Category not found", HttpStatusCode.NotFound);

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.ParentCategoryId = dto.ParentCategoryId;

        _context.Categories.Update(category);
        await _context.SaveChangesAsync();

        return new BaseResponse<string>("Category updated", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> DeleteAsync(Guid id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return new BaseResponse<string>("Category not found", HttpStatusCode.NotFound);

        
        category.IsDeleted = true;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();

        return new BaseResponse<string>("Category deleted", HttpStatusCode.OK);
    }

    private CategoryGetDto MapCategoryToDto(Category category)
    {
        return new CategoryGetDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId, 
            SubCategories = category.SubCategories?.Select(MapCategoryToDto).ToList()
        };
    }
}
