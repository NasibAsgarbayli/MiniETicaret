namespace MiniETicaret.Application.DTOs.CategoryDtos;

public class CategoryGetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public List<CategoryGetDto> SubCategories { get; set; }
}
