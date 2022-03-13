using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RBS.Auth.Db;

public static class DatabaseConfigExtension
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString("AuthDb")));

        return services;
    }
}