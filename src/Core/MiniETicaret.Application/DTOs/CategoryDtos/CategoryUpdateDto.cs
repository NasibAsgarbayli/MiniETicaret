namespace MiniETicaret.Application.DTOs.CategoryDtos;

public class CategoryUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
}
