using MiniETicaret.Application.Abstracts.Repositories;
using System.Net;
using MiniETicaret.Application.Abstracts.Services;
using MiniETicaret.Application.DTOs.ProductDtos;
using MiniETicaret.Application.Shared;
using MiniETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MiniETicaret.Persistence.Services;
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<BaseResponse<string>> AddAsync(ProductCreateDto dto, string userId)
    {
        // Eyni adda məhsul varsa, error qaytar
        var exists = await _productRepository
            .GetByFiltered(p => p.Title.Trim().ToLower() == dto.Title.Trim().ToLower() && p.UserId == userId)
            .FirstOrDefaultAsync();

        if (exists is not null)
            return new BaseResponse<string>("Bu adda məhsul artıq mövcuddur", HttpStatusCode.BadRequest);

        var product = new Product
        {
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            CategoryId = dto.CategoryId,
            UserId = userId,
            // Əgər ImageUrl varsa, burdan əlavə Image entity yaradıla bilər
        };

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangeAsync();
        return new BaseResponse<string>("Məhsul uğurla əlavə olundu", HttpStatusCode.Created);
    }

    // 2. Məhsulu sil (yalnız sahibi)
    public async Task<BaseResponse<string>> DeleteAsync(Guid id, string userId)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
            return new BaseResponse<string>("Məhsul tapılmadı", HttpStatusCode.NotFound);

        if (product.UserId != userId)
            return new BaseResponse<string>("Bu məhsulu silmək hüququnuz yoxdur", HttpStatusCode.Forbidden);

        _productRepository.Delete(product);
        await _productRepository.SaveChangeAsync();

        return new BaseResponse<string>("Məhsul silindi", HttpStatusCode.OK);
    }

    // 3. Məhsulu yenilə (yalnız sahibi)
    public async Task<BaseResponse<string>> UpdateAsync(ProductUpdateDto dto, string userId)
    {
        var product = await _productRepository.GetByIdAsync(dto.Id);

        if (product is null)
            return new BaseResponse<string>("Məhsul tapılmadı", HttpStatusCode.NotFound);

        if (product.UserId != userId)
            return new BaseResponse<string>("Bu məhsulu yeniləmək hüququnuz yoxdur", HttpStatusCode.Forbidden);

        // Eyni adda başqa məhsul yoxdursa davam et
        var exists = await _productRepository
            .GetByFiltered(p => p.Title.Trim().ToLower() == dto.Title.Trim().ToLower() && p.Id != dto.Id && p.UserId == userId)
            .FirstOrDefaultAsync();

        if (exists is not null)
            return new BaseResponse<string>("Bu adda başqa məhsul artıq mövcuddur", HttpStatusCode.BadRequest);

        product.Title = dto.Title;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.CategoryId = dto.CategoryId;
        // Əgər ImageUrl və ya başqa propertilər əlavə etmək istəyirsənsə, burada doldur

        _productRepository.Update(product);
        await _productRepository.SaveChangeAsync();

        return new BaseResponse<string>("Məhsul uğurla yeniləndi", HttpStatusCode.OK);
    }

    // 4. ID ilə məhsulun detallarını gətir
    public async Task<BaseResponse<ProductGetDto>> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
            return new BaseResponse<ProductGetDto>("Məhsul tapılmadı", HttpStatusCode.NotFound);

        var dto = new ProductGetDto
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            ImageUrl = product.Images?.FirstOrDefault()?.ImageUrl,
          
        };

        return new BaseResponse<ProductGetDto>("Məhsul tapıldı", dto, HttpStatusCode.OK);
    }

    // 5. Bütün məhsullar (filtrsiz)
    public async Task<BaseResponse<List<ProductGetDto>>> GetAllAsync()
    {
        var products = await _productRepository.GetAll().ToListAsync();

        if (products.Count == 0)
            return new BaseResponse<List<ProductGetDto>>("Məhsul tapılmadı", HttpStatusCode.NotFound);

        var dtoList = products.Select(product => new ProductGetDto
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            ImageUrl = product.Images?.FirstOrDefault()?.ImageUrl,
         
        }).ToList();

        return new BaseResponse<List<ProductGetDto>>("Bütün məhsullar", dtoList, HttpStatusCode.OK);
    }

    // 6. Filterə əsasən məhsullar
    public async Task<BaseResponse<List<ProductGetDto>>> GetAllFilteredAsync(ProductFilterDto filter)
    {
        var query = _productRepository.GetAll();

        if (filter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == filter.CategoryId);

        if (filter.MinPrice.HasValue)
            query = query.Where(p => p.Price >= filter.MinPrice);

        if (filter.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= filter.MaxPrice);

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(p => p.Title.Contains(filter.Search) || p.Description.Contains(filter.Search));

        var products = await query.ToListAsync();

        if (products.Count == 0)
            return new BaseResponse<List<ProductGetDto>>("Filterə uyğun məhsul tapılmadı", HttpStatusCode.NotFound);

        var dtoList = products.Select(product => new ProductGetDto
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            ImageUrl = product.Images?.FirstOrDefault()?.ImageUrl,
        }).ToList();

        return new BaseResponse<List<ProductGetDto>>("Filterə uyğun məhsullar", dtoList, HttpStatusCode.OK);
    }

    // 7. Aktiv istifadəçinin (saticinin) məhsulları
    public async Task<BaseResponse<List<ProductGetDto>>> GetMyProductsAsync(string userId)
    {
        var products = await _productRepository.GetByFiltered(p => p.UserId == userId).ToListAsync();

        if (products.Count == 0)
            return new BaseResponse<List<ProductGetDto>>("Heç bir məhsul tapılmadı", HttpStatusCode.NotFound);

        var dtoList = products.Select(product => new ProductGetDto
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            ImageUrl = product.Images?.FirstOrDefault()?.ImageUrl,
        }).ToList();

        return new BaseResponse<List<ProductGetDto>>("Sizin məhsullarınız", dtoList, HttpStatusCode.OK);
    }
}

