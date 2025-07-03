using Microsoft.Extensions.DependencyInjection;
using MiniETicaret.Application.Abstracts.Services;
using MiniETicaret.Persistence.Services;

namespace MiniETicaret.Persistence;

public static class ServiceRegistration
{
    public static void RegisterService(this IServiceCollection services)
    {
        #region Repositories

        #endregion
        #region Services
        services.AddScoped<IUserService, UserService>();
        #endregion
    }
}
