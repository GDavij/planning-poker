using Domain.Abstractions.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection InjectDbContext(this IServiceCollection services)
    {
        services.AddAzureSql<ApplicationDbContext>(Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection"));
        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        return services;
    }
}