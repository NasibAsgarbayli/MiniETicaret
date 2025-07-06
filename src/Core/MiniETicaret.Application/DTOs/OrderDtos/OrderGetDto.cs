using MiniETicaret.Application.DTOs.OrderProductDtos;

namespace MiniETicaret.Application.DTOs.OrderDtos;

public class OrderGetDto
{
    public Guid Id { get; set; }
    public decimal? TotalPrice { get; set; }
    public string? Note { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime OrderDate { get; set; }
    public string? Status { get; set; }
    public List<OrderProductGetDto> Products { get; set; }
}
