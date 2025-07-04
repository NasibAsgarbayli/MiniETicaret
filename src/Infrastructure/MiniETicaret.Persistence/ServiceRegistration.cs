using Microsoft.Extensions.DependencyInjection;
using MiniETicaret.Application.Abstracts.Repositories;
using MiniETicaret.Application.Abstracts.Services;
using MiniETicaret.Persistence.Repositories;
using MiniETicaret.Persistence.Services;

namespace MiniETicaret.Persistence;

public static class ServiceRegistration
{
    public static void RegisterService(this IServiceCollection services)
    {
        #region Repositories
        services.AddScoped<ICategoryRepository,CategoryRepository>();
        services.AddScoped<IFavouriteRepository,FavouriteRepository>();
        services.AddScoped<IImageRepository,ImageRepository>();
        services.AddScoped<IOrderRepository,OrderRepository>();
        services.AddScoped<IProductRepository,ProductRepository>();
        services.AddScoped<IReviewRepository,ReviewRepository>();

        #endregion
        #region Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        
        #endregion
    }
}
