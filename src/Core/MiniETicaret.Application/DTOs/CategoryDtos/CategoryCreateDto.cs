namespace MiniETicaret.Application.DTOs.CategoryDtos;

public class CategoryCreateDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
}
