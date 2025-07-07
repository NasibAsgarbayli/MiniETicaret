using MiniETicaret.Application.DTOs.ProductDtos;

namespace MiniETicaret.Application.DTOs.AuthenticationDtos;

public class ProfilInfoDto
{
    public string Id { get; set; } = null!;
    public string Fullname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
    public List<ProductInfoDto>? Products { get; set; } // Əlavə olundu
}
