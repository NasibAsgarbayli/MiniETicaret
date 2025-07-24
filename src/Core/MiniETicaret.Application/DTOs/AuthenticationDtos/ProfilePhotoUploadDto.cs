using Microsoft.AspNetCore.Http;

namespace MiniETicaret.Application.DTOs.AuthenticationDtos;

public class ProfilePhotoUploadDto
{
    public IFormFile Image { get; set; }
}
