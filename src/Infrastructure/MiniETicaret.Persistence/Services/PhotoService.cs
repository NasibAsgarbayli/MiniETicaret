using System.Net;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MiniETicaret.Application.Abstracts.Services;
using MiniETicaret.Application.Shared;
using MiniETicaret.Application.Shared.Settings;

namespace MiniETicaret.Persistence.Services;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;

    public PhotoService(IOptions<CloudinarySettings> config)
    {
        var acc = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );
        _cloudinary = new Cloudinary(acc);
    }

    public async Task<BaseResponse<string>> UploadAsync(IFormFile file,string folder="products")
    {
        if (file == null || file.Length == 0)
            return new BaseResponse<string>("File is empty", HttpStatusCode.BadRequest);

        try
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == HttpStatusCode.OK)
                return new BaseResponse<string>("Image uploaded successfully", uploadResult.SecureUrl.ToString(), HttpStatusCode.Created);

            return new BaseResponse<string>(uploadResult.Error?.Message ?? "Cloudinary upload failed", HttpStatusCode.InternalServerError);
        }
        catch (Exception ex)
        {
            return new BaseResponse<string>($"Photo upload exception: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<BaseResponse<string>> DeleteAsync(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return new BaseResponse<string>("Image URL is empty", HttpStatusCode.BadRequest);

        try
        {
            var publicId = GetPublicIdFromUrl(imageUrl);
            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);

            if (result.Result == "ok")
                return new BaseResponse<string>("Image deleted successfully", HttpStatusCode.OK);

            return new BaseResponse<string>(result.Error?.Message ?? "Cloudinary delete failed", HttpStatusCode.InternalServerError);
        }
        catch (Exception ex)
        {
            return new BaseResponse<string>($"Photo delete exception: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    // Cloudinary public_id çıxarırıq
    private string GetPublicIdFromUrl(string imageUrl)
    {
        var uri = new Uri(imageUrl);
        var segments = uri.Segments;
        var folder = segments[segments.Length - 2].Trim('/'); // products
        var file = System.IO.Path.GetFileNameWithoutExtension(segments.Last());
        return $"{folder}/{file}";
    }
}

