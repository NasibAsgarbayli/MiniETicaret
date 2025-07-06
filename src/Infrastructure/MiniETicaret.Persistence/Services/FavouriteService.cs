using Microsoft.EntityFrameworkCore;
using System.Net;
using MiniETicaret.Application.Abstracts.Services;
using MiniETicaret.Application.DTOs.FavouriteDtos;
using MiniETicaret.Application.Shared;
using MiniETicaret.Domain.Entities;
using MiniETicaret.Persistence.Contexts;

namespace MiniETicaret.Persistence.Services;

public class FavouriteService : IFavouriteService
{
    private readonly MiniETicaretDbContext _context;

    public FavouriteService(MiniETicaretDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<string>> AddAsync(FavouriteAddDto dto, string userId)
    {
        var alreadyExists = await _context.Favourites.AnyAsync(f => f.UserId == userId && f.ProductId == dto.ProductId);
        if (alreadyExists)
            return new BaseResponse<string>("Product already in favourites", HttpStatusCode.BadRequest);

        var favourite = new Favourite
        {
            ProductId = dto.ProductId,
            UserId = userId,
            AddedAt = DateTime.UtcNow
        };

        await _context.Favourites.AddAsync(favourite);
        await _context.SaveChangesAsync();
        return new BaseResponse<string>("Product added to favourites", HttpStatusCode.Created);
    }

    public async Task<BaseResponse<List<FavouriteGetDto>>> GetAllAsync(string userId)
    {
        var favourites = await _context.Favourites
            .Include(f => f.Product)
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.AddedAt)
            .ToListAsync();

        var result = favourites.Select(f => new FavouriteGetDto
        {
            Id = f.Id,
            ProductId = f.ProductId,
            ProductName = f.Product.Name,
            AddedAt = f.AddedAt
        }).ToList();

        return new BaseResponse<List<FavouriteGetDto>>("Favourites retrieved successfully", result, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> RemoveAsync(Guid productId, string userId)
    {
        var favourite = await _context.Favourites
          .FirstOrDefaultAsync(f => f.ProductId == productId && f.UserId == userId);

        if (favourite == null)
            return new BaseResponse<string>("Favourite not found", HttpStatusCode.NotFound);

        _context.Favourites.Remove(favourite);
        await _context.SaveChangesAsync();
        return new BaseResponse<string>("Product removed from favourites", HttpStatusCode.OK);
    }
}
