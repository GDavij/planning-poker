using DataAccess.Interceptors;
using Domain.Abstractions.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection InjectDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        // services.AddScoped<ParticipantResourceInterceptor>();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<IApplicationDbContext, ApplicationDbContext>((sp, options) =>
        {
            // options.AddInterceptors(sp.GetRequiredService<ParticipantResourceInterceptor>());
            options.UseAzureSql(connectionString);
        });

        return services;
    }
}