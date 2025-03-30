using Domain.Abstractions.Auth;
using Domain.Abstractions.Auth.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Firebase.Auth;

public static class DependencyInjection
{
    public static IServiceCollection InjectFirebaseAuth(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, FirebaseAuthenticationService>();

        services.AddScoped<ICurrentAccount, FirebaseUser>();
        services.AddHttpContextAccessor();
        
        return services;
    }
}