using MiniETicaret.Application.DTOs.OrderProductDtos;

namespace MiniETicaret.Application.DTOs.OrderDtos;

public class OrderCreateDto
{
    public List<OrderProductCreateDto> Products { get; set; }
    public string? Note { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? PaymentMethod { get; set; }
}
