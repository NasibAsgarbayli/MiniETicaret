using Microsoft.AspNetCore.Http;

namespace MiniETicaret.Application.DTOs.FileUploadDtos;

public class FileUploadDto
{
    public IFormFile File { get; set; } = null!;

}
