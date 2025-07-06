namespace MiniETicaret.Application.DTOs.OrderProductDtos;

public class OrderProductGetDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public int ProductCount { get; set; }
    public decimal ProductPrice { get; set; }

}
