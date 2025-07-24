using Microsoft.AspNetCore.Http;
using MiniETicaret.Application.Shared;

namespace MiniETicaret.Application.Abstracts.Services;

public interface IPhotoService
{
    Task<BaseResponse<string>> UploadAsync(IFormFile file,string folder="products");
    Task<BaseResponse<string>> DeleteAsync(string imageUrl);

}
