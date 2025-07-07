using Microsoft.AspNetCore.Http;

namespace MiniETicaret.Application.DTOs.ImageDtos;

public class ImageAddDto
{
    public IFormFile Image { get; set; }

}
