using MiniETicaret.Application.DTOs.ImageDtos;

namespace MiniETicaret.Application.DTOs.ProductDtos;

public class ProductGetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? Barcode { get; set; }
    public string ImageUrl { get; set; }
    public string SellerFullName { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public double? AverageRating { get; set; }
    public bool IsActive { get; set; }
    public List<ImageGetDto> Images { get; set; }
}
