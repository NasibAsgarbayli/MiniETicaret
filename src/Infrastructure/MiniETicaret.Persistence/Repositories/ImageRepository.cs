using MiniETicaret.Application.Abstracts.Repositories;
using MiniETicaret.Domain.Entities;
using MiniETicaret.Persistence.Contexts;

namespace MiniETicaret.Persistence.Repositories;

public class ImageRepository:Repository<Image>,IImageRepository
{
    public ImageRepository(MiniETicaretDbContext context):base(context)
    {
        
    }
}
