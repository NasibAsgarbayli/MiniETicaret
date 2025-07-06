using MiniETicaret.Application.Abstracts.Repositories;
using System.Net;
using MiniETicaret.Application.Abstracts.Services;
using MiniETicaret.Application.DTOs.ProductDtos;
using MiniETicaret.Application.Shared;
using MiniETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MiniETicaret.Application.DTOs.ImageDtos;
using System;
using MiniETicaret.Persistence.Contexts;

namespace MiniETicaret.Persistence.Services;
public class ProductService : IProductService
{
    private readonly MiniETicaretDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public ProductService(MiniETicaretDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<BaseResponse<Guid>> CreateAsync(ProductCreateDto dto, string sellerId)
    {
        var seller = await _userManager.FindByIdAsync(sellerId);
        if (seller == null)
            return new BaseResponse<Guid>("Seller not found", Guid.Empty, HttpStatusCode.NotFound);


        var product = new Product
        {
            Name = dto.Name,
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            Barcode = dto.Barcode,
            CategoryId = dto.CategoryId,
            ImageUrl = dto.ImageUrl,
            UserId = sellerId,
            AppUser = seller,
            IsActive = true
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        return new BaseResponse<Guid>("Product created", product.Id, HttpStatusCode.Created);
    }

    public async Task<BaseResponse<string>> UpdateAsync(ProductUpdateDto dto, string sellerId)
    {
        var product = await _context.Products.FindAsync(dto.Id);
        if (product == null)
            return new BaseResponse<string>("Product not found", HttpStatusCode.NotFound);

        if (product.UserId != sellerId)
            return new BaseResponse<string>("You do not have permission to update this product", HttpStatusCode.Forbidden);

        product.Name = dto.Name;
        product.Title = dto.Title;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.Barcode = dto.Barcode;
        product.CategoryId = dto.CategoryId;
        product.ImageUrl = dto.ImageUrl;

        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        return new BaseResponse<string>("Product updated", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> DeleteAsync(Guid id, string sellerId)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return new BaseResponse<string>("Product not found", HttpStatusCode.NotFound);

        if (product.UserId != sellerId)
            return new BaseResponse<string>("You do not have permission to delete this product", HttpStatusCode.Forbidden);

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return new BaseResponse<string>("Product deleted", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<ProductGetDto>> GetByIdAsync(Guid id)
    {
        var product = await _context.Products
            .Include(p => p.AppUser)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return new BaseResponse<ProductGetDto>("Product not found", null, HttpStatusCode.NotFound);

        var dto = MapProductToDto(product);
        return new BaseResponse<ProductGetDto>("Product retrieved successfully", dto, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<List<ProductGetDto>>> GetAllAsync(ProductFilterDto filter = null)
    {
        var query = _context.Products
            .Include(p => p.AppUser)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .AsQueryable();

        if (filter != null)
        {
            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId);

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice);

            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(p => p.Name.Contains(filter.Search) || p.Title.Contains(filter.Search));
        }

        var products = await query
            .OrderByDescending(p => p.Id)
            .ToListAsync();

        var result = products.Select(MapProductToDto).ToList();
        return new BaseResponse<List<ProductGetDto>>("Products retrieved successfully", result, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<List<ProductGetDto>>> GetMyProductsAsync(string sellerId)
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.UserId == sellerId)
            .OrderByDescending(p => p.Id)
            .ToListAsync();

        var result = products.Select(MapProductToDto).ToList();
        return new BaseResponse<List<ProductGetDto>>("Your products retrieved successfully", result, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> AddImageAsync(Guid productId, ImageAddDto dto, string sellerId)
    {
        var product = await _context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
            return new BaseResponse<string>("Product not found", HttpStatusCode.NotFound);

        if (product.UserId != sellerId)
            return new BaseResponse<string>("You do not have permission to add image to this product", HttpStatusCode.Forbidden);

        var image = new Image
        {
            ImageUrl = dto.ImageUrl,
            ProductId = productId
        };

        product.Images.Add(image);
        await _context.SaveChangesAsync();

        return new BaseResponse<string>("Image added to product", HttpStatusCode.Created);
    }

    public async Task<BaseResponse<string>> DeleteImageAsync(Guid productId, Guid imageId, string sellerId)
    {
        var product = await _context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
            return new BaseResponse<string>("Product not found", HttpStatusCode.NotFound);

        if (product.UserId != sellerId)
            return new BaseResponse<string>("You do not have permission to delete image from this product", HttpStatusCode.Forbidden);

        var image = product.Images.FirstOrDefault(i => i.Id == imageId);
        if (image == null)
            return new BaseResponse<string>("Image not found", HttpStatusCode.NotFound);

        product.Images.Remove(image);
        await _context.SaveChangesAsync();

        return new BaseResponse<string>("Image deleted from product", HttpStatusCode.OK);
    }

    // --- Helper mapping ---
    private ProductGetDto MapProductToDto(Product product)
    {
        return new ProductGetDto
        {
            Id = product.Id,
            Name = product.Name,
            Title = product.Title,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Barcode = product.Barcode,
            ImageUrl = product.ImageUrl,
            SellerFullName = product.AppUser?.Fullname,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name,
            AverageRating = product.AverageRating,
            IsActive = product.IsActive,
            Images = product.Images?.Select(i => new ImageGetDto
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl
            }).ToList() ?? new List<ImageGetDto>()
        };
    }
}
