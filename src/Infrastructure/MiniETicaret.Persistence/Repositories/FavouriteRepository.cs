using MiniETicaret.Application.Abstracts.Repositories;
using MiniETicaret.Domain.Entities;
using MiniETicaret.Persistence.Contexts;

namespace MiniETicaret.Persistence.Repositories;

public class FavouriteRepository:Repository<Favourite>,IFavouriteRepository
{
    public FavouriteRepository(MiniETicaretDbContext context):base(context)
    {
        
    }
}
