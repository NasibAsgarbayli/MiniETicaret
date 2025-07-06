using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using MiniETicaret.Application.Abstracts.Services;
using MiniETicaret.Application.DTOs.ReviewDtos;
using MiniETicaret.Application.Shared;
using MiniETicaret.Domain.Entities;
using MiniETicaret.Persistence.Contexts;

namespace MiniETicaret.Persistence.Services;

public class ReviewService : IReviewService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly MiniETicaretDbContext _context;

    public ReviewService(UserManager<AppUser> userManager,MiniETicaretDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<BaseResponse<string>> CreateAsync(ReviewCreateDto dto, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new BaseResponse<string>("User not found", HttpStatusCode.NotFound);

        var review = new Review
        {
            ProductId = dto.ProductId,
            UserId = userId,
            Content = dto.Content,
            Rating = dto.Rating,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
        return new BaseResponse<string>(review.Id.ToString(), "Review created", HttpStatusCode.Created);


    }

    public async Task<BaseResponse<string>> DeleteAsync(Guid id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return new BaseResponse<string>("Review not found", HttpStatusCode.NotFound);

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return new BaseResponse<string>("Review deleted", HttpStatusCode.OK);

    }

    public async Task<BaseResponse<List<ReviewGetDto>>> GetAllByProductIdAsync(Guid productId)
    {
        var reviews = await _context.Reviews
           .Include(r => r.User)
           .Where(r => r.ProductId == productId)
           .OrderByDescending(r => r.CreatedAt)
           .ToListAsync();

        var result = reviews.Select(r => new ReviewGetDto
        {
            Id = r.Id,
            UserFullName = r.User.Fullname,
            ProductId = r.ProductId,
            Content = r.Content,
            Rating = r.Rating,
            CreatedAt = r.CreatedAt
        }).ToList();
        return new BaseResponse<List<ReviewGetDto>>("Reviews retrieved successfully",result,HttpStatusCode.OK );


    }
}
