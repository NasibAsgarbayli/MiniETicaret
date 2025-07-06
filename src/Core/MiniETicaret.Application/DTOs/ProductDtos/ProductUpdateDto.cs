namespace MiniETicaret.Application.DTOs.ProductDtos;

public class ProductUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? Barcode { get; set; }
    public Guid CategoryId { get; set; }
    public string ImageUrl { get; set; }

}
