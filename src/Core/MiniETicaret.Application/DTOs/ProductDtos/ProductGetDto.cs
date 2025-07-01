namespace MiniETicaret.Application.DTOs.ProductDtos;

public class ProductGetDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; } 
    public string? ImageUrl { get; set; }

}
