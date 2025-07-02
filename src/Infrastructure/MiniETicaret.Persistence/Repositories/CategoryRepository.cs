using MiniETicaret.Application.Abstracts.Repositories;
using MiniETicaret.Domain.Entities;
using MiniETicaret.Persistence.Contexts;

namespace MiniETicaret.Persistence.Repositories;

public class CategoryRepository:Repository<Category>,ICategoryRepository
{
    public CategoryRepository(MiniETicaretDbContext context):base(context)
    {
        
    }
}
