namespace MiniETicaret.Application.DTOs.ProductDtos;

public class ProductFilterDto
{
    public Guid? CategoryId { get; set; }
    public int? MinPrice { get; set; }
    public int? MaxPrice { get; set; }
    public string? Search { get; set; }

}
