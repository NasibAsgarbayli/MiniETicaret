using MiniETicaret.Application.Abstracts.Repositories;
using MiniETicaret.Domain.Entities;
using MiniETicaret.Persistence.Contexts;

namespace MiniETicaret.Persistence.Repositories;

public class ReviewRepository:Repository<Review>,IReviewRepository
{
    public ReviewRepository(MiniETicaretDbContext context):base(context)
    {
        
    }
}
