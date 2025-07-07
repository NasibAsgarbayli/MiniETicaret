namespace MiniETicaret.Application.DTOs.ProductDtos;

public class ProductInfoDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public string name { get; set; }
    public string Description { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } 

}
