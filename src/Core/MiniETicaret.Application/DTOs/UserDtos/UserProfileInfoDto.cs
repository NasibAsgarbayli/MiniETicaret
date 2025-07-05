namespace MiniETicaret.Application.DTOs.UserDtos;

public class UserProfileInfoDto
{
    public string Id { get; set; } = null!;
    public string Fullname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
}
