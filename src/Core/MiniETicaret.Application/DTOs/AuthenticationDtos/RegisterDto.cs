using MiniETicaret.Domain.Enums;

namespace MiniETicaret.Application.DTOs.AuthenticationDtos;

public class RegisterDto
{
    public string Fullname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public UserRole Role { get; set; }
}
