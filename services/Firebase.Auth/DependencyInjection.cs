using Domain.Abstractions.Auth;
using Domain.Abstractions.Auth.Models;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Firebase.Auth;

public static class DependencyInjection
{
    public static IServiceCollection AddFirebaseAuth(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.AddScoped<IAuthenticationService, FirebaseAuthenticationService>();

        services.AddScoped<ICurrentAccount, FirebaseUser>();
        services.AddHttpContextAccessor();
     
        FirebaseApp.Create(new AppOptions
        {
            Credential = configuration.GetSection("Firebase:UseJson").Value == "True"
                ? GoogleCredential.FromJson(configuration.GetSection("Firebase:CredentialJson").Value)
                : GoogleCredential.FromFile(configuration.GetSection("Firebase:CredentialPath").Value)
        });
        
        return services;
    }
}