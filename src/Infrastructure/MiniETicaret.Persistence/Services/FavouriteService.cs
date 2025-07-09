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
        var existing = await _context.Favourites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == dto.ProductId);

        if (existing != null)
        {
            if (!existing.IsDeleted)
                return new BaseResponse<string>("Product already in favourites", HttpStatusCode.BadRequest);

            // Soft deleted idi, indi bərpa et
            existing.IsDeleted = false;
            existing.AddedAt = DateTime.UtcNow;
            _context.Favourites.Update(existing);
            await _context.SaveChangesAsync();
            return new BaseResponse<string>("Product re-added to favourites", HttpStatusCode.Created);
        }

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
            .Where(f => f.UserId == userId && !f.IsDeleted)
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
          .FirstOrDefaultAsync(f => f.ProductId == productId && f.UserId == userId && !f.IsDeleted);

        if (favourite == null)
            return new BaseResponse<string>("Favourite not found", HttpStatusCode.NotFound);

        favourite.IsDeleted = true;
        _context.Favourites.Update(favourite);
        await _context.SaveChangesAsync();
        return new BaseResponse<string>("Product removed from favourites", HttpStatusCode.OK);
    }
}
