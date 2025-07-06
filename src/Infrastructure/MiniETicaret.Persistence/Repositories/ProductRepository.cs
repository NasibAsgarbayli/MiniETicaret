using MiniETicaret.Application.Abstracts.Repositories;
using MiniETicaret.Domain.Entities;
using MiniETicaret.Persistence.Contexts;

namespace MiniETicaret.Persistence.Repositories;

public class ProductRepository:Repository<Product>,IProductRepository
{
	public ProductRepository(MiniETicaretDbContext context):base(context)
	{

	}
}
