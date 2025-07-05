using Microsoft.AspNetCore.Http;

namespace MiniETicaret.Application.Abstracts.Services;

public interface IFileService
{
    Task<string> UploadAsync(IFormFile file);
}
